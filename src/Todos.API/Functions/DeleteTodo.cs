using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Todos.API.Functions;
internal class DeleteTodo
{
    private readonly ILogger<CreateTodo> _logger;

    public DeleteTodo(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName("DeleteTodo")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "DeleteTodo" })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** route parameter")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todos/{id}")] HttpRequest request,
        [Table(Constants.TableName, Connection = Constants.TableConnectionKey)] TableClient todoTable, string id)
    {
        try
        {
            _logger.LogInformation("Getting todo with ID: {id}", id);
            await todoTable.DeleteEntityAsync(Constants.PartitionKey, id, ETag.All);
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
            _logger.LogInformation($"Todo with ID: {id} not found");
            return new NotFoundResult();
        }
        return new OkResult();
    }
}
