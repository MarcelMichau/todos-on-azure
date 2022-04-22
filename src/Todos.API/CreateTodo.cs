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

namespace Todos.API
{
    public class CreateTodo
    {
        private readonly ILogger<CreateTodo> _logger;

        public CreateTodo(ILogger<CreateTodo> log)
        {
            _logger = log;
        }

        [FunctionName("CreateTodo")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "todoText", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **TodoText** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string todoText = req.Query["todoText"];

            var newTodo = new Todo(todoText);

            return new OkObjectResult(newTodo);
        }
    }
}

