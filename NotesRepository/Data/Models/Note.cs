using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Constants;
using NotesRepository.Repositories;
using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class Note
    {
        public Note() { }

        public Note(Guid? noteId, string title, string content, string iconName, ApplicationUser owner, Directory directory)
        {
            NoteId = noteId ?? Guid.NewGuid();
            Title = title;
            Content = content;
            IconName = iconName;
            CreatedAt = DateTime.Now;
            Owner = owner;
            Directory = directory;
        }

        public Note(Guid? noteId, string title, string content, ApplicationUser owner, Directory directory)
        {
            NoteId = noteId ?? Guid.NewGuid();
            Title = title;
            Content = content;
            CreatedAt = DateTime.Now;
            Owner = owner;
            Directory = directory;
        }

        public Note(string content)
        {
            NoteId = new Guid();
            Content = content;
        }

        /// <summary>
        /// Unique ID of the note
        /// </summary>
        public Guid NoteId { get; set; }

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
        public string IconName { get; set; } = Emoji.getRandomEmoji();

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
        public ICollection<CollaboratorsNotes>? CollaboratorsNotes { get; set; }

        /// <summary>
        /// Directory assigned to the note
        /// </summary>
        public Directory Directory { get; set; }

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

        /// <summary>
        /// Value that determines if the note was marked as deleted
        /// </summary>
        public bool IsMarkedAsDeleted { get; set; } = false;

        /// <summary>
        /// Date and time when the note was marked as deleted (optional)
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Determines, wheter the note is pinned or not
        /// </summary>
        public bool IsPinned { get; set; } = false;
    }
}
