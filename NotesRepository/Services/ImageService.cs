using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;

namespace NotesRepository.Services
{
    public class ImageService
    {
        private readonly ImageRepository _ir;
        private readonly string containerName = "imagecontainer";
        private AzureStorageHelper _azureHelper;

        public ImageService(ImageRepository imageRepository)
        {
            _ir = imageRepository;
        }
        
        public ImageService(ImageRepository imageRepository, AzureStorageHelper azureStorageHelper)
        {
            _ir = imageRepository;
            _azureHelper = azureStorageHelper;
        }

        public async Task<ICollection<Image>> GetAllNoteImagesAsync(Guid noteId)
            => await _ir.GetAllNoteImagesAsync(noteId);
        
        public async Task<ICollection<Image>> GetAllUserImagesAsync(string userId)
            => await _ir.GetAllUserImagesAsync(userId);
        
        public async Task<Image?> GetImageByIdAsync(Guid imageId)
            => await _ir.GetByIdAsync(imageId);
        
        public async Task<Image?> GetImageByUrlAsync(string imageUrl)
            => await _ir.GetImageByUrlAsync(imageUrl);

        public async Task<bool> AddImageAsync(Image image, IFormFile file)
        {
            var url = await _azureHelper.UploadFileToAzureAsync(file, containerName, image.Name);
            if (!string.IsNullOrEmpty(url))
            {
                image.FileUrl = url;
                return await _ir.AddAsync(image);
            }
            return false;
        }
        
        /// <summary>
        /// Should be used only for unit testing
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<bool> AddImageWithoutAzureUploadAsync(Image image)
            => await _ir.AddAsync(image);

        public async Task<bool> AddImagesAsync(ICollection<Tuple<Image, IFormFile>> images)
        {
            foreach(var image in images)
            {
                var url = await _azureHelper.UploadFileToAzureAsync(image.Item2, containerName, image.Item1.Name);
                if (!string.IsNullOrEmpty(url))
                {
                    image.Item1.FileUrl = url;
                }
                else
                    return false;
            }
            return await _ir.AddManyAsync(images.Select(x => x.Item1).ToList());
        } 
        
        /// <summary>
        /// Should be used only for unit testing
        /// </summary>
        /// <param name="images"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<bool> AddImagesWithoutAzureUploadAsync(ICollection<Image> images)
            => await _ir.AddManyAsync(images);
        
        public async Task<bool> DeleteImageAsync(Image image)
        {
            await _azureHelper.DeleteImageFromAzure(image.Name, containerName);
            return await _ir.DeleteAsync(image);
        }

        /// <summary>
        /// Should be used only in unit testing
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImageWithoutAzureAsync(Image image)
            => await _ir.DeleteAsync(image);

        public async Task<bool> DeleteImageByIdAsync(Guid imageId)
        {
            var image = await _ir.GetByIdAsync(imageId);
            if(image is not null)
            {
                await _azureHelper.DeleteImageFromAzure(image.Name, containerName);
                return await _ir.DeleteByIdAsync(imageId);
            }
            return false;
        }

        /// <summary>
        /// Should be used only in unit testing
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImageByIdWithoutAzureAsync(Guid imageId)
            => await _ir.DeleteByIdAsync(imageId);

        public async Task<bool> DeleteImagesAsync(ICollection<Tuple<Image, IFormFile>> images)
        {
            foreach (var image in images)
                await _azureHelper.DeleteImageFromAzure(image.Item1.Name, containerName);

            return await _ir.DeleteManyAsync(images.Select(x => x.Item1).ToList());
        }
        
        /// <summary>
        /// Should be used only in unit testing
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImagesWithoutAzureAsync(ICollection<Image> images)
            => await _ir.DeleteManyAsync(images);
    }
}
