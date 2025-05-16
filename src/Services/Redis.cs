using System.Text.Json;
using MyApp.Models;
using StackExchange.Redis;

namespace MyApp.Services;

public class RedisService {
  private readonly IDatabase _db;
  private RedisService(IConnectionMultiplexer connection) {
    _db = connection.GetDatabase();
  }
  // Key 是否存在
  private Task<bool> ExistsAsync(string redisKey) => _db.KeyExistsAsync(redisKey);
  
  // 設定
  private Task<bool> SetAsync(
    string redisKey, 
    string jsonStr, 
    TimeSpan? redisExpiry = null
  ) => _db.StringSetAsync(redisKey, jsonStr, redisExpiry);

  // 取得
  private Task<RedisValue> GetAsync(string redisKey) => _db.StringGetAsync(redisKey);

  // 刪除
  private Task<bool> DeleteAsync(string redisKey)  => _db.KeyDeleteAsync(redisKey);
  
  // 取得剩餘生存時間
  private Task<TimeSpan?> GetTtlAsync(string redisKey) => _db.KeyTimeToLiveAsync(redisKey);

  // 延長到期時間
  private Task<bool> RefreshExpire(string key, TimeSpan expiry) => _db.KeyExpireAsync(key, expiry);

  // 提取 context token
  private string? ContextToken(HttpContext context) {
    try {
      var authHeader = context.Request.Headers.Authorization.ToString();
      if (!authHeader.StartsWith("Bearer ")) return null;
      return authHeader.Substring("Bearer ".Length).Trim();
    } catch (Exception) {
      return null;
    }
  }

  // 提取 context Ip
  private string ContextIp(HttpContext context) {
    try {
      return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"; 
    } catch (Exception) {
      return "127.0.0.1";
    }
  }
  // 產生 token
  public async Task<string?> CreateToken(User user, HttpContext context) {
    try {
      var ip = ContextIp(context);  

      var token = Guid.NewGuid().ToString("N");
      var sec = 30 * 60; // 30 分鐘後到期
      var expiresAt = DateTime.UtcNow.AddSeconds(sec); 

      var tokenData = new TokenInfo {
        userId = user.PublicId,
        ip = ip,
        expiresAt = expiresAt,
      };

      string tokenJson = JsonSerializer.Serialize(tokenData);
      string redisKey = $"token:{token}";
      TimeSpan ttl = TimeSpan.FromSeconds(sec);

      await SetAsync(redisKey, tokenJson, ttl);
      return token; 
    } catch (Exception) {
      return null;
    }
  } 

  // 取得 token
  public async Task<TokenInfo?> GetTokenInfo(HttpContext context) {
    try {
      var token = ContextToken(context);
      var ip = ContextIp(context);  
      if (string.IsNullOrWhiteSpace(token)) return null;

      string redisKey = $"token:{token}";
      var infoJson = await GetAsync(redisKey);
      if (infoJson.IsNullOrEmpty) return null;

      var info = JsonSerializer.Deserialize<TokenInfo>(infoJson!);
      if (info == null) return null;
      if (info.ip != ip) return null;
      return info;
    } catch (Exception) {
      return null;
    }
  } 

  // 延長 token
  public async Task<bool> ExtendToken(string token, int sec) {
    try {
      string redisKey = $"token:{token}";
      var infoJson = await GetAsync(redisKey);
      if (infoJson.IsNullOrEmpty) return false;

      var info = JsonSerializer.Deserialize<TokenInfo>(infoJson!);
      if (info == null) return false;

      // 更新 expiresAt
      info.expiresAt = DateTime.UtcNow.AddSeconds(sec);

      // 更新 Redis TTL 與內容
      var newJson = JsonSerializer.Serialize(info);
      TimeSpan extendBy = TimeSpan.FromSeconds(sec);
      await SetAsync(redisKey, newJson, extendBy);
      return true;
    }
    catch {
      return false;
    }
  }
  
  // 刪除 token
  public async Task<bool> DeleteToken(string token) {
    try {
      string redisKey = $"token:{token}";
      return await DeleteAsync(redisKey);
    }
    catch {
      return false;
    }
  }
}