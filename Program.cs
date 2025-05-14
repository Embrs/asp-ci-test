using MyApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SettingSwagger(builder.Configuration);
builder.Services.SettingPostgresDb(builder.Configuration);
builder.Services.SettingJwt(builder.Configuration);

var app = builder.Build();
app.InitSwagger();
app.InitPostgresDb();
app.InitJwt();
app.InitApi();

app.Run();