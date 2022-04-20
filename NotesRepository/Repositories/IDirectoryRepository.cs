using NotesRepository.Data.Models;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Repositories
{
    public interface IDirectoryRepository
    {

        Task<ICollection<Directory>> GetAllDirectoriesAsync();
        Task<Directory?> GetDirectoryByIdAsync(Guid directoryId);
        Task<Directory?> GetDirectoryByNameAsync(string name);
        Task<bool> AddDirectoryAsync(Directory directory);
        Task<bool> UpdateDirectoryAsync(Directory directory);
        Task<bool> DeleteDirectoryAsync(Directory directory);
        Task<bool> DeleteDirectoriesAsync(ICollection<Directory> directories);
        Task<bool> DeleteDirectoryByIdAsync(Guid directoryId);

    }
}


    
