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

namespace Tests.Repositories
{
    public class CollaboratorsNotesRepositoryShould
    {
        private readonly ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;

        public CollaboratorsNotesRepositoryShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("In memory database - CollaboratorsNotes")
                .Options;
            _context = new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "User is able to add note to a collaborator")]
        public async Task AddNoteToCollaborator()
        {
            // Arrange
            var cnr = new CollaboratorsNotesRepository(_context);
            var usr = new ApplicationUser();
            var collaborator = new CollaboratorsNotes(usr, new Note(null, "Test note", "for AddNote()", "def-ico", new ApplicationUser(), new Directory("Default", usr)));

            // Act
            var result = await cnr.AddCollaboratorToNoteAsync(collaborator);

            // Assert
            var collaborators = await _context.CollaboratorsNotes.ToListAsync();
            Assert.True(result);
            Assert.Single(collaborators);
            await cnr.DeleteCollaboratorFromNoteAsync(collaborator);

        }
        [Fact(DisplayName = "User is able to add multiple notes to a collaborator")]
        public async Task AddNotesToCollaborator()
        {
            // Arrange
            var cnr = new CollaboratorsNotesRepository(_context);
            var usr = new ApplicationUser();
            var collaborators = new List<CollaboratorsNotes>()
            {
                new CollaboratorsNotes(usr, new Note(null, "Test note 1", "for AddMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default", usr))),
                new CollaboratorsNotes(usr, new Note(null, "Test note 2", "for AddMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default", usr))),
                new CollaboratorsNotes(usr, new Note(null, "Test note 3", "for AddMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default", usr)))
            };

            // Act
            var result = await cnr.AddCollaboratorsToNoteAsync(collaborators);

            // Assert
            var collaboratorFromDb = await _context.CollaboratorsNotes.ToListAsync();
            Assert.True(result);
            collaboratorFromDb.Should().ContainItemsAssignableTo<CollaboratorsNotes>().And.HaveCount(3);
            await cnr.DeleteCollaboratorsFromNoteAsync(collaborators);
        }

        [Fact(DisplayName = "User is able to delete a note from collaborator")]
        public async Task DeleteNoteFromCollaborator()
        {
            // Arrange
            var cnr = new CollaboratorsNotesRepository(_context);
            var usr = new ApplicationUser();
            var collaborator = new CollaboratorsNotes(usr, new Note(null, "Test note", "for DeleteNote()", "def-ico", new ApplicationUser(), new Directory("Default", usr)));
            await cnr.AddCollaboratorToNoteAsync(collaborator);
            // Act
            var result = await cnr.DeleteCollaboratorFromNoteAsync(collaborator);

            // Assert
            var collaborators = await _context.CollaboratorsNotes.ToListAsync();
            Assert.True(result);
            Assert.Empty(collaborators);

        }

        [Fact(DisplayName = "User is able to delete note from collaborator by its id and note id")]

        public async Task DeleteNoteFromCollaboratorByIds()
        {
            // Arrange
            var cnr = new CollaboratorsNotesRepository(_context);
            var usr = new ApplicationUser();
            var collaborator = new CollaboratorsNotes(new ApplicationUser(), new Note(null, "Test note", "for DeleteNoteByIds()", "def-ico", new ApplicationUser(), new Directory("Default", usr)));
            await cnr.AddCollaboratorToNoteAsync(collaborator);
            // Act
            var result = await cnr.DeleteCollaboratorFromNoteAsync(collaborator.NoteId, collaborator.ApplicationUserId);

            // Assert
            var collaborators = await _context.CollaboratorsNotes.ToListAsync();
            Assert.True(result);
            Assert.Empty(collaborators);
        }
        [Fact(DisplayName = "User is able to delete multiple notes from collaborator")]
        public async Task DeleteNotesFromCollaborator()
        {
            // Arrange
            var cnr = new CollaboratorsNotesRepository(_context);
            var usr = new ApplicationUser();
            var collaborators = new List<CollaboratorsNotes>()
            {
                new CollaboratorsNotes(usr, new Note(null, "Test note 1", "for DeleteMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default", usr))),
                new CollaboratorsNotes(usr, new Note(null, "Test note 2", "for DeleteMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default", usr))),
                new CollaboratorsNotes(usr, new Note(null, "Test note 3", "for DeleteMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default", usr)))
            };
            await cnr.AddCollaboratorsToNoteAsync(collaborators);

            // Act
            var result = await cnr.DeleteCollaboratorsFromNoteAsync(collaborators);

            // Assert
            var collaboratorsFromDb = await _context.CollaboratorsNotes.ToListAsync();
            Assert.True(result);
            collaboratorsFromDb.Should().BeEmpty();
        }


        /* [Fact(DisplayName ="User is able to get all users related to the note")]

          public async Task GetAllUsersRelatedToTheNote()
          {
              // Arrange
              var cnr = new CollaboratorsNotesRepository(_context);
              var collaborators = new List<CollaboratorsNotes>()
              {
                  new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note()),
                  new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note()),
                  new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note())
              };
              await cnr.AddNotesToCollaboratorAsync(collaborators);

             // Act 
             var result = await cnr.GetAllUsersRelatedToTheNoteAsync(collaborators.NoteId);
           }*/

    }
}
