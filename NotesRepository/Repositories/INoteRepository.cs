using NotesRepository.Data.Models;

namespace NotesRepository.Repositories
{
    public interface INoteRepository
    {
        Task<ICollection<Note>> GetAllNotesAsync();
        Task<Note?> GetNoteByIdAsync(Guid noteId);
        Task<Note?> GetNoteByTitleAsync(string title);
        Task<bool> AddNoteAsync(Note note);
        Task<bool> AddNotesAsync(ICollection<Note> notes);
        Task<bool> UpdateNoteAsync(Note note);
        Task<bool> DeleteNoteAsync(Note note);
        Task<bool> DeleteNotesAsync(ICollection<Note> notes);
        Task<bool> DeleteNoteByIdAsync(Guid noteId);
    }
}
