using MyApp.Plugins;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.SettingPostgresDb(builder.Configuration);
builder.Services.SettingJwt(builder.Configuration);
builder.Services.SettingSwagger(builder.Configuration);

var app = builder.Build();

app.InitPostgresDb();

app.Use(async (context, next) =>
{
    Console.WriteLine($"[Pipeline] Request path: {context.Request.Path}");
    await next();
});

app.InitJwt();
app.InitApi();
app.InitSwagger();

app.Run();