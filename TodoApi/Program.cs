using Microsoft.EntityFrameworkCore;
using TodoApi;
using static TodoItemDTO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);
todoItems.MapPost("/addmore", MoreTodos);
todoItems.MapGet("/setcomplete/{id}", SetComplete);

app.Run();

static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(ToTodoDTOList(await db.Todos.ToListAsync()));
}

static async Task<IResult> GetCompleteTodos(TodoDb db)
{
    return TypedResults.Ok(ToTodoDTOList(await db.Todos.Where(t => t.IsComplete).ToListAsync()));
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
            ? TypedResults.Ok(new TodoItemDTO(todo))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoItemDTO todoDTO, TodoDb db)
{
    var todo = ToTodo(todoDTO);
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/todoitems/{todoDTO.Id}", todoDTO);
}

static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoDTO, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = todoDTO.Name;
    todo.IsComplete = todoDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.Ok($"Todo {todo.Id} deleted");
    }
    
    return TypedResults.NotFound();
}


static async Task<IResult> MoreTodos (List<TodoItemDTO> todoDTOs, TodoDb db)
{
    var todos = ToTodoList(todoDTOs);
    db.Todos.AddRange(todos);
    await db.SaveChangesAsync();

    return TypedResults.Ok(todoDTOs);
}

static async Task<IResult> SetComplete (int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is not null)
    {
        var todo = await db.Todos.FindAsync(id);
        if(todo.IsComplete) return TypedResults.Ok("Task is already complete");
        if (!todo.IsComplete)
        {
            todo.IsComplete = true;
            db.Todos.Update(todo);
            await db.SaveChangesAsync();
            return TypedResults.Ok(new TodoItemDTO(todo));
        }

    }
    return TypedResults.NotFound();

}

static Todo ToTodo(TodoItemDTO todoItemDTO)
{
    if (todoItemDTO == null) return null;
    return new Todo
    {
        Id = todoItemDTO.Id,
        Name = todoItemDTO.Name,
        IsComplete = todoItemDTO.IsComplete
    };

}

static List<Todo> ToTodoList(List<TodoItemDTO> todoItemDTOs)
{
    if (todoItemDTOs == null) return null;
    var todoList = new List<Todo>();
    foreach (var todoDTO in todoItemDTOs)
    {
        todoList.Add(ToTodo(todoDTO));
    }
    return todoList;
}


