using MyApp.Plugins;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SettingPostgresDb(builder.Configuration);
builder.Services.SettingJwt(builder.Configuration);
builder.Services.SettingSwagger(builder.Configuration);

var app = builder.Build();
app.InitApi();
app.InitPostgresDb();
app.InitJwt();
app.InitSwagger();

app.Run();