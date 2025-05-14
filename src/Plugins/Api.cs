namespace MyApp.Plugins;

using MyApp.Apis;
public static class ApiPlugins {  
  // API
  public static WebApplication InitApi(this WebApplication app) {
    try {
      app.InitAuthApis();
    } catch (Exception) {}
    return app;
  }
}