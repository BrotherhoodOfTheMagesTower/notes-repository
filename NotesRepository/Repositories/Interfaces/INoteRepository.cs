using NotesRepository.Data.Models;

namespace NotesRepository.Repositories.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<ICollection<Note>> GetAllUserNotesAsync(string userId);
        Task<Note?> GetNoteByTitleAsync(string title);
        Task<ICollection<Note>> GetAllNotesForParticularDirectoryAsync(Guid directoryId);
        Task<bool> AddManyAsync(ICollection<Note> notes);
        Task<bool> DeleteManyAsync(ICollection<Note> notes);
        Task<List<Note>> SearchNoteByTitleAndContentAsync(string searchText);
        Task<bool> SetNoteAsCurrentlyEditedAsync(Guid noteId);
        Task<bool> SetNoteAsCurrentlyNotEditedAsync(Guid noteId);
    }
}
