namespace MyApp.Apis;

using MyApp.ApiParams;
using MyApp.Models;
using MyApp.Db;
using Microsoft.EntityFrameworkCore;
using MyApp.Services;


public static class UserApis {
	public static WebApplication InitUserApis(this WebApplication app) {
		var baseGroup = app.MapGroup("/api/user");
		baseGroup.MapGet("/{id:int}", ApiGetUser);
		baseGroup.MapPut("/{id:int}", ApiSetUser);
		baseGroup.MapDelete("/{id:int}", ApiDelUser);
		return app;
	}

	/** 取得用戶 */
	private static async Task<IResult> ApiGetUser(
		int id, 
		AppDbContext db,
		HttpContext context, 
		JwtService jwtService
	) {
		var tokenUserId = jwtService.CheckToken(context);
		if (tokenUserId == null) return Results.Unauthorized();

		var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
		if (user == null) return Results.NotFound();
		return Results.Ok(user);
	}

	/** 更新用戶 */
	private static async Task<IResult> ApiSetUser(
		int id, 
		AppDbContext db,
		HttpContext context, 
		JwtService jwtService,
	 	User apiParams
	) {
		var tokenUserId = jwtService.CheckToken(context);
		if (tokenUserId == null) return Results.Unauthorized();

		var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
		if (user == null) return Results.NotFound();
		user.DisplayName = apiParams.DisplayName;
		await db.SaveChangesAsync();
		return Results.NoContent();
	}

	/** 刪除用戶 */	
	private static async Task<IResult> ApiDelUser(
		int id, 
		AppDbContext db,
		HttpContext context, 
		JwtService jwtService,
	 	User apiParams
	) {
		var tokenUserId = jwtService.CheckToken(context);
		if (tokenUserId == null) return Results.Unauthorized();

		var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
		if (user == null) return Results.NotFound();
		db.Users.Remove(user);
		await db.SaveChangesAsync();
		return Results.NoContent();
	}
}