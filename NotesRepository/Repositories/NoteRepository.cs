using Microsoft.EntityFrameworkCore;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using Directory = NotesRepository.Data.Models.Directory;
using NotesRepository.Repositories.Interfaces;

namespace NotesRepository.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly ApplicationDbContext ctx;

        public NoteRepository(ApplicationDbContext context)
        {
            ctx = context;
        }

        /// <summary>
        /// Adds a note entity to the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if note was successfully added; otherwise false</returns>
        public async Task<bool> AddAsync(Note note)
        {
            await ctx.Notes.AddAsync(note);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Adds multiple notes entities to the database
        /// </summary>
        /// <param name="notes">Note entities</param>
        /// <returns>true if notes were successfully added; otherwise false</returns>
        public async Task<bool> AddManyAsync(ICollection<Note> notes)
        {
            await ctx.Notes.AddRangeAsync(notes);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Removes a note entity from the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if note was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteAsync(Note note)
        {
            ctx.Notes.Remove(note);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Removes multiple notes entities from the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if notes were successfully removed; otherwise false</returns>
        public async Task<bool> DeleteManyAsync(ICollection<Note> notes)
        {
            ctx.Notes.RemoveRange(notes);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Removes a note entity from the database by noteId
        /// </summary>
        /// <param name="noteId">The unique id of a note</param>
        /// <returns>true if note was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteByIdAsync(Guid noteId)
        {
            var note = await ctx.Notes.FirstOrDefaultAsync(x => x.NoteId == noteId);
            if (note is not null)
            {
                ctx.Notes.Remove(note);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Updates the note entity in the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if note was successfully updated; otherwise false</returns>
        public async Task<bool> UpdateAsync(Note note)
        {
            ctx.Notes.Update(note);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Gets all notes from the database, that are assigned to specific user 
        /// </summary>
        /// <param name="userId">The unique ID of User, whose notes will be returned</param>
        /// <returns>A collection of notes assigned to particulat user, that are currently stored in the database</returns>
        public async Task<ICollection<Note>> GetAllUserNotesAsync(string userId)
        {
            return await ctx.Notes
                .Where(n => n.Owner.Id == userId)
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the note from the database by noteId
        /// </summary>
        /// <param name="noteId">The unique ID of note</param>
        /// <returns>A note entity if it exists in the db; otherwise null</returns>
        public async Task<Note?> GetByIdAsync(Guid noteId)
        {
            return await ctx.Notes
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .FirstOrDefaultAsync(i => i.NoteId == noteId);
        }

        /// <summary>
        /// Gets the note from the database by title
        /// </summary>
        /// <param name="title">The title of note</param>
        /// <returns>A note entity if it exists in the db; otherwise null</returns>
        public async Task<Note?> GetNoteByTitleAsync(string title)
        {
            return await ctx.Notes
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .FirstOrDefaultAsync(i => i.Title == title);
        }

        /// <summary>
        /// Gets all notes, which title or content contains the searchText
        /// </summary>
        /// <param name="searchText">The phrase that user has provided</param>
        /// <returns>A list of note entities if the phrase does match</returns>
        public async Task<List<Note>> SearchNoteByTitleAndContentAsync(string searchText)
        {
            return await ctx.Notes
                .Where(t => t.Title.Contains(searchText) || t.Content.Contains(searchText))
                .ToListAsync();
        }

        /// <summary>
        /// Marks the note as currently edited
        /// </summary>
        /// <param name="noteId">The unique ID of note</param>
        /// <returns>true if note was successfully set as currently edited; otherwise false</returns>
        public async Task<bool> SetNoteAsCurrentlyEditedAsync(Guid noteId)
        {
            var note = await ctx.Notes.SingleOrDefaultAsync(x => x.NoteId == noteId);
            if (note is not null)
            {
                note.IsCurrentlyEdited = true;
                ctx.Update(note);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Marks the note as currently not edited
        /// </summary>
        /// <param name="noteId">The unique ID of note</param>
        /// <returns>true if note was successfully set as currently not edited; otherwise false</returns>
        public async Task<bool> SetNoteAsCurrentlyNotEditedAsync(Guid noteId)
        {
            var note = await ctx.Notes.SingleOrDefaultAsync(x => x.NoteId == noteId);
            if (note is not null)
            {
                note.IsCurrentlyEdited = false;
                ctx.Update(note);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}
