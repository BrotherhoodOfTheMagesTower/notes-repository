using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Tests
{
    public class NoteRepositoryShould
    {
        private readonly ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;

        public NoteRepositoryShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("In memory database - Note")
                .Options;
            _context = new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "User is able to add a note to the database")]
        public async Task AddNote()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr));

            // Act
            var result = await nr.AddAsync(note);

            // Assert
            var notes = await _context.Notes.ToListAsync();
            Assert.True(result);
            Assert.Single(notes);
            await nr.DeleteAsync(note);
        }

        [Fact(DisplayName = "User is able to add multiple notes to the database")]
        public async Task AddMultipleNotes()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var notes = new List<Note>()
            {
                new Note(null, "Test note 1", "for AddMultipleNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note 2", "for AddMultipleNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note 3", "for AddMultipleNotes()", "def-ico", usr, new Directory("Default", usr))
            };

            // Act
            var result = await nr.AddManyAsync(notes);

            // Assert
            var notesFromDb = await _context.Notes.ToListAsync();
            Assert.True(result);
            notesFromDb.Should().ContainItemsAssignableTo<Note>().And.HaveCount(3);
            await nr.DeleteManyAsync(notes);
        }

        [Fact(DisplayName = "User is able to delete a note from the database")]
        public async Task DeleteNote()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for DeleteNote()", "def-ico", usr, new Directory("Default", usr));
            await nr.AddAsync(note);

            // Act
            var result = await nr.DeleteAsync(note);

            // Assert
            var notes = await _context.Notes.ToListAsync();
            Assert.True(result);
            Assert.Empty(notes);
        }

        [Fact(DisplayName = "User is able to delete a note by id from the database")]
        public async Task DeleteNoteById()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for DeleteNoteById()", "def-ico", usr, new Directory("Default", usr));
            await nr.AddAsync(note);

            // Act
            var result = await nr.DeleteByIdAsync(note.NoteId);

            // Assert
            var notes = await _context.Notes.ToListAsync();
            Assert.True(result);
            Assert.Empty(notes);
        }

        [Fact(DisplayName = "User is able to delete multiple notes from the database")]
        public async Task DeleteMultipleNotesB()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var notes = new List<Note>()
            {
                new Note(null, "Test note 1", "for AddMultipleNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note 2", "for AddMultipleNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note 3", "for AddMultipleNotes()", "def-ico", usr, new Directory("Default", usr))
            };
            await nr.AddManyAsync(notes);

            // Act
            var result = await nr.DeleteManyAsync(notes);

            // Assert
            var notesFromDb = await _context.Notes.ToListAsync();
            Assert.True(result);
            notesFromDb.Should().BeEmpty();
        }

        [Fact(DisplayName = "User is able to update a note in the database")]
        public async Task UpdateNote()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for UpdateNote() - not updated", "def-ico", usr, new Directory("Default", usr));
            await nr.AddAsync(note);
            note.Content = "for UpdateNote() - updated";

            // Act
            var result = await nr.UpdateAsync(note);

            // Assert
            var notes = await _context.Notes.ToListAsync();
            Assert.True(result);
            Assert.Single(notes);
            Assert.Equal(note.Content, notes.First(i => i.NoteId == note.NoteId).Content);
            await nr.DeleteAsync(note);
        }

        [Fact(DisplayName = "User is able to get all notes from the database")]
        public async Task GetAllNotes()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var notes = new List<Note>()
            {
                new Note(null, "Test note 1", "for GetAllNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note 2", "for GetAllNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note 3", "for GetAllNotes()", "def-ico", usr, new Directory("Default", usr))
            };
            await nr.AddManyAsync(notes);

            // Act
            var result = await nr.GetAllUserNotesAsync(usr.Id);

            // Assert
            var notesFromDb = await _context.Notes
                    .Include(d => d.Directory)
                    .Include(o => o.Owner)
                    .Include(i => i.Images)
                    .Include(e => e.EditedBy)
                    .Include(ev => ev.Event)
                    .Include(c => c.CollaboratorsNotes)
                    .ToListAsync();
            await nr.DeleteManyAsync(notes);
            Assert.NotNull(result);
            Assert.Equal(result.Select(x => x.NoteId), notes.Select(x => x.NoteId));
            result.Should().AllBeAssignableTo<Note>();
        }

        [Fact(DisplayName = "User is able to get all of his notes from the database")]
        public async Task GetAllUserNotes()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var user = new ApplicationUser { FirstName = "Hugo", LastName = "Ko³³¹taj", Id = Guid.NewGuid().ToString() };
            var anotherUser = new ApplicationUser();
            var notes = new List<Note>()
            {
                new Note(null, "Test note 1", "for GetAllNotes()", "def-ico", user, new Directory("Default", user)),
                new Note(null, "Test note 2", "for GetAllNotes()", "def-ico", user, new Directory("Default", user)),
                new Note(null, "Test note 3", "for GetAllNotes()", "def-ico", anotherUser, new Directory("Default", anotherUser))
            };
            await nr.AddManyAsync(notes);

            // Act
            var result = await nr.GetAllUserNotesAsync(user.Id);

            // Assert
            await nr.DeleteManyAsync(notes);
            Assert.NotNull(result);
            result.Should().AllBeAssignableTo<Note>();
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Owner.Id == user.Id);
        }

        [Fact(DisplayName = "User is able to get a note by id from the database")]
        public async Task GetNotesById()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for GetNotesById()", "def-ico", usr, new Directory("Default", usr));
            await nr.AddAsync(note);

            // Act
            var result = await nr.GetByIdAsync(note.NoteId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(note.NoteId, result!.NoteId);
            result.Should().BeAssignableTo<Note>();
            await nr.DeleteAsync(note);
        }

        [Fact(DisplayName = "User is able to get a note by title from the database")]
        public async Task GetNoteByTitle()
        {
            // Arrange
            var nr = new NoteRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for GetNoteByTitle()", "def-ico", usr, new Directory("Default", usr));
            await nr.AddAsync(note);

            // Act
            var result = await nr.GetNoteByTitleAsync("Test note");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(note.NoteId, result!.NoteId);
            result.Should().BeAssignableTo<Note>();
            await nr.DeleteAsync(note);
        }
    }
}