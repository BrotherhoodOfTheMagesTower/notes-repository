using NotesRepository.Data.Models;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Repositories.Interfaces
{
    public interface IDirectoryRepository : IRepository<Directory>
    {
        bool DeleteByIdSync(Guid directoryId);
        Task<bool> DeleteAllSubDirectoriesForParticularDirectoryAsync(Guid directoryId);
        Task<Directory?> GetDirectoryByNameAsync(string name, string userId);
        Task<ICollection<Directory>> GetAllDirectoriesForParticularUserAsync(string userId);
        Task<ICollection<Directory>> GetAllNotDeletedDirectoriesForParticularUserAsync(string userId);
        ICollection<Directory> GetAllSubDirectoriesOfParticularDirectorySync(Guid directoryId);
        Task<ICollection<Directory>?> GetAllSubDirectoriesOfParticularDirectoryAsync(Guid directoryId);
        Task<ICollection<Directory>?> GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(string userId);
        ICollection<Directory>? GetAllDirectoriesWithoutParentDirectoryForParticularUserSync(string userId);
        ICollection<Directory> GetMainDirectoriesWhichShouldBeRemovedFromDbSync(int daysOld);
        Task<bool> ChangeParentDirectoryForSubDirectoryAsync(Guid subDirectoryId, Guid directoryId);
        Task<bool> DeleteManyAsync(ICollection<Directory> directories);
        Task<bool> MarkDirectoryAsDeletedAsync(Guid directoryId);
        Task<bool> MarkDirectoryAsNotDeletedAsync(Guid directoryId);

    }
}


    
