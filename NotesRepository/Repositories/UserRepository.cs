using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories.Interfaces;

namespace NotesRepository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext ctx;

        public UserRepository(ApplicationDbContext context)
        {
            ctx = context;
        }

        public async Task<bool> AddUserAsync(ApplicationUser user)
        {
            await ctx.Users.AddAsync(user);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteUserByIdAsync(string userId)
        {
            var user = await ctx.Users.FirstOrDefaultAsync(x => x.Id == userId.ToString());
            if (user != null)
            {
                ctx.Users.Remove(user);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        public async Task<ICollection<ApplicationUser>> GetAllUsersAsync()
        {
            return await ctx.Users
                .Include(n => n.Notes)
                .Include(cn => cn.CollaboratorsNotes)
                .Include(e => e.Events)
                .ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await ctx.Users
                .Include(n => n.Notes)
                .Include(cn => cn.CollaboratorsNotes)
                .Include(e => e.Events)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await ctx.Users
                .Include(n => n.Notes)
                .Include(cn => cn.CollaboratorsNotes)
                .Include(e => e.Events)
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }

        public async Task<ICollection<ApplicationUser>> GetUsersByFirstAndLastNameAsync(string firstName, string lastName)
        {
            return await ctx.Users
                .Include(n => n.Notes)
                .Include(cn => cn.CollaboratorsNotes)
                .Include(e => e.Events)
                .Where(u => u.FirstName == firstName)
                .Where(u => u.LastName == lastName)
                .ToListAsync();
        }

        public async Task<ICollection<ApplicationUser>> GetUsersByUserNameAsync(string userName)
        {
            return await ctx.Users
                .Include(n => n.Notes)
                .Include(cn => cn.CollaboratorsNotes)
                .Include(e => e.Events)
                .Where(u => u.UserName == userName)
                .ToListAsync();
        }
    }
}