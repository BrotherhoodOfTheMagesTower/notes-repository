using NotesRepository.Data.Models;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Repositories
{
    public interface IDirectoryRepository
    {

        Task<ICollection<Directory>> GetAllDirectoriesAsync();
        Task<Directory?> GetDirectoryByIdAsync(Guid directoryId);
        Task<Directory?> GetDirectoryByNameAsync(string name);
        Task<Directory?> GetDefaultDirectoryForParticularUserAsync(string userId);
        Task<ICollection<Directory>> GetAllDirectoriesForParticularUserAsync(string userId);
        Task<ICollection<Directory>?> GetAllSubDirectoriesOfParticularDirectory(Guid directoryId);
        Task<bool> AddDirectoryAsync(Directory directory);
        Task<bool> AddSubDirectoryAsync(Directory subDirectory);
        Task<bool> AttachSubDirectoryToParticularDirectoryAsync(Directory subDirectory, Guid directoryId);
        Task<bool> UpdateDirectoryAsync(Directory directory);
        Task<bool> UpdateSubDirectoryAsync(Directory subDirectory);
        Task<bool> DeleteDirectoryAsync(Directory directory);
        Task<bool> DeleteDirectoriesAsync(ICollection<Directory> directories);
        Task<bool> DeleteDirectoryByIdAsync(Guid directoryId);
        Task<bool> DeleteSubDirectoryByIdForParticularDirectoryAsync(Guid subDirectoryId, Guid directoryId);
        Task<bool> DeleteAllSubDirectoriesForParticularDirectoryAsync(Guid directoryId);

    }
}


    
