using TodoApi.Extensions;
using TodoApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// 設置配置
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// 註冊 CORS 服務
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()  // 允許來自任何來源的請求
              .AllowAnyHeader()  // 允許任何標頭
              .AllowAnyMethod(); // 允許任何方法（GET、POST 等）
    });
});

// 註冊應用服務與 JWT 認證等
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

// 啟用 CORS 中介軟體
app.UseCors();

// 使用中介軟體
app.UseAppMiddlewares();

// 註冊 API 端點
app.MapAuthEndpoints();
app.MapTodoEndpoints();
app.MapUserEndpoints();

app.Run();
