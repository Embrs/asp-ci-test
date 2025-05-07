using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos")
            .RequireAuthorization(); // 🔐 加入授權

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);
    }

    static IResult GetAll(TodoRepository repo) =>
        Results.Ok(repo.GetAll());

    static IResult GetById(int id, TodoRepository repo)
    {
        var todo = repo.GetById(id);
        return todo is null ? Results.NotFound() : Results.Ok(todo);
    }

    static IResult Create(TodoDto dto, TodoRepository repo)
    {
        var todo = repo.Create(dto.Title);
        return Results.Created($"/api/todos/{todo.Id}", todo);
    }

    static IResult Update(int id, TodoDto dto, TodoRepository repo)
    {
        var success = repo.Update(id, dto.Title, dto.IsDone);
        return success ? Results.NoContent() : Results.NotFound();
    }

    static IResult Delete(int id, TodoRepository repo)
    {
        var success = repo.Delete(id);
        return success ? Results.NoContent() : Results.NotFound();
    }
}
