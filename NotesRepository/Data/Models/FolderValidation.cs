using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class FolderValidation
    {
        public FolderValidation(string title)
        {
            Title = title;
        }

        [Required(ErrorMessage = "Required field!")]
        [StringLength(32, ErrorMessage = "Too long!")]
        [MinLength(2, ErrorMessage = "Too short!")]
        public string Title { get; set; }
    }
}
