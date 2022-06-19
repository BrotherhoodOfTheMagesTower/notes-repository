using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories.Interfaces;

namespace NotesRepository.Repositories
{
    public class CollaboratorsNotesRepository : ICollaboratorsNotesRepository
    {
        private readonly ApplicationDbContext ctx;
        public CollaboratorsNotesRepository(ApplicationDbContext context)
        {
            ctx = context;
        }

        /// <summary>
        /// Adds collaborator to note 
        /// </summary>
        /// <param name="collaborator"></param>
        /// <returns>true if collaborator was successfully added; otherwise false</returns>
        public async Task<bool> AddCollaboratorToNoteAsync(CollaboratorsNotes collaborator)
        {
            await ctx.CollaboratorsNotes.AddAsync(collaborator);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Add multiple collaborators to note
        /// </summary>
        /// <param name="collaborator"></param>
        /// <returns>true if collaborators were successfully added; otherwise false</returns>
        public async Task<bool> AddCollaboratorsToNoteAsync(ICollection<CollaboratorsNotes> collaborators)
        {
            await ctx.CollaboratorsNotes.AddRangeAsync(collaborators);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Removes collaborator from note
        /// </summary>
        /// <param name="collaborator"></param>
        /// <returns>true if collaborator was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteCollaboratorFromNoteAsync(CollaboratorsNotes collaborator)
        {
            ctx.CollaboratorsNotes.Remove(collaborator);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }
        /// <summary>
        /// Delete multiple collaborators from note
        /// </summary>
        /// <param //name="collaborator"></param>
        /// <returns>true if collaborators were successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteCollaboratorsFromNoteAsync(ICollection<CollaboratorsNotes> collaborators)
        {
            var list = new List<CollaboratorsNotes?>();
            foreach (var collaborator in collaborators)
                list.Add(await ctx.CollaboratorsNotes.Where(c => c.Equals(collaborator)).SingleOrDefaultAsync());
            ctx.CollaboratorsNotes.RemoveRange(collaborators);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// Removes collaborator from note
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="appUserId"></param>
        /// <returns>true if note was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteCollaboratorFromNoteAsync(Guid noteId, string appUserId)
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

        /// <summary>
        /// Get all collaborators for note
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns>A collection of collaborators to which the note was shared</returns>
        public async Task<ICollection<ApplicationUser>> GetAllCollaboratorsForNote(Guid noteId)
        {
            return await ctx.CollaboratorsNotes
                .Where(i => i.NoteId == noteId)
                .Select(u => u.Collaborator)
                .ToListAsync();
        }

        /// <summary>
        /// Get all shared notes for user
        /// <param name="appUserId"></param>
        /// <returns>A collection of notes that were shared with a specified user</returns>
        public async Task<ICollection<Note>> GetAllSharedNotesForUser(string appUserId)
        {
            return await ctx.CollaboratorsNotes
                .Where(a => a.ApplicationUserId == appUserId)
                .Where(i => i.SharedNote.IsMarkedAsDeleted != true)
                .Select(x => x.SharedNote)
                .ToListAsync();
        }
    }
}
