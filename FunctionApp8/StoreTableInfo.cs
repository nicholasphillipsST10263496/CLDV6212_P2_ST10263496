using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionApp8
{
    public static class StoreTableInfo
    {
        // Azure Function that processes incoming HTTP requests to store data in an Azure Table
        [Function("StoreTableInfo")]
        public static async Task<IActionResult> Run(
            // HttpTrigger attribute to specify that this function responds to HTTP POST requests
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log) // Logger instance for logging information and errors
        {
            // Retrieve the table name, partition key, row key, and data from the query parameters of the HTTP request
            string tableName = req.Query["tableName"];
            string partitionKey = req.Query["partitionKey"];
            string rowKey = req.Query["rowKey"];
            string data = req.Query["data"];

            // Check if any of the required parameters are missing
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(partitionKey) ||
                string.IsNullOrEmpty(rowKey) || string.IsNullOrEmpty(data))
            {
                // Return a bad request response if validation fails
                return new BadRequestObjectResult("Table name, partition key, row key, and data must be provided.");
            }

            // Retrieve the Azure Storage connection string from environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");

            // Create a TableServiceClient to interact with the Azure Table service
            var serviceClient = new TableServiceClient(connectionString);

            // Get a TableClient for the specified table name
            var tableClient = serviceClient.GetTableClient(tableName);

            // Create the table if it does not already exist
            await tableClient.CreateIfNotExistsAsync();

            // Create a new TableEntity with the specified partition key and row key, and add the data
            var entity = new TableEntity(partitionKey, rowKey) { ["Data"] = data };

            // Add the entity to the table
            await tableClient.AddEntityAsync(entity);

            // Return a success response indicating the data has been added to the table
            return new OkObjectResult("Data added to table");
        }
    }
}