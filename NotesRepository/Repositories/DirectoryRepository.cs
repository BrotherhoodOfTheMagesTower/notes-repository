using Microsoft.EntityFrameworkCore;
using NotesRepository.Data;
using Directory = NotesRepository.Data.Models.Directory;
namespace NotesRepository.Repositories

{
    public class DirectoryRepository : IDirectoryRepository
    {


        private readonly IDbContextFactory<ApplicationDbContext> _factory;

      
        public DirectoryRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Adds a directory entity to the database
        /// </summary>
        /// <param name="directory">The directory entity</param>
        /// <returns>true if directory was successfully added; otherwise false</returns>
        public async Task<bool> AddDirectoryAsync(Directory directory)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                await ctx.Directories.AddAsync(directory);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Removes multiple directory entities from the database
        /// </summary>
        /// <param name="directories">directory entities</param>
        /// <returns>true if directories were successfully removed; otherwise false</returns>
        public async Task<bool> DeleteDirectoriesAsync(ICollection<Directory> directories)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                ctx.Directories.RemoveRange(directories);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Removes directory entity from the database
        /// </summary>
        /// <param name="directory">The directory entity</param>
        /// <returns>true if directory was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteDirectoryAsync(Directory directory)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                ctx.Directories.Remove(directory);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Removes a directory entity from the database by directoryId
        /// </summary>
        /// <param name="directoryId">The unique id of a directory</param>
        /// <returns>true if directory was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteDirectoryByIdAsync(Guid directoryId)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                var directory = await ctx.Directories.FirstOrDefaultAsync(x => x.DirectoryId == directoryId);
                if (directory is not null)
                {
                    ctx.Directories.Remove(directory);
                    var result = await ctx.SaveChangesAsync();
                    return result > 0;
                }
                return false;
            }
        }

        //to do -> change the method to return the directories of the given user
        /// <summary>
        ///  Gets all directories from the database
        /// </summary>
        /// <returns>A collection of directories currently stored in the database</returns>
        public async Task<ICollection<Directory>> GetAllDirectoriesAsync()
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Directories
                    .Include(n=>n.Notes)
                    .ToListAsync();
                   
            }
        }

        /// <summary>
        /// Gets a directory entity from the database by directoryId
        /// </summary>
        /// <param name="directoryId">The unique id of a directory</param>
        /// <returns>A directory entity if it exists in the db; otherwise null</returns>
        public async Task<Directory?> GetDirectoryByIdAsync(Guid directoryId)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Directories
                    .Include(n => n.Notes)
                    .FirstOrDefaultAsync(i => i.DirectoryId == directoryId);
            }
        }

        /// <summary>
        /// Gets a directory entity from the database by name
        /// </summary>
        /// <param name="name">The name of a directory</param>
        /// <returns>A directory entity if it exists in the db; otherwise null</returns>
        public async Task<Directory?> GetDirectoryByNameAsync(string name)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Directories
                    .Include(n => n.Notes)
                    .FirstOrDefaultAsync(i => i.Name == name);
            }
        }

        /// <summary>
        /// Updates the directory entity in the database
        /// </summary>
        /// <param name="directory">The directory entity</param>
        /// <returns>True if directory was successfully updated; otherwise false</returns>
        public async Task<bool> UpdateDirectoryAsync(Directory directory)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                ctx.Directories.Update(directory);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }
    }
}
