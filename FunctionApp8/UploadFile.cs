using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp8
{
    public static class UploadFile
    {
        // Azure Function that processes incoming HTTP requests to upload files to an Azure File Share
        [Function("UploadFile")]
        public static async Task<IActionResult> Run(
            // HttpTrigger attribute to specify that this function responds to HTTP POST requests
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log) // Logger instance for logging information and errors
        {
            // Retrieve the share name and file name from the query parameters of the HTTP request
            string shareName = req.Query["shareName"];
            string fileName = req.Query["fileName"];

            // Check if either the share name or file name is not provided
            if (string.IsNullOrEmpty(shareName) || string.IsNullOrEmpty(fileName))
            {
                // Return a bad request response if validation fails
                return new BadRequestObjectResult("Share name and file name must be provided.");
            }

            // Retrieve the Azure Storage connection string from environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");

            // Create a ShareServiceClient to interact with the Azure File Share service
            var shareServiceClient = new ShareServiceClient(connectionString);

            // Get a ShareClient for the specified share name
            var shareClient = shareServiceClient.GetShareClient(shareName);

            // Create the share if it does not already exist
            await shareClient.CreateIfNotExistsAsync();

            // Get the root directory client of the share
            var directoryClient = shareClient.GetRootDirectoryClient();

            // Get a FileClient for the specified file name within the root directory
            var fileClient = directoryClient.GetFileClient(fileName);

            // Use the request body stream to upload the file
            using var stream = req.Body;

            // Create the file in Azure Files with the specified length
            await fileClient.CreateAsync(stream.Length);

            // Upload the file from the stream to Azure Files
            await fileClient.UploadAsync(stream);

            // Return a success response indicating the file has been uploaded
            return new OkObjectResult("File uploaded to Azure Files");
        }
    }
}