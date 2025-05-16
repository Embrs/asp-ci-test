using Microsoft.EntityFrameworkCore;
using MyApp.Db;
using MyApp.Services;

namespace MyApp.Filters;

// 會驗證token 與尋找token user
public class RequireUserFilter : IEndpointFilter {
  public async ValueTask<object?> InvokeAsync(
    EndpointFilterInvocationContext context,
    EndpointFilterDelegate next
  ) {
    var httpContext = context.HttpContext;
    
    var db = httpContext.RequestServices.GetRequiredService<AppDbContext>();
    var redis = httpContext.RequestServices.GetRequiredService<RedisService>();
    // check toekn
    var tokenInfo = await redis.GetTokenInfo(httpContext);

    if (tokenInfo == null) return Results.Unauthorized();

    // get user
    var user = await db.Users.FirstOrDefaultAsync(u => u.PublicId == tokenInfo.userId);
    if (user == null) return Results.NotFound();

    httpContext.Items["tokenUser"] = user;

    return await next(context);
  }
}
