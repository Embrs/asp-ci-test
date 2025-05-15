using MyApp.Plugins;
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.SettingJwt(builder.Configuration);
builder.Services.SettingPostgresDb(builder.Configuration);
builder.Services.SettingSwagger(builder.Configuration);

var app = builder.Build();
app.InitJwt();
app.InitPostgresDb();
app.InitSwagger();
app.InitApi();

app.Run();