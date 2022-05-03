using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public class CollaboratorsNotesServiceShould
    {
        private readonly CollaboratorsNotesRepository _cnr;
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly ApplicationDbContext ctx;
        private DbContextOptions<ApplicationDbContext> _options;

        public CollaboratorsNotesServiceShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            ctx = CreateDbContext();
            _cnr = new CollaboratorsNotesRepository(ctx);
            _nr = new NoteRepository(ctx);
            _ur = new UserRepository(ctx);
        }
        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "Be able to get all collaborators for note")]
        public async Task GetAllCollaborators()
        {
            //Arrange
            var cns = new CollaboratorsNotesService(_cnr, _nr, _ur);
            var usr = new ApplicationUser();
            var note = new Note
            {
                NoteId = Guid.NewGuid(),
                Title = "Test note",
                Content = "GetAllCollaborators()",
                IconName = "def-ico",
                Owner = usr,
                Directory = new Directory("default", usr)
            };
            note.CollaboratorsNotes = new List<CollaboratorsNotes>
            { new CollaboratorsNotes(new ApplicationUser(), note),
              new CollaboratorsNotes(new ApplicationUser(), note),
              new CollaboratorsNotes(new ApplicationUser(), note) };
            await _nr.AddAsync(note);

            // Act
            var result = await cns.GetAllCollaboratorsForNoteAsync(note.NoteId);

            // Assert
            Assert.NotNull(result);
            result.Should().HaveCount(3).And.Contain(note.CollaboratorsNotes.Select(u => u.Collaborator));
        }

        [Fact(DisplayName = "Be able to get all shared notes for user")]
        public async Task GetAllSharedNotes()
        {
            //Arrange
            var cns = new CollaboratorsNotesService(_cnr, _nr, _ur);
            var usr = new ApplicationUser();
            var note1 = new Note
            {
                NoteId = Guid.NewGuid(),
                Title = "Test note1",
                Content = "GetAllSharedNotes()",
                IconName = "def-ico",
                Owner = usr,
                Directory = new Directory("default", usr)
            };
            var note2 = new Note
            {
                NoteId = Guid.NewGuid(),
                Title = "Test note2",
                Content = "GetAllSharedNotes()",
                IconName = "def-ico",
                Owner = usr,
                Directory = new Directory("default", usr)
            };
            note1.CollaboratorsNotes = new List<CollaboratorsNotes>
            { new CollaboratorsNotes(usr, note1) };
            note2.CollaboratorsNotes = new List<CollaboratorsNotes>
            { new CollaboratorsNotes(usr, note2) };

            var notes = new List<Note> { note1, note2 };
            await _nr.AddManyAsync(notes);

            // Act
            var result = await cns.GetAllSharedNotesForUserAsync(usr.Id);

            // Assert
            Assert.NotNull(result);
            result.Should().HaveCount(2).And.Contain(note1).And.Contain(note2);
        }
        
        [Fact(DisplayName = "Be able to get add collaborator to note")]
        public async Task AddCollaboratorToNote()
        {
            //Arrange
            var cns = new CollaboratorsNotesService(_cnr, _nr, _ur);
            var usr = new ApplicationUser();
            var note = new Note
            {
                NoteId = Guid.NewGuid(),
                Title = "Test note",
                Content = "AddCollaboratorToNote()",
                IconName = "def-ico",
                Owner = usr,
                Directory = new Directory("default", usr)
            };
            await _nr.AddAsync(note);

            // Act
            var result = await cns.AddCollaboratorToNoteAsync(usr.Id, note.NoteId);

            // Assert
            Assert.True(result);
            var collaborators = await cns.GetAllCollaboratorsForNoteAsync(note.NoteId);
            collaborators.Should().Contain(usr);
        }
        
        [Fact(DisplayName = "Be able to add collaborators to note")]
        public async Task AddCollaboratorsToNote()
        {
            //Arrange
            var cns = new CollaboratorsNotesService(_cnr, _nr, _ur);
            var usr = new ApplicationUser();
            var usr1 = new ApplicationUser();
            var usr2 = new ApplicationUser();
            var note = new Note
            {
                NoteId = Guid.NewGuid(),
                Title = "Test note",
                Content = "AddCollaboratorsToNote()",
                IconName = "def-ico",
                Owner = usr,
                Directory = new Directory("default", usr)
            };
            await _nr.AddAsync(note);
            await _ur.AddUserAsync(usr1);
            await _ur.AddUserAsync(usr2);

            // Act
            var result = await cns.AddCollaboratorsToNoteAsync(new List<string> { usr1.Id, usr2.Id }, note.NoteId);

            // Assert
            Assert.True(result);
            var collaborators = await cns.GetAllCollaboratorsForNoteAsync(note.NoteId);
            collaborators.Should().Contain(usr1).And.Contain(usr2);
        }
        
        [Fact(DisplayName = "Be able to delete a collaborator from note")]
        public async Task DeleteCollaboratorFromNote()
        {
            //Arrange
            var cns = new CollaboratorsNotesService(_cnr, _nr, _ur);
            var usr = new ApplicationUser();
            var note = new Note
            {
                NoteId = Guid.NewGuid(),
                Title = "Test note",
                Content = "DeleteCollaboratorFromNote()",
                IconName = "def-ico",
                Owner = usr,
                Directory = new Directory("default", usr)
            };
            note.CollaboratorsNotes = new List<CollaboratorsNotes> { new CollaboratorsNotes(usr, note) };
            await _nr.AddAsync(note);

            // Act
            var result = await cns.DeleteCollaboratorFromNoteAsync(usr.Id, note.NoteId);

            // Assert
            Assert.True(result);
            var collaborators = await cns.GetAllCollaboratorsForNoteAsync(note.NoteId);
            collaborators.Should().BeEmpty();
        }
        
        [Fact(DisplayName = "Be able to delete a collaborators from note")]
        public async Task DeleteCollaboratorsFromNote()
        {
            //Arrange
            var cns = new CollaboratorsNotesService(_cnr, _nr, _ur);
            var usr = new ApplicationUser();
            var usr1 = new ApplicationUser();
            var note = new Note
            {
                NoteId = Guid.NewGuid(),
                Title = "Test note",
                Content = "DeleteCollaboratorFromNote()",
                IconName = "def-ico",
                Owner = usr,
                Directory = new Directory("default", usr)
            };
            note.CollaboratorsNotes = new List<CollaboratorsNotes> { new CollaboratorsNotes(usr, note), new CollaboratorsNotes(usr1, note) };
            await _nr.AddAsync(note);

            // Act
            var result = await cns.DeleteCollaboratorsFromNoteAsync(new List<string> { usr.Id, usr1.Id }, note.NoteId);

            // Assert
            Assert.True(result);
            var collaborators = await cns.GetAllCollaboratorsForNoteAsync(note.NoteId);
            collaborators.Should().BeEmpty();
        }
    }
}
