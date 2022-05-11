using Microsoft.EntityFrameworkCore;
using NotesRepository.Data;
using NotesRepository.Repositories.Interfaces;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Repositories
{
    public class DirectoryRepository : IDirectoryRepository
    {
        private readonly ApplicationDbContext ctx;

        public DirectoryRepository(ApplicationDbContext context)
        {
            ctx = context;
        }

        /// <summary>
        /// Adds a directory entity to the database
        /// </summary>
        /// <param name="directory">The directory entity</param>
        /// <returns>true if directory was successfully added; otherwise false</returns>
        public async Task<bool> AddAsync(Directory directory)
        {
            await ctx.Directories.AddAsync(directory);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }


/// <summary>
/// Attaches a subdirectory entity to the particular directory. 
/// </summary>
/// <param name="subDirectory">The subdirectory entity</param>
/// <param name="directoryId">The unique ID of directory</param>
/// <returns>true if subdirectory was successfully added; otherwise false</returns>
public async Task<bool> ChangeParentDirectoryForSubDirectory(Guid subDirectoryId, Guid directoryId)
        {
            var dir = await ctx.Directories.SingleOrDefaultAsync(x => x.DirectoryId == directoryId);
            var subDir = await ctx.Directories.SingleOrDefaultAsync(x => x.DirectoryId == subDirectoryId);
            if (dir is not null && subDir is not null)
            {
                subDir.ParentDir = dir;

                ctx.Directories.Update(dir);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Removes multiple directory entities from the database
        /// </summary>
        /// <param name="directories">directory entities</param>
        /// <returns>true if directories were successfully removed; otherwise false</returns>
        public async Task<bool> DeleteManyAsync(ICollection<Directory> directories)
        {
            ctx.Directories.RemoveRange(directories);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Removes directory entity from the database
        /// </summary>
        /// <param name="directory">The directory entity</param>
        /// <returns>true if directory was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteAsync(Directory directory)
        {
            ctx.Directories.Remove(directory);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Removes a directory entity from the database by directoryId
        /// </summary>
        /// <param name="directoryId">The unique id of a directory</param>
        /// <returns>true if directory was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteByIdAsync(Guid directoryId)
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

        /// <summary>
        /// Removes all subdirectory entities for particular directory
        /// </summary>
        /// <param name="directoryId">The unique ID of a directory</param>
        /// <returns>true if all subdirectories were successfully removed; otherwise false</returns>
        public async Task<bool> DeleteAllSubDirectoriesForParticularDirectoryAsync(Guid directoryId)
        {
            var directory = await GetByIdAsync(directoryId);
            if (directory is not null)
            {
                if (directory.SubDirectories is not null)
                {
                    ctx.Directories.RemoveRange(directory.SubDirectories);
                    var result = await ctx.SaveChangesAsync();
                    return result > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a directory entity from the database by directoryId
        /// </summary>
        /// <param name="directoryId">The unique id of a directory</param>
        /// <returns>A directory entity if it exists in the db; otherwise null</returns>
        public async Task<Directory?> GetByIdAsync(Guid directoryId)
        {
            return await ctx.Directories
                .Where(d => d.DirectoryId == directoryId)
                .Include(n => n.Notes)
                .Include(s => s.SubDirectories)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Gets a directory entity from the database by name
        /// </summary>
        /// <param name="name">The name of a directory</param>
        /// <returns>A directory entity if it exists in the db; otherwise null</returns>
        public async Task<Directory?> GetDirectoryByNameAsync(string name, string userId)
        {
            return await ctx.Directories
                .Where(u => u.User.Id == userId)
                .Include(n => n.Notes)
                .Include(s => s.SubDirectories)
                .FirstOrDefaultAsync(i => i.Name == name);
        }

        /// <summary>
        /// Gets all directories from specific user
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of directories from specific user</returns>
        public async Task<ICollection<Directory>> GetAllDirectoriesForParticularUserAsync(string userId)
        {
            return await ctx.Directories
                .Where(u => u.User.Id == userId)
                .Include(n => n.Notes)
                .Include(s => s.SubDirectories)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all subdirectories for specific directory
        /// </summary>
        /// <param name="directoryId">The unique ID of the directory</param>
        /// <returns>A collection of subdirectories for specific directory</returns>
        public async Task<ICollection<Directory>?> GetAllSubDirectoriesOfParticularDirectory(Guid directoryId)
        {
            return await ctx.Directories
                .Include(s => s.SubDirectories)
                .Include(s => s.Notes)
                .Where(d => d.ParentDir.DirectoryId == directoryId)
                .ToListAsync();
        }

        public ICollection<Directory>? GetAllSubDirectoriesOfParticularDirectorySync(Guid directoryId)
        {
            return ctx.Directories
                .Include(s => s.SubDirectories)
                .Include(s => s.Notes)
                .Where(d => d.ParentDir.DirectoryId == directoryId)
                .ToList();
        }

        /// <summary>
        /// Gets all subdirectories for specific directory
        /// </summary>
        /// <param name="directoryId">The unique ID of the directory</param>
        /// <returns>A collection of subdirectories for specific directory</returns>
        public async Task<ICollection<Directory>?> GetAllDirectoriesWithoutParentDirectoryForParticularUserAsync(string userId)
        {
            return await ctx.Directories
                .Where(u => u.User.Id == userId)
                .Where(d => d.ParentDir == null)
                .Where(n => n.Name != "Bin")
                .Include(s => s.SubDirectories)
                .Include(s => s.Notes)
                .ToListAsync();
        }

        /// <summary>
        /// Updates the directory entity in the database
        /// </summary>
        /// <param name="directory">The directory entity</param>
        /// <returns>True if directory was successfully updated; otherwise false</returns>
        public async Task<bool> UpdateAsync(Directory directory)
        {
            ctx.Directories.Update(directory);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Marks the directory as currently edited
        /// </summary>
        /// <param name="directoryId">The unique ID of directory</param>
        /// <returns>true if note was successfully marked as deleted; otherwise false</returns>
        public async Task<bool> MarkDirectoryAsDeletedAsync(Guid directoryId)
        {
            var directory = await ctx.Directories.SingleOrDefaultAsync(x => x.DirectoryId == directoryId);
            if (directory is not null)
            {
                directory.IsMarkedAsDeleted = true;
                directory.DeletedAt = DateTime.UtcNow;
                ctx.Update(directory);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Marks the directory as currently edited
        /// </summary>
        /// <param name="directoryId">The unique ID of directory</param>
        /// <returns>true if note was successfully marked as not deleted; otherwise false</returns>
        public async Task<bool> MarkDirectoryAsNotDeletedAsync(Guid directoryId)
        {
            var directory = await ctx.Directories.SingleOrDefaultAsync(x => x.DirectoryId == directoryId);
            if (directory is not null)
            {
                directory.IsMarkedAsDeleted = false;
                directory.DeletedAt = null;
                ctx.Update(directory);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}
