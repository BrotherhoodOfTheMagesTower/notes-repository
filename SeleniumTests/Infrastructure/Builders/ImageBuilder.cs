using NotesRepository.Data.Models;

namespace SeleniumTests.Infrastructure.Builders;

public class ImageBuilder
{
    private Image _image = new();

    public Image Build() => _image;

    public ImageBuilder WithImageId(Guid imageId)
    {
        _image.ImageId = imageId;
        return this;
    }
    
    public ImageBuilder WithName(string name)
    {
        _image.Name = name;
        return this;
    }
    
    public ImageBuilder WithFileUrl(string fileUrl)
    {
        _image.FileUrl = fileUrl;
        return this;
    }
    
    public ImageBuilder WithNote(Note note)
    {
        _image.Note = note;
        return this;
    }
}
