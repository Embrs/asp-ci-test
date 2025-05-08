using MyApp.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AdCorsServices();
builder.Services.AddJwtServices(builder.Configuration);
builder.Services.AddAppServices();

var app = builder.Build();
app.UseSwaggerMiddlewares();
app.UseCorsMiddlewares();
app.UseJwtMiddlewares();
app.UseAppMiddlewares();

app.Run();
