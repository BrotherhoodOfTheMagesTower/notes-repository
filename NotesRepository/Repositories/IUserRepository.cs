using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;

namespace NotesRepository.Repositories
{
    public interface IUserRepository
    {
        Task<ICollection<ApplicationUser>> GetAllUsersAsync();
        Task<ICollection<ApplicationUser>> GetUsersByUserNameAsync(string userName);
        Task<ICollection<ApplicationUser>> GetUsersByFirstAndLastNameAsync(string firstName, string lastName);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<bool> AddUserAsync(ApplicationUser user); // TODO za to jest odpowiedzialne microsoft identity
        Task<bool> DeleteUserByIdAsync(string userId); // TODO za to jest odpowiedzialne microsoft identity
    }
}