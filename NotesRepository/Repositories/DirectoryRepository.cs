﻿using Microsoft.EntityFrameworkCore;
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
        /// Removes a directory entity from the database by directoryId
        /// </summary>
        /// <param name="directoryId">The unique id of a directory</param>
        /// <returns>true if directory was successfully removed; otherwise false</returns>
        public bool DeleteByIdSync(Guid directoryId)
        {
            var directory = ctx.Directories.FirstOrDefault(x => x.DirectoryId == directoryId);
            if (directory is not null)
            {
                ctx.Directories.Remove(directory);
                var result = ctx.SaveChanges();
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
        /// <param name="userId">User id</param>
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
        /// Gets all not deleted directories from specific user
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of directories from specific user</returns>
        public async Task<ICollection<Directory>> GetAllNotDeletedDirectoriesForParticularUserAsync(string userId)
        {
            return await ctx.Directories
                .Where(u => u.User.Id == userId)
                .Where(n => n.Name != "Bin")
                .Where(i => i.IsMarkedAsDeleted != true)
                .Include(n => n.Notes)
                .Include(s => s.SubDirectories)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all subdirectories for specific directory
        /// </summary>
        /// <param name="directoryId">The unique ID of the directory</param>
        /// <returns>A collection of subdirectories for specific directory</returns>
        public async Task<ICollection<Directory>?> GetAllSubDirectoriesOfParticularDirectoryAsync(Guid directoryId)
        {
            return await ctx.Directories
                .Include(s => s.SubDirectories)
                .Include(s => s.Notes)
                .Where(d => d.ParentDir.DirectoryId == directoryId)
                .ToListAsync();
        }


        /// <summary>
        /// Gets all subdirectories for specific directory
        /// </summary>
        /// <param name="directoryId">The unique ID of the directory</param>
        /// <returns>A collection of subdirectories for specific directory</returns>
        public ICollection<Directory>? GetAllSubDirectoriesOfParticularDirectorySync(Guid directoryId)
        {
            return ctx.Directories
                .Include(s => s.SubDirectories)
                .Include(s => s.Notes)
                .Where(d => d.ParentDir.DirectoryId == directoryId)
                .ToList();
        }

        /// <summary>
        /// Gets all directories without parent directory for particular user which aren't deleted and aren't named bin
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of directories without parent directory</returns>
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
        /// Gets all directories without parent directory for particular user which aren't deleted and aren't named bin
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of directories without parent directory</returns>
        public ICollection<Directory>? GetAllDirectoriesWithoutParentDirectoryForParticularUserSync(string userId)
        {
            return ctx.Directories
                .Where(u => u.User.Id == userId)
                .Where(d => d.ParentDir == null)
                .Where(n => n.Name != "Bin")
                .Include(s => s.SubDirectories)
                .Include(s => s.Notes)
                .ToList();
        }

        /// <summary>
        /// Gets all directories, which were in bin for param daysOld days
        /// </summary>
        /// <param name="daysOld">time how long folders are in the bin in days</param>
        /// <returns>A collection of directories which are in bin for param daysOld days</returns>
        public ICollection<Directory> GetMainDirectoriesWhichShouldBeRemovedFromDbSync(int daysOld)
            => ctx.Directories
              .Where(x => x.ParentDir.Name == "Bin"
                && x.DeletedAt < DateTime.Now.AddDays(-daysOld)
                && x.IsMarkedAsDeleted == true)
            .ToArray();


        /// <summary>
        /// Attaches a subdirectory entity to the particular directory. 
        /// </summary>
        /// <param name="subDirectory">The subdirectory entity</param>
        /// <param name="directoryId">The unique ID of directory</param>
        /// <returns>true if subdirectory was successfully added; otherwise false</returns>
        public async Task<bool> ChangeParentDirectoryForSubDirectoryAsync(Guid subDirectoryId, Guid directoryId)
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
        /// Marks the directory as deleted
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
        /// Marks the directory as not deleted
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
