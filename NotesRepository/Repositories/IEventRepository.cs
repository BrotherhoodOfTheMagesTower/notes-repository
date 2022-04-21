using NotesRepository.Data.Models;

namespace NotesRepository.Repositories
{
    public interface IEventRepository
    {
        Task<ICollection<Event>> GetAllEventsAsync();
        Task<ICollection<Event>> GetAllUserEventsAsync(string userId);
        Task<Event?> GetEventByIdAsync(Guid eventId);
        Task<ICollection<Event>> GetEventsByContentAsync(string content);
        Task<ICollection<Event>> GetEventsByDateAsync(DateTime date);
        Task<bool> AddEventAsync(Event calendarEvent);
        Task<bool> UpdateEventAsync(Event calendarEvent);
        Task<bool> DeleteEventAsync(Event calendarEvent);
        Task<bool> DeleteEventsAsync(ICollection<Event> events);
        Task<bool> DeleteEventByIdAsync(Guid eventId);
    }
}
