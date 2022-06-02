using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class DeletedNoteValidation : Validation
    {
        public DeletedNoteValidation(string directory)   
        {
            Directory = directory;
        }
    }
}
