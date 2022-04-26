using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ImageRepositoryShould
    {
        private readonly ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;
        public ImageRepositoryShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("In memory database")
                .Options;
            _context = new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "User is able to add an Image in the note")]
        public async Task AddImage()
        {
            // Arrange
            var nr = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var image = new Image(null, "test image", "../resources/images", new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr)));

            // Act
            var result = await nr.AddAsync(image);

            // Assert
            var images = await _context.Images.ToListAsync();
            Assert.True(result);
            Assert.Single(images);
            await nr.DeleteAsync(image);
        }

    }
}