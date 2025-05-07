using TodoApi.Extensions;
using TodoApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// ✅ 註冊服務與自訂設定
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();
app.UseAppMiddlewares();

// ✅ 註冊 API endpoints
app.MapAuthEndpoints();
app.MapTodoEndpoints();
app.MapUserEndpoints();

app.Run();
