using NotesRepository.Data.Models;

namespace NotesRepository.Repositories.Interfaces
{
    public interface IImageRepository : IRepository<Image>
    {
        Task<bool> AddManyAsync(ICollection<Image> images);
        Task<bool> DeleteManyAsync(ICollection<Image> images);
        Task<Image?> GetImageByUrlAsync(string imageUrl);
        Task<ICollection<Image>> GetAllUserImagesAsync(string userId);
        Task<ICollection<Image>> GetAllNoteImagesAsync(Guid noteId);
    }
}