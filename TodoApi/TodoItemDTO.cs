using TodoApi;


public class TodoItemDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    public TodoItemDTO() { }
    public TodoItemDTO(Todo todoItem)
    {
        Id = todoItem.Id;
        Name = todoItem.Name;
        IsComplete = todoItem.IsComplete;
    }

    /*
   public static Todo ToTodo(TodoItemDTO todoItemDTO)
    {
        if (todoItemDTO == null) return null;
        return new Todo
        {
            Id = todoItemDTO.Id,
            Name = todoItemDTO.Name,
            IsComplete = todoItemDTO.IsComplete
        };
       
    }
    */

   public static List<TodoItemDTO> ToTodoDTOList(List<Todo> todos) 
    {
        if (todos == null) return null;
        var todoDTOList = new List<TodoItemDTO>();
        foreach(var todo in todos)
        {
            todoDTOList.Add(new TodoItemDTO(todo));
        }
        return todoDTOList;
    }
   

}