using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;

namespace SeleniumTests.Infrastructure.Builders;

public class EventBuilder
{
    private Event _event = new Event();

    public Event Build() => _event;

    public EventBuilder WithEventId(Guid id)
    {
        _event.EventId = id;
        return this;
    }

    public EventBuilder WithContent(string content)
    {
        _event.Content = content;
        return this;
    }
    
    public EventBuilder WithStartAt(DateTime startAt)
    {
        _event.StartAt = startAt;
        return this;
    }
    
    public EventBuilder WithEndAt(DateTime endAt)
    {
        _event.EndAt = endAt;
        return this;
    }
    
    public EventBuilder WithUser(ApplicationUser user)
    {
        _event.User = user;
        return this;
    }
    
    public EventBuilder WithReminderAt(DateTime reminderAt)
    {
        _event.ReminderAt = reminderAt;
        return this;
    }
    
    public EventBuilder WithNote(Note note)
    {
        _event.Note = note;
        return this;
    }
}
