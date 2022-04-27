using NotesRepository.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class Directory
    {
        public Directory() { }

        public Directory(string name, ApplicationUser user, Guid? directoryId = null, ICollection<Note>? notes = null)
        {
            DirectoryId = directoryId ?? Guid.NewGuid();
            Name = name;
            Notes = notes;
            User= user;
        }

        /// <summary>
        /// Unique ID of the directory
        /// </summary>
        public Guid DirectoryId { get; set; }

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
        
        /// <summary>
        /// User, that the directory belongs to
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Collection of subdirectiories (optional)
        /// </summary>
        public ICollection<Directory>? SubDirectories { get; set; }
    }
}
