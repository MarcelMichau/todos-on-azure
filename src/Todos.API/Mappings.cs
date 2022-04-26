using Todos.Domain;

namespace Todos.API;
internal static class Mappings
{
    public static TodoTableEntity ToTableEntity(this Todo todo)
    {
        return new TodoTableEntity
        {
            PartitionKey = "TODO",
            RowKey = todo.Id.ToString(),
            IsDone = todo.IsDone,
            Text = todo.Text
        };
    }

    //public static Todo ToTodo(this TodoTableEntity todo)
    //{
    //    return new Todo
    //    {
    //        Id = todo.RowKey,
    //        IsCompleted = todo.IsDone,
    //        TaskDescription = todo.Text
    //    };
    //}
}
