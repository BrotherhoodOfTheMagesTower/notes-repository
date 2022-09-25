using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;

using Directory = NotesRepository.Data.Models.Directory;

namespace SeleniumTests.Infrastructure.Builders;

public class NoteBuilder
{
    private Note _note = new Note();

    public Note Build() => _note;

    public NoteBuilder WithNoteId(Guid id)
    {
        _note.NoteId = id;
        return this;
    }

    public NoteBuilder WithTitle(string title)
    {
        _note.Title = title;
        return this;
    }

    public NoteBuilder WithContent(string content)
    {
        _note.Content = content;
        return this;
    }

    public NoteBuilder WithCreatedAt(DateTime? createdAt = null)
    {
        _note.CreatedAt = createdAt ?? DateTime.Now;
        return this;
    }

    public NoteBuilder WithEditedAt(DateTime editedAt)
    {
        _note.EditedAt = editedAt;
        return this;
    }

    public NoteBuilder WithEditedBy(ApplicationUser user)
    {
        _note.EditedBy = user;
        return this;
    }

    public NoteBuilder WithOwner(ApplicationUser user)
    {
        _note.Owner = user;
        return this;
    }

    public NoteBuilder WithDirectory(Directory directory)
    {
        _note.Directory = directory;
        return this;
    }
}
