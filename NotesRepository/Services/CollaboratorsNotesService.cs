using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;

namespace NotesRepository.Services
{
    public class CollaboratorsNotesService
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly CollaboratorsNotesRepository _cnr;

        public CollaboratorsNotesService(CollaboratorsNotesRepository collaboratorsNotesRepository, NoteRepository noteRepository, UserRepository userRepository)
        {
            _cnr = collaboratorsNotesRepository;
            _nr = noteRepository;
            _ur = userRepository;
        }

        public async Task<ICollection<ApplicationUser>> GetAllCollaboratorsForNoteAsync(Guid noteId)
            => await _cnr.GetAllCollaboratorsForNote(noteId);

        public async Task<ICollection<Note>> GetAllSharedNotesForUserAsync(string userId)
            => await _cnr.GetAllSharedNotesForUser(userId);

        public async Task<bool> AddCollaboratorToNoteAsync(string userId, Guid noteId)
        {
            var user = await _ur.GetUserByIdAsync(userId);
            var note = await _nr.GetByIdAsync(noteId);
            if (user is not null && note is not null)
            {
                return await _cnr.AddCollaboratorToNoteAsync(new CollaboratorsNotes(user, note));
            }
            return false;
        }
        
        public async Task<bool> AddCollaboratorToNoteAsync(CollaboratorsNotes collaboratorNote)
            => await _cnr.AddCollaboratorToNoteAsync(collaboratorNote);
        
        public async Task<bool> AddCollaboratorsToNoteAsync(ICollection<string> userIds, Guid noteId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if(note is not null)
            {
                var list = new List<CollaboratorsNotes>();
                foreach (var item in userIds)
                {
                    var user = await _ur.GetUserByIdAsync(item);
                    if (user is not null)
                    {
                        list.Add(new CollaboratorsNotes(user, note));
                    }
                    else
                        return false;
                }
                return await _cnr.AddCollaboratorsToNoteAsync(list);
            }
            return false;
        }

        public async Task<bool> AddCollaboratorsToNoteAsync(ICollection<CollaboratorsNotes> collaboratorsNotes)
            => await _cnr.AddCollaboratorsToNoteAsync(collaboratorsNotes);

        public async Task<bool> DeleteCollaboratorFromNoteAsync(string userId, Guid noteId)
            => await _cnr.DeleteCollaboratorFromNoteAsync(noteId, userId);
        
        public async Task<bool> DeleteCollaboratorFromNoteAsync(CollaboratorsNotes collaboratorNote)
            => await _cnr.DeleteCollaboratorFromNoteAsync(collaboratorNote);
        
        public async Task<bool> DeleteCollaboratorsFromNoteAsync(ICollection<CollaboratorsNotes> collaboratorsNotes)
            => await _cnr.DeleteCollaboratorsFromNoteAsync(collaboratorsNotes);

        public async Task<bool> DeleteCollaboratorsFromNoteAsync(ICollection<string> userIds, Guid noteId)
        {
            var note = await _nr.GetByIdAsync(noteId);
            if (note is not null)
            {
                var list = new List<CollaboratorsNotes>();
                foreach (var item in userIds)
                {
                    var user = await _ur.GetUserByIdAsync(item);
                    if (user is not null)
                    {
                        list.Add(new CollaboratorsNotes(user, note));
                    }
                    else
                        return false;
                }
                return await _cnr.DeleteCollaboratorsFromNoteAsync(list);
            }
            return false;
        }
    }
}
