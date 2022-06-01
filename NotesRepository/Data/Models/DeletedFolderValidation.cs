using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class DeletedFolderValidation
    {
        public DeletedFolderValidation(string directory)
        {
            Directory = directory;
        }

        [Required(ErrorMessage = "Required field!")]
        [StringLength(32, ErrorMessage = "Too long!")]
        [MinLength(2, ErrorMessage = "Too short!")]
        public string Directory { get; set; }
    }
}
