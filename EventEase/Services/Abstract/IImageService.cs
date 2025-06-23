namespace EventEase.Services.Abstract
{
    public interface IImageService
    {
        string UploadImageToAzure(IFormFile file);

        void DeleteImageFromAzure(string imageUrl);
    }
}
