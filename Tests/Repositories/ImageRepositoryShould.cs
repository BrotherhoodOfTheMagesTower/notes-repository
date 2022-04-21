using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ImageRepositoryShould : IDbContextFactory<ApplicationDbContext>
    {
        private readonly ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;
        public ImageRepositoryShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("In memory database")
                .Options;
            _context = CreateDbContext();
        }
        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "User is able to add an Image in the note")]
        public async Task AddImage()
        {
            // Arrange
            var nr = new ImageRepository(new ImageRepositoryShould());
            var image = new Image(null, "test image", "../resources/images", new Note(null, "Test note", "for AddNote()", "def-ico", new ApplicationUser(), new Directory("Default")));

            // Act
            var result = await nr.AddImageAsync(image);

            // Assert
            var images = await _context.Images.ToListAsync();
            Assert.True(result);
            Assert.Single(images);
            await nr.DeleteImageAsync(image);
        }

    }
}