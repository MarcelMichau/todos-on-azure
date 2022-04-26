using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Todos.Domain;

namespace Todos.API;

internal class CreateTodo
{
    private const string TableName = "todos";

    private readonly ILogger<CreateTodo> _logger;

    public CreateTodo(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName("CreateTodo")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "CreateTodo" })]
    [OpenApiParameter(name: "todoText", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **TodoText** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Todo), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todos")] HttpRequest request,
        [Table(TableName, Connection = "AzureWebJobsStorage")] IAsyncCollector<TodoTableEntity> todoTable)
    {
        _logger.LogInformation("Creating a new Todo");

        string todoText = request.Query["todoText"];

        var newTodo = new Todo(todoText);

        await todoTable.AddAsync(newTodo.ToTableEntity());

        return new OkObjectResult(newTodo);
    }
}