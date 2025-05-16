
namespace MyApp.Plugins;

using MyApp.Services;
using StackExchange.Redis;

public static class RedisPlugins {  
  // JWT 設定
  public static IServiceCollection AddRedis (this IServiceCollection services, IConfiguration config) {
    try {
      services.AddSingleton<IConnectionMultiplexer>(sp => {
        var redisHost = config.GetConnectionString("Redis") ?? "localhost:6379";
        // var options = ConfigurationOptions.Parse(redisHost, true);
        return ConnectionMultiplexer.Connect(redisHost);
      });

      services.AddSingleton<RedisService>();
    }
    catch (Exception) {
      Console.WriteLine($"[AddRedis ERROR]");  
    }
    Console.WriteLine($"[AddRedis OK]");
    return services;
  } 

  // 建立資料庫
  public static WebApplication InitRedis (this WebApplication app) {
    try {
      // TODO
    } catch (Exception ex) {
      var env = app.Environment;
      Console.WriteLine($"[InitReids ERROR] {env.EnvironmentName}: {ex.Message}");  
    }
    Console.WriteLine($"[InitReids OK]");
    return app;
  }
}