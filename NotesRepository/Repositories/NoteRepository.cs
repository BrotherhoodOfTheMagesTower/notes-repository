using Microsoft.EntityFrameworkCore;
using NotesRepository.Data;
using NotesRepository.Data.Models;

namespace NotesRepository.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public NoteRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Adds a note entity to the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if note was successfully added; otherwise false</returns>
        public async Task<bool> AddNoteAsync(Note note)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                await ctx.Notes.AddAsync(note);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Adds multiple notes entities to the database
        /// </summary>
        /// <param name="note"></param>
        /// <returns>true if notes were successfully added; otherwise false</returns>
        public async Task<bool> AddNotesAsync(ICollection<Note> notes)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                await ctx.Notes.AddRangeAsync(notes);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Removes a note entity from the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if note was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteNoteAsync(Note note)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                ctx.Notes.Remove(note);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }
        
        /// <summary>
        /// Removes multiple notes entities from the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if notes were successfully removed; otherwise false</returns>
        public async Task<bool> DeleteNotesAsync(ICollection<Note> notes)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                ctx.Notes.RemoveRange(notes);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Removes a note entity from the database by noteId
        /// </summary>
        /// <param name="noteId">The unique id of a note</param>
        /// <returns>true if note was successfully removed; otherwise false</returns>
        public async Task<bool> DeleteNoteByIdAsync(Guid noteId)
        {
            using (var ctx = _factory.CreateDbContext())
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
        }

        /// <summary>
        /// Updates the note entity in the database
        /// </summary>
        /// <param name="note"></param>
        /// <returns>true if note was successfully updated; otherwise false</returns>
        public async Task<bool> UpdateNoteAsync(Note note)
        {
            using (var ctx = _factory.CreateDbContext())
            {
                ctx.Notes.Update(note);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// Gets all notes from the database
        /// </summary>
        /// <returns>A collection of notes currently stored in the database</returns>
        public async Task<ICollection<Note>> GetAllNotesAsync()
        {
            using (var ctx = _factory.CreateDbContext())
            {
                return await ctx.Notes
                    .Include(d => d.Directory)
                    .Include(o => o.Owner)
                    .Include(i => i.Images)
                    .Include(e => e.EditedBy)
                    .Include(ev => ev.Event)
                    .Include(c => c.CollaboratorsNotes)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets all notes from the database, that are assigned to specific user 
        /// </summary>
        /// <returns>A collection of notes assigned to particulat user, that are currently stored in the database</returns>
        public async Task<ICollection<Note>> GetAllUserNotesAsync(string userId)
        {
            using (var ctx = _factory.CreateDbContext())
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
        }

        /// <summary>
        /// Gets the note from the database by noteId
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns>A note entity if it exists in the db; otherwise null</returns>
        public async Task<Note?> GetNoteByIdAsync(Guid noteId)
        {
            using (var ctx = _factory.CreateDbContext())
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
        }

        /// <summary>
        /// Gets the note from the database by title
        /// </summary>
        /// <param name="title"></param>
        /// <returns>A note entity if it exists in the db; otherwise null</returns>
        public async Task<Note?> GetNoteByTitleAsync(string title)
        {
            using (var ctx = _factory.CreateDbContext())
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
        }
    }
}
