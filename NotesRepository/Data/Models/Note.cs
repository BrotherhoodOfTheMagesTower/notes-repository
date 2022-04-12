using NotesRepository.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class Note
    {
        /// <summary>
        /// Unique ID of the note
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the note (amount of characters between 3 and 50)
        /// </summary>
        [MaxLength(50, ErrorMessage = "Title may not contain more than 50 characters.")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content of the note (amount of characters between 3 and 7999)
        /// </summary>
        [MaxLength(8000, ErrorMessage = "Content may not contain more than 8000 characters.")]
        [MinLength(3, ErrorMessage = "Content must be at least 3 characters long.")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Icon name (from for ex. Bootstrap)
        /// </summary>
        public string IconName { get; set; } = "default"; //todo: select a default

        /// <summary>
        /// Date and time when the note was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Date and time when the note was edited
        /// </summary>
        public DateTime? EditedAt { get; set; }
        
        /// <summary>
        /// Last user, which edited the note
        /// </summary>
        public ApplicationUser? EditedBy { get; set; }

        /// <summary>
        /// User which created the note
        /// </summary>
        public ApplicationUser Owner { get; set; }

        /// <summary>
        /// Users, which were added as collaborators to the note
        /// </summary>
        public ICollection<ApplicationUser>? Collaborators { get; set; }

        /// <summary>
        /// Directory assigned to the note
        /// </summary>
        public NoteDirectory Directory { get; set; }

        /// <summary>
        /// Collection of Images assigned to the note (optional)
        /// </summary>
        public ICollection<Image>? Images { get; set; }

        /// <summary>
        /// Value that determines if the note is being currently edited
        /// </summary>
        public bool IsCurrentlyEdited { get; set; } = false;

        /// <summary>
        /// Event, to which the note is assigned (optional)
        /// </summary>
        public Event? Event { get; set; }
    }
}
