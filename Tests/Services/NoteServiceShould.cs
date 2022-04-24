using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using NotesRepository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public class NoteServiceShould
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly EventRepository _er;
        private readonly DirectoryRepository _dr;
        private readonly ImageRepository _ir;
        private readonly ApplicationDbContext ctx;
        private DbContextOptions<ApplicationDbContext> _options;

        public NoteServiceShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("In memory database - NoteService")
                .Options;
            ctx = CreateDbContext();
            _nr = new NoteRepository(ctx);
            _ur = new UserRepository(ctx);
            _er = new EventRepository(ctx);
            _dr = new DirectoryRepository(ctx);
            _ir = new ImageRepository(ctx);
        }
        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "Be able to get a note by ID")]
        public async Task GetNoteById()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "GetNoteById()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            var result = await ns.GetNoteByIdAsync(note.NoteId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result!.NoteId, note.NoteId);
        }

        [Fact(DisplayName = "Be able to get all user notes by user ID")]
        public async Task GetAllUserNotesById()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var notes = new List<Note>
            {
                new Note(null, "Test note1", "GetAllUserNotesById()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note2", "GetAllUserNotesById()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note3", "GetAllUserNotesById()", "def-ico", usr, new Directory("Default", usr))
            };
            await ns.AddNotesAsync(notes);

            // Act
            var results = await ns.GetAllUserNotesByIdAsync(usr.Id);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(results.Select(x => x.NoteId), notes.Select(x => x.NoteId));
        }

        [Fact(DisplayName = "Be able to get a note by title")]
        public async Task GetNoteByTitle()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Custom", "GetNoteByTitle()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            var result = await ns.GetNoteByTitleAsync("Custom");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result!.NoteId, note.NoteId);
        }

        [Fact(DisplayName = "Be able to get note by searching (title & content)")]
        public async Task SearchNotesByTitleAndContent()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var notes = new List<Note>
            {
                new Note(null, "Test note1", "Search1()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note2", "Search2()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note3", "Search3()", "def-ico", usr, new Directory("Default", usr))
            };
            await ns.AddNotesAsync(notes);

            // Act
            var titleResult = await ns.SearchNotesByTitleAndContentAsync("te1");
            var contentResult = await ns.SearchNotesByTitleAndContentAsync("rch2");
            var notMatchResult = await ns.SearchNotesByTitleAndContentAsync("foo");
            var multipleTitleMatchResult = await ns.SearchNotesByTitleAndContentAsync("note");
            var multipleContentMatchResult = await ns.SearchNotesByTitleAndContentAsync("earch");

            // Assert
            Assert.NotNull(titleResult);
            Assert.Single(titleResult);
            Assert.NotNull(contentResult);
            Assert.Single(contentResult);
            Assert.NotNull(multipleTitleMatchResult);
            multipleTitleMatchResult.Should().HaveCount(3);
            Assert.NotNull(multipleContentMatchResult);
            multipleContentMatchResult.Should().HaveCount(3);
            notMatchResult.Should().BeEmpty();
        }

        [Fact(DisplayName = "Be able to add a note to the database")]
        public async Task AddNote()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "AddNote()", "def-ico", usr, new Directory("Default", usr));

            // Act
            var result = await ns.AddNoteAsync(note);

            // Assert
            var notes = await ns.GetAllUserNotesByIdAsync(usr.Id);
            Assert.True(result);
            Assert.Single(notes);
        }

        [Fact(DisplayName = "Be able to add multiple notes to the database")]
        public async Task AddMultipleNotes()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var notes = new List<Note>
            {
                new Note(null, "Test note1", "Search1()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note2", "Search2()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note3", "Search3()", "def-ico", usr, new Directory("Default", usr))
            };

            // Act
            await ns.AddNotesAsync(notes);

            // Assert
            var result = await ns.GetAllUserNotesByIdAsync(usr.Id);
            result.Should().HaveCount(3);
        }

        [Fact(DisplayName = "Be able to update a note")]
        public async Task UpdateNote()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "UpdateNote()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);
            note.Title = "Edited note";

            // Act
            await ns.UpdateNoteAsync(note);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Title.Should().Be("Edited note");
        }

        [Fact(DisplayName = "Be able to delete a note")]
        public async Task DeleteNote()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "DeleteNote()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.DeleteNoteAsync(note);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().BeNull();
        }

        [Fact(DisplayName = "Be able to delete a note by ID")]
        public async Task DeleteNoteById()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "DeleteNoteById()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.DeleteNoteByIdAsync(note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().BeNull();
        }

        [Fact(DisplayName = "Be able to delete notes")]
        public async Task DeleteNotes()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var notes = new List<Note>
            {
                new Note(null, "Test note1", "DeleteNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note2", "DeleteNotes()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note3", "DeleteNotes()", "def-ico", usr, new Directory("Default", usr))
            };
            await ns.AddNotesAsync(notes);

            // Act
            await ns.DeleteNotesAsync(notes);

            // Assert
            var noteFromDb = await ns.GetAllUserNotesByIdAsync(usr.Id);
            noteFromDb.Should().BeEmpty();
        }

        [Fact(DisplayName = "Be able to delete notes by ID")]
        public async Task DeleteNotesById()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var notes = new List<Note>
            {
                new Note(null, "Test note1", "DeleteNotesById()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note2", "DeleteNotesById()", "def-ico", usr, new Directory("Default", usr)),
                new Note(null, "Test note3", "DeleteNotesById()", "def-ico", usr, new Directory("Default", usr))
            };
            await ns.AddNotesAsync(notes);

            // Act
            foreach (var note in notes)
                await ns.DeleteNoteByIdAsync(note.NoteId);

            // Assert
            var noteFromDb = await ns.GetAllUserNotesByIdAsync(usr.Id);
            noteFromDb.Should().BeEmpty();
        }

        [Fact(DisplayName = "Be able to set note as currently edited")]
        public async Task SetNoteAsCurrentlyEdited()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "SetNoteAsCurrentlyEdited()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.SetNoteAsCurrentlyEditedAsync(note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.IsCurrentlyEdited.Should().BeTrue();
        }

        [Fact(DisplayName = "Be able to set note as currently not edited")]
        public async Task SetNoteAsCurrentlyNotEdited()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr = new ApplicationUser();
            var note = new Note(null, "Any", "SetNoteAsCurrentlyNotEdited()", "def-ico", usr, new Directory("Default", usr));
            note.IsCurrentlyEdited = true;
            await ns.AddNoteAsync(note);

            // Act
            await ns.SetNoteAsCurrentlyNotEditedAsync(note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.IsCurrentlyEdited.Should().BeFalse();
        }

        [Fact(DisplayName = "Be able to set last edited time and user")]
        public async Task SetLastEditedTimeAndUser()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "SetLastEditedTimeAndUser()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);
            var time = DateTime.Now;
            var editUser = new ApplicationUser();
            await _ur.AddUserAsync(editUser);

            // Act
            await ns.SetLastEditedTimeAndUserAsync(time, editUser.Id, note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.EditedBy!.Id.Should().Be(editUser.Id);
            noteFromDb!.EditedAt.Should().Be(time);
        }

        [Fact(DisplayName = "Be able to mark note as deleted and start timer")]
        public async Task MarkNoteAsDeletedAndStartTimer()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "MarkNoteAsDeletedAndStartTimer()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.MarkNoteAsDeletedAndStartTimerAsync(note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.IsMarkedAsDeleted!.Should().BeTrue();
        }

        [Fact(DisplayName = "Be able to pin a note")]
        public async Task PinNote()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "PinNote()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.PinNoteAsync(note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.IsPinned!.Should().BeTrue();
        }

        [Fact(DisplayName = "Be able to attach event to a note")]
        public async Task AttachEventToNoteAsync()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Tst", "AttachEventToNote()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);
            var ev = new Event(Guid.NewGuid(), "Con", DateTime.Now, DateTime.Now.AddMinutes(2), usr);
            await _er.AddEventAsync(ev);

            // Act
            await ns.AttachEventToNoteAsync(ev.EventId, note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Event!.Should().Be(ev);
        }

        [Fact(DisplayName = "Be able to edit note content")]
        public async Task EditNoteContent()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Tst", "EditNoteContent()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.EditNoteContentAsync(note.NoteId, "new");

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Content!.Should().Be("new");
        }

        [Fact(DisplayName = "Be able to edit note title")]
        public async Task EditNoteTitle()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Tst", "EditNoteTitle()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.EditNoteTitleAsync(note.NoteId, "fresh");

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Title!.Should().Be("fresh");
        }

        [Fact(DisplayName = "Be able to change note directory")]
        public async Task ChangeNoteDirectory()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Tst", "ChangeNoteDirectory()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);
            var dir = new Directory("new", usr);
            await _dr.AddDirectoryAsync(dir);

            // Act
            await ns.ChangeNoteDirectoryAsync(note.NoteId, dir.DirectoryId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Directory!.DirectoryId.Should().Be(dir.DirectoryId);
        }
        
        [Fact(DisplayName = "Be able to change note owner")]
        public async Task ChangeNoteOwner()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Tst", "ChangeNoteDirectory()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);
            var newUser = new ApplicationUser();
            await _ur.AddUserAsync(newUser);
            note.Owner = newUser;
            // Act
            await _nr.UpdateNoteAsync(note);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Owner!.Id.Should().Be(newUser.Id);
        }
    }
}
