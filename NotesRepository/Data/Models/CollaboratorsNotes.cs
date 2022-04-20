using NotesRepository.Areas.Identity.Data;

namespace NotesRepository.Data.Models
{
    public class CollaboratorsNotes
    {
        public CollaboratorsNotes() { }

        public CollaboratorsNotes(string appUserId, Guid? noteId, ApplicationUser collaborator, Note sharedNote)
        {
            ApplicationUserId = appUserId;
            NoteId = noteId ?? Guid.NewGuid();
            Collaborator = collaborator;
            SharedNote = sharedNote;
        }
        public string ApplicationUserId { get; set; }

        public Guid NoteId { get; set; }

        public ApplicationUser Collaborator { get; set; }

        public Note SharedNote { get; set; }
    }
}
