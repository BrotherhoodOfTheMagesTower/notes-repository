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
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
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
            var result = await ns.GetNoteByTitleAsync("Custom", usr.Id);

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
            var notesFromAnotherUser = new List<Note>
            {
                new Note(null, "Test note1RandomUser", "Search1()", "def-ico", new ApplicationUser(), new Directory("Default", usr)),
                new Note(null, "Test note2", "Search2()RandomUser", "def-ico", new ApplicationUser(), new Directory("Default", usr)),
                new Note(null, "Test note3RandomUser", "Search3()", "def-ico", new ApplicationUser(), new Directory("Default", usr))
            };
            await ns.AddNotesAsync(notes);
            await ns.AddNotesAsync(notesFromAnotherUser);

            // Act
            var titleResult = await ns.SearchNotesByTitleAndContentAsync("te1", usr.Id);
            var contentResult = await ns.SearchNotesByTitleAndContentAsync("rch2", usr.Id);

            var noMatch1 = await ns.SearchNotesByTitleAndContentAsync("te1RandomUser", usr.Id);
            var noMatch2 = await ns.SearchNotesByTitleAndContentAsync("Search2()Random", usr.Id);

            var notMatchResult = await ns.SearchNotesByTitleAndContentAsync("foo", usr.Id);
            var multipleTitleMatchResult = await ns.SearchNotesByTitleAndContentAsync("note", usr.Id);
            var multipleContentMatchResult = await ns.SearchNotesByTitleAndContentAsync("earch", usr.Id);
            var its_A_TRAP = await ns.SearchNotesByTitleAndContentAsync("itsATrap", usr.Id);

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
            noMatch1.Should().BeEmpty();
            noMatch2.Should().BeEmpty();
        }
        
        [Fact(DisplayName = "Be able to get all single notes from bin")]
        public async Task GetAllSingleNotesFromBin()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr1 = new ApplicationUser();
            var usr2 = new ApplicationUser();
            var usr3 = new ApplicationUser();
            var notes = new List<Note>
            {
                new Note(null, "from usr1", "GetAllSingleNotesFromBin()", "def-ico", usr1, new Directory("Bin", usr1)),
                new Note(null, "from usr2", "GetAllSingleNotesFromBin()", "def-ico", usr2, new Directory("Bin", usr2)),
                new Note(null, "from usr3", "GetAllSingleNotesFromBin()", "def-ico", usr3, new Directory("Bin", usr3)),
                new Note(null, "from usr3", "GetAllSingleNotesFromBin()", "def-ico", usr3, new Directory("Bin", usr3)),
            };
            await ns.AddNotesAsync(notes);

            // Act
            var notesInBinFromUsr3 = await ns.GetAllSingleNotesFromUserThatAreCurrentlyInRecycleBinAsync(usr3.Id);

            // Assert
            Assert.NotNull(notesInBinFromUsr3);
            notesInBinFromUsr3.Should().HaveCount(2);
        }
        
        [Fact(DisplayName = "Be able to get all pinned notes from particular user")]
        public async Task GetAllPinnedNotes()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var usr1 = new ApplicationUser();
            var usr2 = new ApplicationUser();
            var usr3 = new ApplicationUser();
            var notes = new List<Note>
            {
                new Note(null, "from usr1", "GetAllPinnedNotes()", "def-ico", usr1, new Directory("Random", usr1)),
                new Note(null, "from usr2", "GetAllPinnedNotes()", "def-ico", usr2, new Directory("Random", usr2)),
                new Note(null, "from usr3 - not pinned", "GetAllPinnedNotes()", "def-ico", usr3, new Directory("Random3", usr3)),
                new Note(null, "from usr3, but pinned", "GetAllPinnedNotes()", "def-ico", usr3, new Directory("Random34", usr3)),
                new Note(null, "from usr3, but pinned", "GetAllPinnedNotes()", "def-ico", usr3, new Directory("Random35", usr3)),
            };
            notes.ElementAt(2).IsPinned = true;
            notes.ElementAt(3).IsPinned = true;
            notes.ElementAt(4).IsPinned = true;
            await ns.AddNotesAsync(notes);

            // Act
            var notesInBinFromUsr3 = await ns.GetAllPinnedNotesFromUserAsync(usr3.Id);

            // Assert
            Assert.NotNull(notesInBinFromUsr3);
            notesInBinFromUsr3.Should().HaveCount(3);
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

        [Fact(DisplayName = "Be able to move single note to bin")]
        public async Task MoveSingleNoteToBin()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "MoveToBin()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);
            await _dr.AddAsync(new Directory("Bin", usr));

            // Act
            await ns.MoveSingleNoteToBinAsync(note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.IsMarkedAsDeleted.Should().BeTrue();
            noteFromDb!.DeletedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(500));
            noteFromDb!.Directory.Name.Should().Be("Bin");
            noteFromDb!.Directory.User.Should().Be(usr);
        }
        
        [Fact(DisplayName = "Be able to restore a single note from bin to existing directory")]
        public async Task RestoreASingleNote()
        {
            //Arrange
            var ns = new NoteService(_nr, _ur, _er, _dr, _ir);
            var usr = new ApplicationUser();
            var targetDir = new Directory("TargetDir", usr);
            await _dr.AddAsync(new Directory("Bin", usr));
            await _dr.AddAsync(targetDir);
            var note = new Note(null, "Test note", "RestoreANote()", "def-ico", usr, new Directory("Some dir", usr));
            await ns.AddNoteAsync(note);

            // Act
            await ns.MoveSingleNoteToBinAsync(note.NoteId);
            await Task.Delay(1000);
            var result = await ns.RestoreASingleNoteFromTheBinAsync(note.NoteId, (await _dr.GetByIdAsync(targetDir.DirectoryId)).DirectoryId);

            // Assert
            var noteAfterRestore = await ns.GetNoteByIdAsync(note.NoteId);
            result.Should().BeTrue();

            noteAfterRestore.Should().NotBeNull();
            noteAfterRestore!.IsMarkedAsDeleted.Should().BeFalse();
            noteAfterRestore!.DeletedAt.Should().Be(null);
            noteAfterRestore!.Directory.Name.Should().Be(targetDir.Name);
            noteAfterRestore!.Directory.User.Should().Be(usr);
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
            await _dr.AddAsync(dir);

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
            await _nr.UpdateAsync(note);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Owner!.Id.Should().Be(newUser.Id);
        }
    }
}
