using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;

namespace NotesRepository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public UserRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<bool> AddUserAsync(ApplicationUser user)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                await ctx.Users.AddAsync(user);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteUserByIdAsync(Guid userId)
        {
            using (var ctx = _factory.CreateDbContext())
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
        }

        public async Task<ICollection<ApplicationUser>> GetAllUsersAsync()
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Users
                    .Include(n => n.Notes)
                    .Include(cn => cn.CollaboratorsNotes)
                    .Include(e => e.Events)
                    .ToListAsync();
            }
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Users
                    .Include(n => n.Notes)
                    .Include(cn => cn.CollaboratorsNotes)
                    .Include(e => e.Events)
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Users
                    .Include(n => n.Notes)
                    .Include(cn => cn.CollaboratorsNotes)
                    .Include(e => e.Events)
                    .FirstOrDefaultAsync(u => u.Id == userId.ToString());
            }
        }

        public async Task<ICollection<ApplicationUser>> GetUsersByFirstAndLastNameAsync(string firstName, string lastName)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Users
                    .Include(n => n.Notes)
                    .Include(cn => cn.CollaboratorsNotes)
                    .Include(e => e.Events)
                    .Where(u => u.FirstName == firstName)
                    .Where(u => u.LastName == lastName)
                    .ToListAsync();
            }
        }

        public async Task<ICollection<ApplicationUser>> GetUsersByUserNameAsync(string userName)
        {
            using (var ctx = _factory.CreateDbContext())
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
}