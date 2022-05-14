using System;
using Todos.Domain;

namespace FunctionAppWithCosmosDb.API;
internal static class Mappings
{
    public static TodoTableEntity ToTableEntity(this Todo todo)
    {
        return new TodoTableEntity
        {
            PartitionKey = Constants.PartitionKey,
            RowKey = todo.Id.ToString(),
            IsDone = todo.IsDone,
            Text = todo.Text
        };
    }

    public static Todo ToTodo(this TodoTableEntity todo)
    {
        return new Todo(todo.Text)
        {
            Id = Guid.Parse(todo.RowKey),
            CreatedOn = todo.Timestamp ?? DateTimeOffset.MinValue,
            IsDone = todo.IsDone
        };
    }
}
