namespace MyApp.Endpoints;

using Data;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

public static class TodoEndpoints {
  public static void MapTodoEndpoints(this IEndpointRouteBuilder app) {
    var group = app.MapGroup("/api/todo")
      .RequireAuthorization(); // 🔐 加入授權

    group.MapGet("/", GetAll);
    group.MapGet("/{id:int}", GetById);
    group.MapPost("/", Create);
    group.MapPut("/{id:int}", Update);
    group.MapDelete("/{id:int}", Delete);
  }

  static async Task<IResult> GetAll(AppDbContext db) {
    var todoList = await db.Todos.ToListAsync();
    return Results.Ok(todoList);
  }
  
  static async Task<IResult> GetById(int id, AppDbContext db) {
    var todoItem = await db.Todos.FindAsync(id);
    return todoItem is not null ? Results.Ok(todoItem) : Results.NotFound();
  }

  static async Task<IResult> Create(TodoItem input, AppDbContext db) {
    db.Todos.Add(input);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todo/{input.Id}", input);
  }

  static async Task<IResult> Update(int id, TodoItem input, AppDbContext db) {
    var todoItem = await db.Todos.FindAsync(id);
    if (todoItem is null) return Results.NotFound();

    todoItem.Title = input.Title;
    todoItem.IsCompleted = input.IsCompleted;

    await db.SaveChangesAsync();
    return Results.Ok(todoItem);
  }

  static async Task<IResult> Delete(int id, AppDbContext db) {
    var todoItem = await db.Todos.FindAsync(id);
    if (todoItem is null) return Results.NotFound();

    db.Todos.Remove(todoItem);
    await db.SaveChangesAsync();
    return Results.NoContent();
  }
}
