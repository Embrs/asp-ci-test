using System.Text.Json;
using MyApp.Models;
using StackExchange.Redis;

namespace MyApp.Services;

public class RedisService(IConnectionMultiplexer connection) {
  private readonly IDatabase _db = connection.GetDatabase();
  private const int DefaultTokenTTLSeconds = 1800; // 預設 TTL：30 分鐘

  // 檢查 Redis Key 是否存在
  private Task<bool> ExistsAsync(string redisKey) => _db.KeyExistsAsync(redisKey);

  // 設定 Redis 字串值（可選擇性設定 TTL）
  private Task<bool> SetAsync(string redisKey, string jsonStr, TimeSpan? expiry = null) =>
    _db.StringSetAsync(redisKey, jsonStr, expiry);

  // 取得 Redis 字串值
  private Task<RedisValue> GetAsync(string redisKey) => _db.StringGetAsync(redisKey);

  // 刪除 Redis Key
  private Task<bool> DeleteAsync(string redisKey) => _db.KeyDeleteAsync(redisKey);

  // 指定新的剩餘有效時間
  private Task<bool> RefreshExpire(string key, TimeSpan expiry) =>
    _db.KeyExpireAsync(key, expiry);

  // 提取 HTTP Context 中的 Bearer Token
  private string? ContextToken(HttpContext context) {
    try {
      var authHeader = context.Request.Headers.Authorization.ToString();
      if (!authHeader.StartsWith("Bearer ")) return null;
      return authHeader["Bearer ".Length..].Trim();
    }
    catch {
      return null;
    }
  }

  // 取得使用者 IP（預設為 127.0.0.1）
  private string ContextIp(HttpContext context) {
    try {
      return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }
    catch {
      return "127.0.0.1";
    }
  }

  // 建立 token 並儲存至 Redis
  public async Task<string?> CreateToken(User user, HttpContext context) {
    try {
      var ip = ContextIp(context);
      var token = Guid.NewGuid().ToString("N");
      var ttl = TimeSpan.FromSeconds(DefaultTokenTTLSeconds);

      var tokenData = new TokenInfo {
        userId = user.PublicId,
        ip = ip,
      };

      var tokenJson = JsonSerializer.Serialize(tokenData);
      var tokenKey = $"token:{token}";
      var userSetKey = $"user_tokens:{user.PublicId}";

      // 使用 batch 提高效能：同時寫入 token 與對應 user 的 Set
      var batch = _db.CreateBatch();
      var setTokenTask = batch.StringSetAsync(tokenKey, tokenJson, ttl);
      var addToUserSetTask = batch.SetAddAsync(userSetKey, token);
      var expireUserSetTask = batch.KeyExpireAsync(userSetKey, TimeSpan.FromDays(1));
      batch.Execute();

      await Task.WhenAll(setTokenTask, addToUserSetTask, expireUserSetTask);

      return token;
    }
    catch {
      return null;
    }
  }

  // 依據 HTTP Context 中的 token 取得對應資訊
  public async Task<TokenInfo?> GetTokenInfo(HttpContext context) {
    try {
      var token = ContextToken(context);
      var ip = ContextIp(context);
      if (string.IsNullOrWhiteSpace(token)) return null;

      var redisKey = $"token:{token}";
      var infoJson = await GetAsync(redisKey);
      if (infoJson.IsNullOrEmpty) return null;

      var info = JsonSerializer.Deserialize<TokenInfo>(infoJson!);
      if (info == null || info.ip != ip) return null;

      return info;
    }
    catch {
      return null;
    }
  }

  // 延長指定 token 的存活時間
public async Task<bool> ExtendToken(string token, int seconds) {
  try {
    var key = $"token:{token}";
    var json = await GetAsync(key);
    if (json.IsNullOrEmpty) return false;

    var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(json!);
    if (tokenInfo == null) return false;

    var userSetKey = $"user_tokens:{tokenInfo.userId}";

    // 建立 batch（非 atomic，但效能好）
    var batch = _db.CreateBatch();
    var task1 = batch.StringSetAsync(key, json!, TimeSpan.FromSeconds(seconds));
    var task2 = batch.KeyExpireAsync(userSetKey, TimeSpan.FromDays(1));
    batch.Execute();
    await Task.WhenAll(task1, task2);

    return true;
  }
  catch {
    return false;
  }
}

  // 刪除目前使用者的 token（從 context 取得）
  public async Task<bool> DeleteToken(HttpContext context) {
    try {
      var token = ContextToken(context);
      if (string.IsNullOrEmpty(token)) return false;

      return await DeleteAsync($"token:{token}");
    }
    catch {
      return false;
    }
  }

  // ✅ 批次刪除某個使用者的所有 token（強制登出）
  public async Task<int> InvalidateAllTokens(string userId) {
    try {
      var userSetKey = $"user_tokens:{userId}";
      var tokens = await _db.SetMembersAsync(userSetKey);

      if (tokens.Length == 0) return 0;

      var keys = tokens.Select(t => (RedisKey)$"token:{t}").ToArray();
      var deletedCount = await _db.KeyDeleteAsync(keys);

      await DeleteAsync(userSetKey); // 同時刪除 set 本身

      return (int)deletedCount;
    }
    catch {
      return 0;
    }
  }
}