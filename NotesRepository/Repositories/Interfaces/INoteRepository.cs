using NotesRepository.Data.Models;

namespace NotesRepository.Repositories.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<ICollection<Note>> GetAllUserNotesAsync(string userId);
        Task<ICollection<Note>> GetAllUserNotesWithoutEventAsync(string userId);
        Task<Note?> GetNoteByTitleAsync(string title, string userId);
        Task<ICollection<Note>> GetAllNotesForParticularDirectoryAsync(Guid directoryId);
        Task<bool> AddManyAsync(ICollection<Note> notes);
        Task<bool> DeleteManyAsync(ICollection<Note> notes);
        Task<List<Note>> SearchNoteByTitleAndContentAsync(string searchText, string userId);
        Task<bool> MarkNoteAsCurrentlyEditedAsync(Guid noteId);
        Task<bool> MarkNoteAsCurrentlyNotEditedAsync(Guid noteId);
        ICollection<Note> GetAllNotesForParticularDirectory(Guid directoryId);
        
    }
}
