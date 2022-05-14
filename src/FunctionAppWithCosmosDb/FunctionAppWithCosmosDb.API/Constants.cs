namespace FunctionAppWithCosmosDb.API;
internal static class Constants
{
    internal const string TableName = "todos";
    internal const string TableConnectionKey = "AzureWebJobsStorage";
    internal const string PartitionKey = "TODO";
}
