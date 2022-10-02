namespace MyWishly.App.Services
{
    public interface IImageService
    {
        Task<string> UploadProductImage(Guid userId, byte[] image, string extension);
    }
}