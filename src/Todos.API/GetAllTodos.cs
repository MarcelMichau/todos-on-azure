﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Todos.Domain;

namespace Todos.API;
internal class GetAllTodos
{
    private const string TableName = "todos";

    private readonly ILogger<CreateTodo> _logger;

    public GetAllTodos(ILogger<CreateTodo> log)
    {
        _logger = log;
    }

    [FunctionName("GetAllTodos")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "GetAllTodos" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Todo>), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest request,
        [Table(TableName, "TODO", Connection = "AzureWebJobsStorage")] TableClient todoTable)
    {
        _logger.LogInformation("Getting all todos");
        var todos = await todoTable.QueryAsync<TodoTableEntity>().AsPages().FirstAsync();

        return new OkObjectResult(todos.Values.Select(Mappings.ToTodo));
    }
}