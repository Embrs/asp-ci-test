namespace MyApp.Endpoints;

using MyApp.Models;
using MyApp.Repositories;

public static class DemoEndpoints {
  public static void MapDemoEndpoints(this IEndpointRouteBuilder app) {
    var group = app.MapGroup("/api/demo")
      .RequireAuthorization(); // 🔐 加入授權

    group.MapGet("/", GetAll);
    group.MapGet("/{id:int}", GetById);
    group.MapPost("/", Create);
    group.MapPut("/{id:int}", Update);
    group.MapDelete("/{id:int}", Delete);
  }

  static IResult GetAll(DemoRepository repo) =>
    Results.Ok(repo.GetAll());

  static IResult GetById(int id, DemoRepository repo) {
    var dmoeIetm = repo.GetById(id);
    return dmoeIetm is null ? Results.NotFound() : Results.Ok(dmoeIetm);
  }

  static IResult Create(DemoDto dto, DemoRepository repo) {
    var dmoeIetm = repo.Create(dto.Title);
    return Results.Created($"/api/todos/{dmoeIetm.Id}", dmoeIetm);
  }

  static IResult Update(int id, DemoDto dto, DemoRepository repo) {
    var success = repo.Update(id, dto.Title, dto.IsComplete);
    return success ? Results.NoContent() : Results.NotFound();
  }

  static IResult Delete(int id, DemoRepository repo) {
    var success = repo.Delete(id);
    return success ? Results.NoContent() : Results.NotFound();
  }
}
