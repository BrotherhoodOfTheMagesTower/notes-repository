using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class FolderValidation : Validation
    {
        public FolderValidation(string title, string directory) : base(title, directory)
        {
            Title = title;
            Directory = directory;
        }
    }
}
