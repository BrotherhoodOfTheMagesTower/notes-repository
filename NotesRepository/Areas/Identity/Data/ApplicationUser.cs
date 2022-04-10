using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NotesRepository.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [MinLength(3, ErrorMessage = "The first name must be at least 3 characters long.")]
        [MaxLength(30, ErrorMessage = "The first name can be maximum 30 characters long.")]
        public string? FirstName { get; set; }

        [MinLength(3, ErrorMessage = "The last name must be at least 3 characters long.")]
        [MaxLength(30, ErrorMessage = "The last name can be maximum 30 characters long.")]
        public string? LastName { get; set; }

    }
}
