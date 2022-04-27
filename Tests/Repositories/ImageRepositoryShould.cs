using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using System.Collections.Generic;
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

        [Fact(DisplayName = "User is able to add multiple images in the note")]
        public async Task AddMultipleImages()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var images = new List<Image>()
            {
                new Image(null, "test image1", "../resources/images", new Note(null, "Test note1", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
                new Image(null, "test image2", "../resources/images", new Note(null, "Test note2", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
                new Image(null, "test image3", "../resources/images", new Note(null, "Test note3", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
                new Image(null, "test image4", "../resources/images", new Note(null, "Test note4", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
            };

            //Act
            var result = await imageRepository.AddManyAsync(images);

            //Assert
            var imagesFromDb = await _context.Images.ToListAsync();
            Assert.True(result);
            imagesFromDb.Should().ContainItemsAssignableTo<Image>().And.HaveCount(4);
            await imageRepository.DeleteManyAsync(images);
        }

        [Fact(DisplayName = "User is able to delete Image")]
        public async Task DeleteImage()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var image = new Image(null, "test image", "../resources/images", new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr)));
            await imageRepository.AddAsync(image);

            //Act
            var result = await imageRepository.DeleteAsync(image);

            //Assert
            var imagesFromDb = await _context.Images.ToListAsync();
            Assert.True(result);
            Assert.Empty(imagesFromDb);
        }

        [Fact(DisplayName = "User is able to delete Image by Id")]
        public async Task DeleteImageById()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var image = new Image(null, "test image", "../resources/images", new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr)));
            await imageRepository.AddAsync(image);

            //Act
            var result = await imageRepository.DeleteByIdAsync(image.ImageId);

            //Assert
            var imagesFromDb = await _context.Images.ToListAsync();
            Assert.True(result);
            Assert.Empty(imagesFromDb);
        }

        [Fact(DisplayName = "User is able to delete many Images")]
        public async Task DeleteManyImages()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var images = new List<Image>() {
                new Image(null, "test image1", "../resources/images", new Note(null, "Test note1", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
                new Image(null, "test image2", "../resources/images", new Note(null, "Test note2", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
                new Image(null, "test image3", "../resources/images", new Note(null, "Test note3", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
                new Image(null, "test image4", "../resources/images", new Note(null, "Test note4", "for AddNote()", "def-ico", usr, new Directory("Default", usr))),
                new Image(null, "test image5", "../resources/images", new Note(null, "Test note5", "for AddNote()", "def-ico", usr, new Directory("Default", usr)))
                 };
            await imageRepository.AddManyAsync(images);

            //Act
            var result = await imageRepository.DeleteManyAsync(images);

            //Assert
            var imagesFromDb = await _context.Images.ToListAsync();
            Assert.True(result);
            Assert.Empty(imagesFromDb);
        }

        [Fact(DisplayName = "User is able to get all Note Images")]
        public async Task GetImagesFromNote()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr));

            var imagesList = new List<Image>()
            {
                new Image(null, "test image1", "../resources/images", note),
                new Image(null, "test image2", "../resources/images", note),
                new Image(null, "test image3", "../resources/images", note),
            };

            await imageRepository.AddManyAsync(imagesList);

            //Act
            var result = await imageRepository.GetAllNoteImagesAsync(note.NoteId);

            //Assert
            result.Should().AllBeAssignableTo<Image>();
            result.Should().HaveCount(3);
            foreach (var image in result)
            {
                Assert.True(image.Note == note);
            }
        }

        [Fact(DisplayName = "User is able to get all Images")]
        public async Task GetAllImages()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var note = new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr));
            var note2 = new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr));
            var note3 = new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr));

            var imagesList = new List<Image>()
            {
                new Image(null, "test image1", "../resources/images", note),
                new Image(null, "test image2", "../resources/images", note),
                new Image(null, "test image3", "../resources/images", note2),
                new Image(null, "test image3", "../resources/images", note2),
                new Image(null, "test image3", "../resources/images", note3),
                new Image(null, "test image3", "../resources/images", note3),
                new Image(null, "test image3", "../resources/images", note3),
            };

            await imageRepository.AddManyAsync(imagesList);

            //Act
            var result = await imageRepository.GetAllUserImagesAsync(usr.Id);

            //Assert
            await imageRepository.DeleteManyAsync(imagesList);
            Assert.NotNull(result);
            result.Should().AllBeAssignableTo<Image>();
            result.Should().HaveCount(7);
            result.Should().Contain(x => x.Note.Owner.Id == usr.Id);
        }

        [Fact(DisplayName = "User is able to get Image by Id")]
        public async Task GetImageById()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var image = new Image(null, "test image", "../resources/images", new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr)));
            await imageRepository.AddAsync(image);

            //Act
            var result = await imageRepository.GetByIdAsync(image.ImageId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Image>();
            if (result != null)
            {
                Assert.Equal(image.ImageId, result.ImageId);
            }
        }

        [Fact(DisplayName = "User is able to get Image by URL")]
        public async Task GetImageByURL()
        {
            //Arrange
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var image = new Image(null, "test image", "../resources/images", new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr)));
            await imageRepository.AddAsync(image);

            //Act
            var result = await imageRepository.GetImageByUrlAsync(image.FileUrl);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Image>();
            if (result != null)
            {
                Assert.Equal(image.ImageId, result.ImageId);
            }
        }

        [Fact(DisplayName = "User is able to update Image")]
        public async Task UpdateImage()
        {
            var imageRepository = new ImageRepository(_context);
            var usr = new ApplicationUser();
            var image = new Image(null, "test image", "../resources/images", new Note(null, "Test note", "for AddNote()", "def-ico", usr, new Directory("Default", usr)));
            await imageRepository.AddAsync(image);

            //Act
            image.Name = "changed name";
            var result = await imageRepository.UpdateAsync(image);

            //Assert
            var updatedImage = await imageRepository.GetImageByUrlAsync(image.FileUrl);
            Assert.True(result);
            if (updatedImage != null)
            {
                Assert.Equal(updatedImage.Name, image.Name);
            }
        }
    }
}