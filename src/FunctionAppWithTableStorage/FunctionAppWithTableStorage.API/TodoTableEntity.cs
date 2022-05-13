using System;
using Azure;
using Azure.Data.Tables;

namespace FunctionAppWithTableStorage.API;
internal class TodoTableEntity : BaseTableEntity
{
    public string Text { get; set; }
    public bool IsDone { get; set; }
}

internal class BaseTableEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
