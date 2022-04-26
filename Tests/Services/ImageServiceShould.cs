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
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public class ImageServiceShould
    {
        private readonly ImageRepository _ir;
        private readonly ApplicationDbContext ctx;
        private DbContextOptions<ApplicationDbContext> _options;

        public ImageServiceShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            ctx = CreateDbContext();
            _ir = new ImageRepository(ctx);
        }
        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "Be able to get a image by ID")]
        public async Task GetImageById()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var image = new Image
                (null, "TestIMG", "www.zmitac.com", new Note(null, "DefNote", "", "", usr, new Directory("def", usr)));
            await ims.AddImageAsync(image);

            // Act
            var result = await ims.GetImageByIdAsync(image.ImageId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result!.ImageId, image.ImageId);
        }
        
        [Fact(DisplayName = "Be able to get a image by URL")]
        public async Task GetImageByURL()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var image = new Image
                (null, "TestIMG", "www.zmitac.com", new Note(null, "DefNote", "", "", usr, new Directory("def", usr)));
            await ims.AddImageAsync(image);

            // Act
            var result = await ims.GetImageByUrlAsync("www.zmitac.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result!.ImageId, image.ImageId);
        }
        
        [Fact(DisplayName = "Be able to get all note images")]
        public async Task GetAllNoteImages()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var note = new Note(null, "DefNote", "", "", usr, new Directory("def", usr));
            var imagesToAdd = new List<Image>
            {
                new Image(null, "TestIMG1", "www.zmitac.com", note),
                new Image(null, "TestIMG2", "www.zmitac.com", note)
            };
            await ims.AddImagesAsync(imagesToAdd);

            // Act
            var result = await ims.GetAllNoteImagesAsync(note.NoteId);

            // Assert
            Assert.NotNull(result);
            result.Should().HaveCount(2).And.Contain(imagesToAdd);
        }
        
        [Fact(DisplayName = "Be able to get all user images")]
        public async Task GetAllUserImages()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var not1 = new Note(null, "DefNote1", "", "", usr, new Directory("def", usr));
            var not2 = new Note(null, "DefNote2", "", "", usr, new Directory("def", usr));
            var imagesToAdd = new List<Image>
            {
                new Image(null, "TestIMG1", "www.zmitac.com", not1),
                new Image(null, "TestIMG2", "www.zmitac.com", not2)
            };
            await ims.AddImagesAsync(imagesToAdd);

            // Act
            var result = await ims.GetAllUserImagesAsync(usr.Id);

            // Assert
            Assert.NotNull(result);
            result.Should().HaveCount(2).And.Contain(imagesToAdd);
        }
        
        [Fact(DisplayName = "Be able to update image")]
        public async Task UpdateImage()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var not1 = new Note(null, "DefNote1", "", "", usr, new Directory("def", usr));
            var image = new Image(null, "TestIMG1", "www.zmitac.com", not1);
            await ims.AddImageAsync(image);
            image.Name = "Edited Name";

            // Act
            var result = await ims.UpdateImageAsync(image);

            // Assert
            Assert.True(result);
            var img = await ims.GetImageByIdAsync(image.ImageId);
            img.Should().NotBeNull();
            img!.Name.Should().Be(image.Name);
        }
        
        [Fact(DisplayName = "Be able to delete image")]
        public async Task DeleteImage()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var not1 = new Note(null, "DefNote1", "", "", usr, new Directory("def", usr));
            var image = new Image(null, "TestIMG1", "www.zmitac.com", not1);
            await ims.AddImageAsync(image);

            // Act
            var result = await ims.DeleteImageAsync(image);

            // Assert
            Assert.True(result);
            var img = await ims.GetImageByIdAsync(image.ImageId);
            img.Should().BeNull();
        }
        
        [Fact(DisplayName = "Be able to delete image by ID")]
        public async Task DeleteImageById()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var not1 = new Note(null, "DefNote1", "", "", usr, new Directory("def", usr));
            var image = new Image(null, "TestIMG1", "www.zmitac.com", not1);
            await ims.AddImageAsync(image);

            // Act
            var result = await ims.DeleteImageAsync(image);

            // Assert
            Assert.True(result);
            var img = await ims.GetImageByIdAsync(image.ImageId);
            img.Should().BeNull();
        }

        [Fact(DisplayName = "Be able to delete many images")]
        public async Task DeleteManyImages()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var not1 = new Note(null, "DefNote1", "", "", usr, new Directory("def", usr));
            var imagesToDelete = new List<Image>
            {
                new Image(null, "TestIMG1", "www.zmitac.com", not1),
                new Image(null, "TestIMG2", "www.zmitac.com", not1)
            };
            await ims.AddImagesAsync(imagesToDelete);

            // Act
            var result = await ims.DeleteImagesAsync(imagesToDelete);

            // Assert
            Assert.True(result);
            var img1 = await ims.GetImageByIdAsync(imagesToDelete.First().ImageId);
            var img2 = await ims.GetImageByIdAsync(imagesToDelete.Last().ImageId);
            img1.Should().BeNull();
            img2.Should().BeNull();
        }
        
        [Fact(DisplayName = "Be able to set image URL")]
        public async Task SetImageUrl()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var not1 = new Note(null, "DefNote1", "", "", usr, new Directory("def", usr));
            var image = new Image(null, "TestIMG1", "default.com", not1);
            await ims.AddImageAsync(image);

            // Act
            var result = await ims.SetImageUrl("newurl.com", image.ImageId);

            // Assert
            Assert.True(result);
            var img = await ims.GetImageByIdAsync(image.ImageId);
            img.Should().NotBeNull();
            img!.FileUrl.Should().Be("newurl.com");
        }
        
        [Fact(DisplayName = "Be able to set image name")]
        public async Task SetImageName()
        {
            //Arrange
            var ims = new ImageService(_ir);
            var usr = new ApplicationUser();
            var not1 = new Note(null, "DefNote1", "", "", usr, new Directory("def", usr));
            var image = new Image(null, "TestIMG1", "default.com", not1);
            await ims.AddImageAsync(image);

            // Act
            var result = await ims.SetImageName("Fancy image", image.ImageId);

            // Assert
            Assert.True(result);
            var img = await ims.GetImageByIdAsync(image.ImageId);
            img.Should().NotBeNull();
            img!.Name.Should().Be("Fancy image");
        }
    }
}
