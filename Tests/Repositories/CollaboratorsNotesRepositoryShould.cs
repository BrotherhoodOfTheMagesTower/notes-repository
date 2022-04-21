//using Microsoft.EntityFrameworkCore;
//using NotesRepository.Areas.Identity.Data;
//using NotesRepository.Data;
//using NotesRepository.Data.Models;
//using NotesRepository.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;
//using FluentAssertions;

//namespace Tests
//{
//    public class CollaboratorsNotesRepositoryShould : IDbContextFactory<ApplicationDbContext>
//    {
//        private readonly ApplicationDbContext _context;
//        private DbContextOptions<ApplicationDbContext> _options;

//        public CollaboratorsNotesRepositoryShould()
//        {
//            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase("In memory database")
//                .Options;
//            _context = CreateDbContext();
//        }

//        public ApplicationDbContext CreateDbContext()
//        {
//            return new ApplicationDbContext(_options);
//        }


//        [Fact(DisplayName = "User is able to add note to a collaborator")]
//        public async Task AddNoteToCollaborator()
//        {
//            // Arrange
//            var cnr = new CollaboratorsNotesRepository(new CollaboratorsNotesRepositoryShould());
//            var collaborator = new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note", "for AddNote()", "def-ico", new ApplicationUser(), new Directory("Default")));

//            // Act
//            var result = await cnr.AddNoteToCollaboratorAsync(collaborator);

//            // Assert
//            var collaborators = await _context.CollaboratorsNotes.ToListAsync();
//            Assert.True(result);
//            Assert.Single(collaborators);
//            await cnr.DeleteNoteFromCollaboratorAsync(collaborator);

//        }
//        [Fact(DisplayName = "User is able to add multiple notes to a collaborator")]
//        public async Task AddNotesToCollaborator()
//        {
//            // Arrange
//            var cnr = new CollaboratorsNotesRepository(new CollaboratorsNotesRepositoryShould());
//            var collaborators = new List<CollaboratorsNotes>()
//            {
//                new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note 1", "for AddMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default"))),
//                new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note 2", "for AddMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default"))),
//                new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note 3", "for AddMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default")))
//            };

//            // Act
//            var result = await cnr.AddNotesToCollaboratorAsync(collaborators);

//            // Assert
//            var collaboratorFromDb = await _context.CollaboratorsNotes.ToListAsync();
//            Assert.True(result);
//            collaboratorFromDb.Should().ContainItemsAssignableTo<CollaboratorsNotes>().And.HaveCount(3);
//            await cnr.DeleteNotesFromCollaboratorAsync(collaborators);
//        }

//        [Fact(DisplayName = "User is able to delete a note from collaborator")]
//        public async Task DeleteNoteFromCollaborator()
//        {

//            // Arrange
//            var cnr = new CollaboratorsNotesRepository(new CollaboratorsNotesRepositoryShould());
//            var collaborator = new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note", "for DeleteNote()", "def-ico", new ApplicationUser(), new Directory("Default")));
//            await cnr.AddNoteToCollaboratorAsync(collaborator);
//            // Act
//            var result = await cnr.DeleteNoteFromCollaboratorAsync(collaborator);

//            // Assert
//            var collaborators = await _context.CollaboratorsNotes.ToListAsync();
//            Assert.True(result);
//            Assert.Empty(collaborators);

//        }

//        [Fact(DisplayName = "User is able to delete note from collaborator by its id and note id")]

//        public async Task DeleteNoteFromCollaboratorByIds()
//        {
//            // Arrange
//            var cnr = new CollaboratorsNotesRepository(new CollaboratorsNotesRepositoryShould());
//            var collaborator = new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note", "for DeleteNoteByIds()", "def-ico", new ApplicationUser(), new Directory("Default")));
//            await cnr.AddNoteToCollaboratorAsync(collaborator);
//            // Act
//            var result = await cnr.DeleteNoteFromCollaboratorAsync(collaborator.NoteId, collaborator.ApplicationUserId);

//            // Assert
//            var collaborators = await _context.CollaboratorsNotes.ToListAsync();
//            Assert.True(result);
//            Assert.Empty(collaborators);
//        }
//        [Fact(DisplayName = "User is able to delete multiple notes from collaborator")]
//        public async Task DeleteNotesFromCollaborator()
//        {


//            // Arrange
//            var cnr = new CollaboratorsNotesRepository(new CollaboratorsNotesRepositoryShould());
//            var collaborators = new List<CollaboratorsNotes>()
//            {
//                new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note 1", "for DeleteMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default"))),
//                new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note 2", "for DeleteMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default"))),
//                new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note(null, "Test note 3", "for DeleteMultipleNotes()", "def-ico", new ApplicationUser(), new Directory("Default")))
//            };
//            await cnr.AddNotesToCollaboratorAsync(collaborators);

//            // Act
//            var result = await cnr.DeleteNotesFromCollaboratorAsync(collaborators);

//            // Assert
//            var collaboratorsFromDb = await _context.CollaboratorsNotes.ToListAsync();
//            Assert.True(result);
//            collaboratorsFromDb.Should().BeEmpty();
//        }


//        /* [Fact(DisplayName ="User is able to get all users related to the note")]

//          public async Task GetAllUsersRelatedToTheNote()
//          {
//              // Arrange
//              var cnr = new CollaboratorsNotesRepository(new CollaboratorsNotesRepositoryShould());
//              var collaborators = new List<CollaboratorsNotes>()
//              {
//                  new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note()),
//                  new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note()),
//                  new CollaboratorsNotes(null, new Guid(), new ApplicationUser(), new Note())
//              };
//              await cnr.AddNotesToCollaboratorAsync(collaborators);

//             // Act 
//             var result = await cnr.GetAllUsersRelatedToTheNoteAsync(collaborators.NoteId);
//           }*/

//    }
//}
