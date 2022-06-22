using Microsoft.AspNetCore.Components.Forms;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using NotesRepository.Services.Azure;

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

        /// <summary>
        /// Gets all images attached to given note
        /// </summary>
        /// <param name="noteId">ID of the note</param>
        /// <returns>A collection with images, that are attached to note</returns>
        public async Task<ICollection<Image>> GetAllNoteImagesAsync(Guid noteId)
            => await _ir.GetAllNoteImagesAsync(noteId);
        
        /// <summary>
        /// Gets all images for particular user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with images for particular user</returns>
        public async Task<ICollection<Image>> GetAllUserImagesAsync(string userId)
            => await _ir.GetAllUserImagesAsync(userId);
        
        /// <summary>
        /// Gets image entity by ID
        /// </summary>
        /// <param name="imageId">ID of the image</param>
        /// <returns>The image entity</returns>
        public async Task<Image?> GetImageByIdAsync(Guid imageId)
            => await _ir.GetByIdAsync(imageId);
        
        /// <summary>
        /// Gets the image entity by URL
        /// </summary>
        /// <param name="imageUrl">URL of the image</param>
        /// <returns>The image entity</returns>
        public async Task<Image?> GetImageByUrlAsync(string imageUrl)
            => await _ir.GetImageByUrlAsync(imageUrl);

        /// <summary>
        /// Adds image entity to the database
        /// </summary>
        /// <param name="image">The image entity</param>
        /// <param name="file">The file interface</param>
        /// <returns>Tuple - (was successfully added, file url)</returns>
        public async Task<(bool, string)> AddImageAsync(Image image, IBrowserFile file)
        {
            var url = await _azureHelper.UploadFileToAzureAsync(file, containerName, image.Name);
            if (!string.IsNullOrEmpty(url))
            {
                image.FileUrl = url;
                var result = await _ir.AddAsync(image);
                return (result, url);
            }
            return (false,"");
        }
        
        /// <summary>
        /// Should be used only for unit testing
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<bool> AddImageWithoutAzureUploadAsync(Image image)
            => await _ir.AddAsync(image);
        
        /// <summary>
        /// Should be used only for unit testing
        /// </summary>
        /// <param name="images"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<bool> AddImagesWithoutAzureUploadAsync(ICollection<Image> images)
            => await _ir.AddManyAsync(images);
        
        /// <summary>
        /// Deletes an image entity from the database
        /// </summary>
        /// <param name="image">The image entity</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
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
        
        /// <summary>
        /// Should be used only in unit testing
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImagesWithoutAzureAsync(ICollection<Image> images)
            => await _ir.DeleteManyAsync(images);
    }
}
