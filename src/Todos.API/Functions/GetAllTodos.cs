using System.Collections.Generic;
using System.Linq;
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
using Todos.Domain;

namespace Todos.API.Functions;
internal class GetAllTodos
{
    private readonly ILogger<CreateTodo> _logger;

    public GetAllTodos(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName(nameof(GetAllTodos))]
    [OpenApiOperation(operationId: nameof(GetAllTodos), tags: new[] { nameof(GetAllTodos) })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(List<Todo>), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, nameof(HttpMethod.Get), Route = "todos")] HttpRequest request,
        [Table(Constants.TableName, Constants.PartitionKey, Connection = Constants.TableConnectionKey)] TableClient todoTable)
    {
        _logger.LogInformation("Getting all todos");
        var todos = await todoTable.QueryAsync<TodoTableEntity>().AsPages().FirstAsync();

        return new OkObjectResult(todos.Values.Select(Mappings.ToTodo));
    }
}
