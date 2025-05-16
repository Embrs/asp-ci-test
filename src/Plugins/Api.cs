namespace MyApp.Plugins;

using MyApp.Apis;
public static class ApiPlugins {  
  // API
  public static WebApplication InitApi(this WebApplication app) {
    try {
      app.InitBaseApis();
      app.InitUserApis();
    } catch (Exception) {
      Console.WriteLine($"[InitApi ERROR]");  
    }
    Console.WriteLine($"[InitApi OK]");  
    return app;
  }
}