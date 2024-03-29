﻿using Microsoft.EntityFrameworkCore;
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
            if (note.Event is not null)
                note.Event = null;
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
            foreach (var note in notes)
                if (note.Event is not null) note.Event = null;

            ctx.Notes.RemoveRange(notes);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Removes multiple notes entities from the database
        /// </summary>
        /// <param name="note">The note entity</param>
        /// <returns>true if notes were successfully removed; otherwise false</returns>
        public bool DeleteMany(ICollection<Note> notes)
        {
            foreach (var note in notes)
                if (note.Event is not null) note.Event = null;

            ctx.Notes.RemoveRange(notes);
            var result = ctx.SaveChanges();
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
                if (note.Event is not null)
                    note.Event = null;
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
        /// Gets all notes without event from the database, that are assigned to specific user 
        /// </summary>
        /// <param name="userId">The unique ID of User, whose notes will be returned</param>
        /// <returns>A collection of notes without event assigned to particulat user, that are currently stored in the database</returns>
        public async Task<ICollection<Note>> GetAllUserNotesWithoutEventAsync(string userId)
        {
            return await ctx.Notes
                .Where(n => n.Owner.Id == userId)
                .Where(i => i.IsMarkedAsDeleted != true)
                .Where(e => e.Event == null)
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
            var note = await ctx.Notes.FirstOrDefaultAsync(n => n.NoteId == noteId);

            if(note is not null)
                ctx.Entry(note).Reload();

            return await ctx.Notes
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .FirstOrDefaultAsync(i => i.NoteId == noteId);
        }

        public Note? GetByIdSync(Guid noteId)
        {
            var note = ctx.Notes.FirstOrDefault(n => n.NoteId == noteId);

            ctx.Entry(note).Reload();

            return ctx.Notes
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .FirstOrDefault(i => i.NoteId == noteId);
        }

        /// <summary>
        /// Gets the note from the database by title
        /// </summary>
        /// <param name="title">The title of note</param>
        /// <returns>A note entity if it exists in the db; otherwise null</returns>
        public async Task<Note?> GetNoteByTitleAsync(string title, string userId)
        {
            return await ctx.Notes
                .Where(u => u.Owner.Id == userId)
                .Where(i => i.Title == title)
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all notes from the database, that are assigned to specific directory 
        /// </summary>
        /// <param name="directoryId">The unique ID of Directory, which notes will be returned</param>
        /// <returns>A collection of notes assigned to particular directory, that are currently stored in the database</returns>
        public async Task<ICollection<Note>> GetAllNotesForParticularDirectoryAsync(Guid directoryId)
        {
            return await ctx.Notes
                .Where(d => d.Directory.DirectoryId == directoryId)
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all notes from the database, that are assigned to specific directory 
        /// </summary>
        /// <param name="directoryId">The unique ID of Directory, which notes will be returned</param>
        /// <returns>A collection of notes assigned to particular directory, that are currently stored in the database</returns>
        public ICollection<Note> GetAllNotesForParticularDirectory(Guid directoryId)
        {
            var notes = ctx.Notes.Where(d => d.Directory.DirectoryId == directoryId).ToList();

            foreach(var note in notes)
            ctx.Entry(note).Reload();

            return ctx.Notes
                .Where(d => d.Directory.DirectoryId == directoryId)
                .Include(d => d.Directory)
                .Include(o => o.Owner)
                .Include(i => i.Images)
                .Include(e => e.EditedBy)
                .Include(ev => ev.Event)
                .Include(c => c.CollaboratorsNotes)
                .ToList();
        }

        /// <summary>
        /// Gets all notes from the database, that are were moved to the bin by single delete 
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of notes from particular user that are were moved to the bin by single delete</returns>
        public async Task<ICollection<Note>> GetAllNotesFromParticularUserThatAreCurrentlyInRecycleBinAsync(string userId)
        {
            return await ctx.Notes
                .Where(o => o.Owner.Id == userId)
                .Where(d => d.Directory.Name == "Bin")
                .ToListAsync();
        }

        /// <summary>
        /// Gets all notes from the database, that are were moved to the bin by single delete 
        /// Note objects does not include other properties. If you want to get more properties from the entity call
        /// GetNoteByIdAsync()
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <returns>A collection of notes from particular user that are were moved to the bin by single delete</returns>
        public async Task<ICollection<Note>> GetAllPinnedNotesFromUserAsync(string userId)
        {
            return await ctx.Notes
                .Where(o => o.Owner.Id == userId)
                .Where(d => d.IsPinned == true)
                .Where(b => b.IsMarkedAsDeleted == false)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all notes, which title or content contains the searchText
        /// </summary>
        /// <param name="searchText">The phrase that user has provided</param>
        /// <returns>A list of note entities if the phrase does match</returns>
        public async Task<List<Note>> SearchNoteByTitleAndContentAsync(string searchText, string userId)
        {
            return await ctx.Notes
                .Where(o => o.Owner.Id == userId)
                .Where(b => b.IsMarkedAsDeleted == false)
                .Where(t => t.Title.Contains(searchText) || t.Content.Contains(searchText))
                .ToListAsync();
        }

        /// <summary>
        /// Marks the note as currently edited
        /// </summary>
        /// <param name="noteId">The unique ID of note</param>
        /// <returns>true if note was successfully set as currently edited; otherwise false</returns>
        public async Task<bool> MarkNoteAsCurrentlyEditedAsync(Guid noteId)
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
        public async Task<bool> MarkNoteAsCurrentlyNotEditedAsync(Guid noteId)
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

        /// <summary>
        /// Marks the note as deleted
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns>true if note was successfully marked as deleted; otherwise false</returns>
        public async Task<bool> MarkNoteAsDeletedAsync(Guid noteId)
        {
            var note = await GetByIdAsync(noteId);
            if (note is not null)
            {
                note.DeletedAt = DateTime.Now;
                note.IsMarkedAsDeleted = true;
                note.Event = null;
                return await UpdateAsync(note);
            }
            return false;
        }

        /// <summary>
        /// Marks the note as not deleted
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns>true if note was successfully marked as not deleted; otherwise false</returns>
        public async Task<bool> MarkNoteAsNotDeletedAsync(Guid noteId)
        {
            var note = await GetByIdAsync(noteId);
            if (note is not null)
            {
                note.DeletedAt = null;
                note.IsMarkedAsDeleted = false;
                return await UpdateAsync(note);
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <returns>Returns <paramref name="count"/> recently edited or created notes of a particular user</returns>
        public async Task<ICollection<Note>> GetRecentlyEditedNotesAsync(string userId, int count)
        {
            var allNotes = (await GetAllUserNotesAsync(userId)).Where(b => b.IsMarkedAsDeleted == false);
            return allNotes.OrderByDescending(x => x.EditedAt).Take(count).ToArray();
        }

        /// <summary>
        /// Gets all notes, which were transferred 'in bulk' to bin at least 30 days ago
        /// </summary>
        /// <returns>An ICollection of Note entities, which were transferred 'in bulk' to bin at least 30 days ago</returns>
        public ICollection<Note> GetAllSingleNotesWhichShouldBeRemovedFromDb(int daysOld = 30)
            => ctx.Notes
            .Where(x => x.DeletedAt < DateTime.Now.AddDays(-daysOld)
                && x.Directory.Name == "Bin"
                && x.IsMarkedAsDeleted == true)
            .ToArray();
    }
}