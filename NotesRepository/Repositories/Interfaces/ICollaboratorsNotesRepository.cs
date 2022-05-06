using NotesRepository.Data.Models;
using NotesRepository.Areas.Identity.Data;

namespace NotesRepository.Repositories.Interfaces
{
    public interface ICollaboratorsNotesRepository
    {
        Task<bool> AddCollaboratorToNoteAsync(CollaboratorsNotes collaborator);
        Task<bool> AddCollaboratorsToNoteAsync(ICollection<CollaboratorsNotes> collaborators);
        Task<bool> DeleteCollaboratorFromNoteAsync(Guid noteId, string appUserId);
        Task<bool> DeleteCollaboratorFromNoteAsync(CollaboratorsNotes collaborator);
        Task<bool> DeleteCollaboratorsFromNoteAsync(ICollection<CollaboratorsNotes> collaborators);
        Task<ICollection<Note>> GetAllSharedNotesForUser(string appUserId);
        Task<ICollection<ApplicationUser>> GetAllCollaboratorsForNote(Guid noteId);
    }
}
