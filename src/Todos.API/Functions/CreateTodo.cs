using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Todos.Domain;

namespace Todos.API.Functions;

internal class CreateTodo
{
    private readonly ILogger<CreateTodo> _logger;

    public CreateTodo(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName(nameof(CreateTodo))]
    [OpenApiOperation(operationId: nameof(CreateTodo), tags: new[] { nameof(CreateTodo) })]
    [OpenApiParameter(name: "todoText", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **TodoText** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Todo), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, nameof(HttpMethod.Post), Route = "todos")] HttpRequest request,
        [Table(Constants.TableName, Connection = Constants.TableConnectionKey)] TableClient todoTable)
    {
        _logger.LogInformation("Creating a new Todo");

        string todoText = request.Query["todoText"];

        var newTodo = new Todo(todoText);

        await todoTable.AddEntityAsync(newTodo.ToTableEntity());

        var newRow = await todoTable.GetEntityAsync<TodoTableEntity>(Constants.PartitionKey, newTodo.Id.ToString());

        return new OkObjectResult(newRow.Value.ToTodo());
    }
}