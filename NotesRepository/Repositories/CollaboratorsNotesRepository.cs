﻿using Microsoft.EntityFrameworkCore;
using NotesRepository.Data;
using NotesRepository.Data.Models;


namespace NotesRepository.Repositories
{
    public class CollaboratorsNotesRepository : ICollaboratorsNotesRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        public CollaboratorsNotesRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// <summary>
        /// Add note to collaborator 
        /// </summary>
        /// <param name="collaborator"></param>
        /// <returns>true if notes were successfully added; otherwise false</returns>
        public async Task<bool> AddNoteToCollaboratorAsync(CollaboratorsNotes collaborator)
        {
            using (var ctx = _factory.CreateDbContext())
            {

                await ctx.CollaboratorsNotes.AddAsync(collaborator);
                var result = await ctx.SaveChangesAsync();
                return result > 0;

            }
        }
        /// <summary>
        /// Add multiple notes to collaborator
        /// </summary>
        /// <param name="collaborator"></param>
        /// <returns>true if notes were successfully added; otherwise false</returns>
        public async Task<bool> AddNotesToCollaboratorAsync(ICollection<CollaboratorsNotes> collaborators)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                await ctx.CollaboratorsNotes.AddRangeAsync(collaborators);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Removing note from collaborator
        /// </summary>
        /// <param name="collaborator"></param>
        /// <returns>true if note was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteNoteFromCollaboratorAsync(CollaboratorsNotes collaborator)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                ctx.CollaboratorsNotes.Remove(collaborator);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }
        /// <summary>
        /// Delete multiple notes from collaborator
        /// </summary>
        /// <param //name="collaborator"></param>
        /// <returns>true if notes were successfully added; otherwise false</returns>
        public async Task<bool> DeleteNotesFromCollaboratorAsync(ICollection<CollaboratorsNotes> collaborators)
        {
            using (var ctx = _factory.CreateDbContext())
            {

                ctx.CollaboratorsNotes.RemoveRange(collaborators);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// Removing note from collaborator including id of the user and id of the note
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="appUserId"></param>
        /// <returns>true if note was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteNoteFromCollaboratorAsync(Guid noteId, string appUserId)
        {
            using (var ctx = _factory.CreateDbContext())
            {

                var collaborator = await ctx.CollaboratorsNotes.Where(a => a.ApplicationUserId == appUserId && a.NoteId == noteId).FirstOrDefaultAsync();

                if (collaborator is not null)
                {
                    ctx.CollaboratorsNotes.Remove(collaborator);
                    var result = await ctx.SaveChangesAsync();
                    return result > 0;
                }
                return false;

            }
        }

    

        /// <summary>
        /// Get all users including id of the note from database
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns>A collection of users related to the note</returns>
        public async Task<ICollection<CollaboratorsNotes>> GetAllUsersRelatedToTheNoteAsync(Guid noteId)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                 return await ctx.CollaboratorsNotes.Where(i => i.NoteId == noteId).ToListAsync();
            }
                 
        }

        /// <summary>
        /// Get all notes including id of the user from database
        /// </summary>
        /// <param name="appUserId"></param>
        /// <returns>A collection of notes that can be edited by specified user</returns>
        public async Task<ICollection<CollaboratorsNotes>> GetAllNotesCanBeEditedByUserAsync(string appUserId)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.CollaboratorsNotes.Where(a => a.ApplicationUserId == appUserId).ToListAsync();
            }
        }


    }
}