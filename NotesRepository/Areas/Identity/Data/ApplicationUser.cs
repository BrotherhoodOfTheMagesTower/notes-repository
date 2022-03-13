using Microsoft.AspNetCore.Identity;

namespace NotesRepository.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }
}
