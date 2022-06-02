using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class NoteValidation : Validation
    {
        public NoteValidation(string title, string emoji, string directory) : base(title, directory)
        {
            Title = title;
            Directory = directory;
            Emoji = emoji;
        }

        [Required(ErrorMessage = "Required field!")]
        public string Emoji { get; set; }
    }
}
