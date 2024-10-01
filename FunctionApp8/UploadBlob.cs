using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp8
{
    public static class UploadBlob
    {
        // Azure Function that processes incoming HTTP requests to upload blobs to an Azure Blob Storage container
        [Function("UploadBlob")]
        public static async Task<IActionResult> Run(
            // HttpTrigger attribute to specify that this function responds to HTTP POST requests
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log) // Logger instance for logging information and errors
        {
            // Retrieve the container name and blob name from the query parameters of the HTTP request
            string containerName = req.Query["containerName"];
            string blobName = req.Query["blobName"];

            // Check if either the container name or blob name is not provided
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                // Return a bad request response if validation fails
                return new BadRequestObjectResult("Container name and blob name must be provided.");
            }

            // Retrieve the Azure Storage connection string from environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");

            // Create a BlobServiceClient to interact with the Azure Blob Storage service
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get a BlobContainerClient for the specified container name
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create the container if it does not already exist
            await containerClient.CreateIfNotExistsAsync();

            // Get a BlobClient for the specified blob name within the container
            var blobClient = containerClient.GetBlobClient(blobName);

            // Use the request body stream to upload the blob
            using var stream = req.Body;

            // Upload the blob from the stream, allowing overwrite if it already exists
            await blobClient.UploadAsync(stream, true);

            // Return a success response indicating the blob has been uploaded
            return new OkObjectResult("Blob uploaded");
        }
    }
}