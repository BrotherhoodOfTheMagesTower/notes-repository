using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class Validation
    {
        public Validation(string title, string directory)
        {
            Title = title;
            Directory = directory;
        }
        public Validation(string directory)
        {
            Directory = directory;
        }

        [Required(ErrorMessage = "Required field!")]
        [StringLength(32, ErrorMessage = "Too long!")]
        [MinLength(2, ErrorMessage = "Too short!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Required field!")]
        [StringLength(32, ErrorMessage = "Too long!")]
        [MinLength(2, ErrorMessage = "Too short!")]
        public string Directory { get; set; }
    }
}
