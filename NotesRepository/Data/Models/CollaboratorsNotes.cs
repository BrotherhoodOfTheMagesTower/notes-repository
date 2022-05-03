using NotesRepository.Areas.Identity.Data;

namespace NotesRepository.Data.Models
{
    public class CollaboratorsNotes
    {
        public CollaboratorsNotes() { }

        public CollaboratorsNotes(ApplicationUser collaborator, Note sharedNote)
        {
            Collaborator = collaborator;
            SharedNote = sharedNote;
            ApplicationUserId = collaborator.Id;
            NoteId = sharedNote.NoteId;
        }
        public string ApplicationUserId { get; set; }

        public Guid NoteId { get; set; }

        public ApplicationUser Collaborator { get; set; }

        public Note SharedNote { get; set; }
    }
}
