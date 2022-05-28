using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class NoteValidation
    {
        public NoteValidation(string title, string emoji, string directory)
        {
            Title = title;
            Directory = directory;
            Emoji = emoji;
        }

        [Required(ErrorMessage = "Required field!")]
        [StringLength(32, ErrorMessage = "Too long!")]
        [MinLength(2, ErrorMessage = "Too short!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Required field!")]
        [StringLength(32, ErrorMessage = "Too long!")]
        [MinLength(2, ErrorMessage = "Too short!")]
        public string Directory { get; set; }

        [Required(ErrorMessage = "Required field!")]
        public string Emoji { get; set; }
    }
}
