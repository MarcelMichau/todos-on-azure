using System.Net;
using System.Net.Mime;
using System.Text.Json;
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
using Todos.API.Models;
using Todos.Domain;

namespace Todos.API.Functions;
internal class UpdateTodo
{
    private readonly ILogger<CreateTodo> _logger;

    public UpdateTodo(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName("UpdateTodo")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "UpdateTodo" })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** route parameter")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(UpdateTodoModel))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Todo), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todos/{id}")] HttpRequest request,
        [Table(Constants.TableName, Connection = Constants.TableConnectionKey)] TableClient todoTable, string id)
    {
        _logger.LogInformation("Updating todo with ID: {id}", id);

        var updated = await JsonSerializer.DeserializeAsync<UpdateTodoModel>(request.Body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        TodoTableEntity existingRow;
        try
        {
            var findResult = await todoTable.GetEntityAsync<TodoTableEntity>(Constants.PartitionKey, id);
            existingRow = findResult.Value;
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
            _logger.LogInformation($"Todo with ID: {id} not found");
            return new NotFoundResult();
        }

        var existingTodo = existingRow.ToTodo();

        if (updated != null)
        {
            existingTodo.UpdateText(updated.Text);

            if (updated.IsDone)
                existingTodo.MarkAsDone();
            else 
                existingTodo.MarkAsNotDone();
        }

        await todoTable.UpdateEntityAsync(existingTodo.ToTableEntity(), existingRow.ETag, TableUpdateMode.Replace);

        _logger.LogInformation("Updated todo with ID: {id}", id);

        var updatedRow = await todoTable.GetEntityAsync<TodoTableEntity>(Constants.PartitionKey, id);

        return new OkObjectResult(updatedRow.Value.ToTodo());
    }
}
