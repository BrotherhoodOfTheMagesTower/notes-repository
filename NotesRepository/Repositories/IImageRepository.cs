using NotesRepository.Data.Models;

namespace NotesRepository.Repositories
{
    public interface IImageRepository
    {
        Task<bool> AddImagesAsync(ICollection<Image> images);
        Task<bool> AddImageAsync(Image image);
        Task<bool> DeleteImageAsync(Image image);
        Task<bool> DeleteImageByIdAsync(Guid imageId);
        Task<bool> DeleteImagesAsync(ICollection<Image> images);
        Task<Image?> GetImageByIdAsync(Guid imageId);
        Task<Image?> GetImageByUrlAsync(string imageUrl);
        Task<ICollection<Image>> GetAllImagesAsync();
        Task<ICollection<Image>> GetAllUserImagesAsync(string userId);
        Task<ICollection<Image>> GetAllNoteImages(Note note);
        Task<bool> UpdateImageAsync(Image image);
    }
}