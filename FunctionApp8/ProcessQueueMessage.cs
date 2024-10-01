using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionApp8
{
    public static class ProcessQueueMessage
    {
        // Azure Function that processes incoming HTTP requests to add messages to a storage queue
        [Function("ProcessQueueMessage")]
        public static async Task<IActionResult> Run(
            // HttpTrigger attribute to specify that this function responds to HTTP POST requests
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log) // Logger instance for logging information and errors
        {
            // Retrieve the queue name and message from the query parameters of the HTTP request
            string queueName = req.Query["queueName"];
            string message = req.Query["message"];

            // Check if either queue name or message is not provided
            if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
            {
                // Return a bad request response if validation fails
                return new BadRequestObjectResult("Queue name and message must be provided.");
            }

            // Retrieve the Azure Storage connection string from environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");

            // Create a QueueServiceClient to interact with the Azure Queue service
            var queueServiceClient = new QueueServiceClient(connectionString);

            // Get a QueueClient for the specified queue name
            var queueClient = queueServiceClient.GetQueueClient(queueName);

            // Create the queue if it does not already exist
            await queueClient.CreateIfNotExistsAsync();

            // Send the message to the specified queue
            await queueClient.SendMessageAsync(message);

            // Return a success response indicating the message has been added to the queue
            return new OkObjectResult("Message added to queue");
        }
    }
}