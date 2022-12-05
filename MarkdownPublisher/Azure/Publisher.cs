using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using MarkdownPublisher.Abstract;

namespace MarkdownPublisher.Azure
{
    internal class Publisher : PublisherBase<Options>
    {
        public Publisher(string sourcePath, Dictionary<string, string> config) : base(sourcePath, config)
        {
        }

        protected override Options BuildConfiguration(Dictionary<string, string> config) => new Options()
        {
            ConnectionString = config["ConnectionString"],
            ContainerName = config["ContainerName"],
            BlobPrefix = config.TryGetValue("BlobPrefix", out string? blobPrefix) ? blobPrefix : default!,
            HostRoutePrefix = config.TryGetValue("HostRoutePrefix", out string? routePrefix) ? routePrefix : default!
        };
        
        protected override async Task PublishFileAsync(string localFile, string targetFile)
        {
            var containerClient = new BlobContainerClient(Config.ConnectionString, Config.ContainerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = new BlockBlobClient(Config.ConnectionString, Config.ContainerName, targetFile);
            using var stream = File.OpenRead(localFile);
            await blobClient.UploadAsync(stream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = "text/html"
                }
            });
        }
    }
}
