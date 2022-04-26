using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Todos.Domain;

namespace Todos.API.Functions;
internal class GetTodo
{
    private readonly ILogger<CreateTodo> _logger;

    public GetTodo(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName(nameof(GetTodo))]
    [OpenApiOperation(operationId: nameof(GetTodo), tags: new[] { nameof(GetTodo) })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** route parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Todo), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, nameof(HttpMethod.Get), Route = "todos/{id}")] HttpRequest request,
        [Table(Constants.TableName, Constants.PartitionKey, "{id}", Connection = Constants.TableConnectionKey)] TodoTableEntity todo, string id)
    {
        _logger.LogInformation("Getting todo with ID: {id}", id);

        if (todo != null) return new OkObjectResult(todo.ToTodo());
        
        _logger.LogInformation($"Todo with ID: {id} not found");
        return new NotFoundResult();
    }
}
