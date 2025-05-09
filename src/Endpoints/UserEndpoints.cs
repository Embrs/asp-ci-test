using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MyApp.Models;
using MyApp.Repositories;

namespace MyApp.Endpoints;

public static class UserEndpoints {
  public static void MapUserEndpoints(this IEndpointRouteBuilder app) {
    var group = app.MapGroup("/api/user"); // ✅ 集中管理 prefix

    group.MapGet("/", GetAll);
    group.MapGet("/{id:int}", GetById);
    group.MapPost("/", Create);
    group.MapPut("/{id:int}", Update);
    group.MapDelete("/{id:int}", Delete);
  }

  static IResult GetAll(UserRepository repo) =>
    Results.Ok(repo.GetAll());

  static IResult GetById(int id, UserRepository repo) {
    var user = repo.GetById(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
  }

  static IResult Create(UserDto dto, UserRepository repo) {
    var user = repo.Create(dto.Username, dto.Email);
    return Results.Created($"/api/users/{user.Id}", user);
  }

  static IResult Update(int id, UserDto dto, UserRepository repo) {
    var success = repo.Update(id, dto.Username, dto.Email);
    return success ? Results.NoContent() : Results.NotFound();
  }

  static IResult Delete(int id, UserRepository repo) {
    var success = repo.Delete(id);
    return success ? Results.NoContent() : Results.NotFound();
  }
}
