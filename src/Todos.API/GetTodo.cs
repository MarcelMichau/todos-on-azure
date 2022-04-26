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
internal class GetTodo
{
    private const string TableName = "todos";

    private readonly ILogger<CreateTodo> _logger;

    public GetTodo(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName("GetTodo")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "GetTodo" })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** route parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Todo), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos/{id}")] HttpRequest request,
        [Table(TableName, "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoTableEntity todo, string id)
    {
        _logger.LogInformation("Getting todo by ID");
        
        if (todo != null) return new OkObjectResult(todo.ToTodo());
        
        _logger.LogInformation($"Item {id} not found");
        return new NotFoundResult();
    }
}
