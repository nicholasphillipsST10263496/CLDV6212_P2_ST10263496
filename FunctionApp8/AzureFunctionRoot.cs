using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp8
{
    public class AzureFunctionRoot
    {
        // Logger instance for logging information and errors
        private readonly ILogger<AzureFunctionRoot> _logger;

        // Constructor to initialize the logger
        public AzureFunctionRoot(ILogger<AzureFunctionRoot> logger)
        {
            _logger = logger; // Assign the logger to the private field
        }

        // Function attribute to define this method as an Azure Function
        [Function("AzureFunctionRoot")]
        public IActionResult Run(
            // HttpTrigger attribute to specify that this function responds to HTTP requests
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Log information when the function is triggered
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Return a successful response with a welcome message
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}