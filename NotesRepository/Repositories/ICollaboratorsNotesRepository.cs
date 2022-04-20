using NotesRepository.Data.Models;
using NotesRepository.Areas.Identity.Data;

namespace NotesRepository.Repositories
{
    public interface ICollaboratorsNotesRepository
    {

        Task<bool> AddNoteToCollaboratorAsync(CollaboratorsNotes collaborator);
        Task<bool> AddNotesToCollaboratorAsync(ICollection<CollaboratorsNotes> collaborators);
        Task<bool> DeleteNoteFromCollaboratorAsync(Guid noteId, string appUserId);
        Task<bool> DeleteNoteFromCollaboratorAsync(CollaboratorsNotes collaborator);
        Task<bool> DeleteNotesFromCollaboratorAsync(ICollection<CollaboratorsNotes> collaborators);
        Task<ICollection<CollaboratorsNotes>> GetAllNotesCanBeEditedByUserAsync(string appUserId);
        Task<ICollection<CollaboratorsNotes>> GetAllUsersRelatedToTheNoteAsync(Guid noteId);
        


    }
}
