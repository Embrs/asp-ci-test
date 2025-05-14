namespace MyApp.Plugins;

using MyApp.Apis;
public static class ApiPlugins {  
  // API
  public static void InitApi(this WebApplication app) {
    app.InitAuthApis();
  }
}