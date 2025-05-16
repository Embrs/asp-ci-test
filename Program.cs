using MyApp.Plugins;
using MyApp.Services;
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddPostgresDb(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddSwagger();

var app = builder.Build();

app.InitPostgresDb();
app.InitSwagger();
app.InitApi();

app.Run();