using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class DeletedFolderValidation : Validation
    {
        public DeletedFolderValidation(string directory) :base(directory)
        {
            Directory = directory;
        }
    }
}
