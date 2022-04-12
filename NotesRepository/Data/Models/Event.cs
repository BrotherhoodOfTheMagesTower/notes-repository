using NotesRepository.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Data.Models
{
    public class Event
    {
        public Event(Guid? eventId, string content, DateTime startAt, DateTime endAt, ApplicationUser user, DateTime? remainderAt = null, Note? note = null)
        {
            EventId = eventId ?? Guid.NewGuid();
            Content = content;
            RemainderAt = remainderAt;
            StartAt = startAt;
            EndAt = endAt;
            User = user;
            Note = note;
        }

        /// <summary>
        /// Unique ID of the event
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Content of the event
        /// </summary>
        [MaxLength(100, ErrorMessage = "Content may have maximum 100 characters")]
        [MinLength(3, ErrorMessage = "Content must have at least 3 characters")]
        public string Content { get; set; }

        /// <summary>
        /// Date and time of the remainder
        /// </summary>
        public DateTime? RemainderAt { get; set; }

        /// <summary>
        /// Date and time when the event will start
        /// </summary>
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Date and time when the event will end 
        /// </summary>
        public DateTime EndAt { get; set; }

        /// <summary>
        /// User, which created the event
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Note, which is assigned to the event
        /// </summary>
        public Note? Note { get; set; }
    }
}
