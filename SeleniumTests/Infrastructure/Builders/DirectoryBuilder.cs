namespace SeleniumTests.Infrastructure.Builders;

using NotesRepository.Areas.Identity.Data;
using Directory = NotesRepository.Data.Models.Directory;

public class DirectoryBuilder
{
    private Directory _directory = new Directory();

    public Directory Build() => _directory;

    public DirectoryBuilder WithDirectoryId(Guid id)
    {
        _directory.DirectoryId = id;
        return this;
    }

    public DirectoryBuilder WithName(string name)
    {
        _directory.Name = name;
        return this;
    }

    public DirectoryBuilder WithUser(ApplicationUser user)
    {
        _directory.User = user;
        return this;
    }

    public DirectoryBuilder WithParentDir(Directory directory)
    {
        _directory.ParentDir = directory;
        return this;
    }

    public DirectoryBuilder WithSubDirectories(ICollection<Directory> directories)
    {
        _directory.SubDirectories = directories;
        return this;
    }

}
