using NotesRepository.Data.Models;
using NotesRepository.Repositories;

namespace NotesRepository.Services
{
    //TODO: add Azure Blob Storage handling
    public class ImageService
    {
        private readonly ImageRepository _ir;

        public ImageService(ImageRepository imageRepository)
        {
            _ir = imageRepository;
        }

        public async Task<ICollection<Image>> GetAllNoteImagesAsync(Guid noteId)
            => await _ir.GetAllNoteImagesAsync(noteId);
        
        public async Task<ICollection<Image>> GetAllUserImagesAsync(string userId)
            => await _ir.GetAllUserImagesAsync(userId);
        
        public async Task<Image?> GetImageByIdAsync(Guid imageId)
            => await _ir.GetByIdAsync(imageId);
        
        public async Task<Image?> GetImageByUrlAsync(string imageUrl)
            => await _ir.GetImageByUrlAsync(imageUrl);

        public async Task<bool> AddImageAsync(Image image)
            => await _ir.AddAsync(image);

        public async Task<bool> AddImagesAsync(ICollection<Image> images)
            => await _ir.AddManyAsync(images);

        public async Task<bool> UpdateImageAsync(Image image)
            => await _ir.UpdateAsync(image);

        public async Task<bool> DeleteImageAsync(Image image)
            => await _ir.DeleteAsync(image);

        public async Task<bool> DeleteImageByIdAsync(Guid imageId)
            => await _ir.DeleteByIdAsync(imageId);

        public async Task<bool> DeleteImagesAsync(ICollection<Image> images)
            => await _ir.DeleteManyAsync(images);

        public async Task<bool> SetImageUrl(string imageUrl, Guid imageId)
        {
            var image = await _ir.GetByIdAsync(imageId);
            if (image is not null)
            {
                image.FileUrl = imageUrl;
                return await _ir.UpdateAsync(image);
            }
            return false;
        }
        
        public async Task<bool> SetImageName(string name, Guid imageId)
        {
            var image = await _ir.GetByIdAsync(imageId);
            if (image is not null)
            {
                image.Name = name;
                return await _ir.UpdateAsync(image);
            }
            return false;
        }
    }
}
