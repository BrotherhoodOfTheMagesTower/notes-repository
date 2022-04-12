using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class NoteDirectory
    {
        /// <summary>
        /// Unique ID of the directory
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the directory
        /// </summary>
        [MaxLength(30, ErrorMessage = "Name of directory may not contain more than 30 characters.")]
        [MinLength(3, ErrorMessage = "Name of directory must be at least 3 characters long.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Collection of Notes assigned to the directory (optional)
        /// </summary>
        public ICollection<Note>? Notes { get; set; }
    }
}
