using MyApp.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCorsServices();
builder.Services.AddJwtServices(builder.Configuration);
builder.Services.AddApiServices();
builder.Services.AddPostgresDb(builder.Configuration);

var app = builder.Build();
app.UseSwaggerMiddlewares();
app.UseCorsMiddlewares();
app.UseJwtMiddlewares();
app.UseApiMiddlewares();
app.UsePostgresDb();

app.Run();
