namespace NotesRepository.Data.Models
{
    public class Image
    {
        public Image(Guid? imageId, string name, string fileUrl, Note note)
        {
            ImageId = imageId ?? Guid.NewGuid();
            Name = name;
            FileUrl = fileUrl;
            Note = note;
        }

        /// <summary>
        /// Unique ID of the image
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Name of the image
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File data of the image
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Note that the image is assigned to
        /// </summary>
        public Note Note { get; set; }
    }
}
