using Microsoft.EntityFrameworkCore;
using NotesRepository.Data;
using NotesRepository.Data.Models;

namespace NotesRepository.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext ctx;

        public ImageRepository(ApplicationDbContext context)
        {
            ctx = context;
        }

        /// <summary>
        /// Adds image to the database
        /// </summary>
        /// <param name="image">The image entity</param>
        /// <returns>True if image was successfully added, otherwise false.</returns>
        public async Task<bool> AddImageAsync(Image image)
        {
            await ctx.Images.AddAsync(image);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Adds multiple images to the database.
        /// </summary>
        /// <param name="images">Images to be added to the database.</param>
        /// <returns>True if images were successfully added, otherwise false.</returns>
        public async Task<bool> AddImagesAsync(ICollection<Image> images)
        {
            await ctx.Images.AddRangeAsync(images);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Method removes image from database.
        /// </summary>
        /// <param name="image">Image to be deleted from DB.</param>
        /// <returns>True if image was successfully deleted, otherwise false.</returns>
        public async Task<bool> DeleteImageAsync(Image image)
        {
            ctx.Images.Remove(image);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Method removes image from database.
        /// </summary>
        /// <param name="imageId">Id of image to be deleted from DB.</param>
        /// <returns>True if image was successfully deleted, otherwise false.</returns>
        public async Task<bool> DeleteImageByIdAsync(Guid imageId)
        {
            var image = await ctx.Images.FirstOrDefaultAsync(x => x.ImageId == imageId);
            if (image is not null)
            {
                ctx.Images.Remove(image);
                var result = await ctx.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Removes multiple Images from DB.
        /// </summary>
        /// <param name="images">Images to be removed.</param>
        /// <returns>True if images were successfully deleted, otherwise false.</returns>
        public async Task<bool> DeleteImagesAsync(ICollection<Image> images)
        {
            ctx.Images.RemoveRange(images);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Method gets all images from DB.
        /// </summary>
        /// <returns>all images</returns>
        public async Task<ICollection<Image>> GetAllImagesAsync()
        {
            return await ctx.Images
                .Include(n => n.Note)
                .ToListAsync();
        }

        /// <summary>
        /// Method gets all images that are in a note
        /// </summary>
        /// <param name="note">note from which to return images</param>
        /// <returns>images from a chosen note</returns>
        public async Task<ICollection<Image>> GetAllNoteImages(Note note)
        {
            return await ctx.Images
                .Include(d => d.Note)
                .Where(n => n.Note == note)
                .ToListAsync();
        }

        /// <summary>
        /// Method gets all user images from the DB
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>user images</returns>
        public async Task<ICollection<Image>> GetAllUserImagesAsync(string userId)
        {
            return await ctx.Images
                .Include(d => d.Note)
                .Include(d => d.Note.Owner)
                .Where(n => n.Note.Owner.Id == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Method gets image from DB by image id
        /// </summary>
        /// <param name="imageId">id of image</param>
        /// <returns>Image if exists, otherwise null</returns>
        public async Task<Image?> GetImageByIdAsync(Guid imageId)
        {
            return await ctx.Images
                .Include(n => n.Note)
                .FirstOrDefaultAsync(i => i.ImageId == imageId);
        }

        /// <summary>
        /// Method gets Image from DB by image file URL.
        /// </summary>
        /// <param name="imageUrl">image file path</param>
        /// <returns>Image if exists, otherwise null.</returns>
        public async Task<Image?> GetImageByUrlAsync(string imageUrl)
        {
            return await ctx.Images
                .Include(n => n.Note)
                .FirstOrDefaultAsync(i => i.FileUrl == imageUrl);
        }

        /// <summary>
        /// Method updates image in the DB.
        /// </summary>
        /// <param name="image">Image to be updated</param>
        /// <returns>True if image was successfully updated, otherwise false.</returns>
        public async Task<bool> UpdateImageAsync(Image image)
        {
            ctx.Images.Update(image);
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }
    }
}