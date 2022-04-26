using NotesRepository.Data.Models;

namespace NotesRepository.Repositories.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<ICollection<Event>> GetAllUserEventsAsync(string userId);
        Task<ICollection<Event>> GetEventsByContentAsync(string content);
        Task<ICollection<Event>> GetEventsByDateAsync(DateTime date);
        Task<bool> DeleteManyAsync(ICollection<Event> events);
    }
}
