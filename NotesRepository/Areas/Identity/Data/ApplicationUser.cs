using Microsoft.AspNetCore.Identity;
using NotesRepository.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// First name of user (optional)
        /// </summary>
        [MinLength(3, ErrorMessage = "The first name must be at least 3 characters long.")]
        [MaxLength(30, ErrorMessage = "The first name can be maximum 30 characters long.")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name of user (optional)
        /// </summary>
        [MinLength(3, ErrorMessage = "The last name must be at least 3 characters long.")]
        [MaxLength(30, ErrorMessage = "The last name can be maximum 30 characters long.")]
        public string? LastName { get; set; }

        /// <summary>
        /// Avatar image for user (optional)
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Collection of notes created by user
        /// </summary>
        public ICollection<Note>? Notes { get; set; }

        ///// <summary>
        ///// Collection of notes, that were shared with user
        ///// </summary>
        //public ICollection<Note>? SharedNotes { get; set; }

        /// <summary>
        /// Collection of events created by user
        /// </summary>
        public ICollection<Event>? Events { get; set; }
    }
}
