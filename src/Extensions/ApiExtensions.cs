using Microsoft.OpenApi.Models;
using MyApp.Endpoints;

public static class ApiExtensions {  
  // API
  public static void InitApi(this WebApplication app) {
    app.MapAuthEndpoints();
  }
}