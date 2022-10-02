using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using MyWishly.App.Models.Options;

namespace MyWishly.App.Services
{
    public class ImageService : IImageService
    {
        public Connections Connections { get; }
        public BlobServiceClient BlobServiceClient { get; }
        public BlobContainerClient ContainerClient { get; }

        public ImageService(IOptions<Connections> connections)
        {
            Connections = connections.Value;
            BlobServiceClient = new BlobServiceClient(Connections.Storage);
            ContainerClient = BlobServiceClient.GetBlobContainerClient(Connections.ContainerNameProductImages);
            ContainerClient.CreateIfNotExists();
        }

        public async Task<string> UploadProductImage(Guid userId, byte[] image, string extension)
        {
            var imageId = Guid.NewGuid();
            var blobClient = ContainerClient.GetBlobClient($"/{userId}/{imageId}{extension}");
            await blobClient.UploadAsync(new BinaryData(image));
            return blobClient.Uri.AbsoluteUri;
        }
    }
}
