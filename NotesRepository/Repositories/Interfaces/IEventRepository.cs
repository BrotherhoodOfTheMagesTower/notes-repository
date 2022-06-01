using NotesRepository.Data.Models;

namespace NotesRepository.Repositories.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<ICollection<Event>> GetAllUserEventsAsync(string userId);
        Task<ICollection<Event>> GetEventsByContentAsync(string content);
        Task<ICollection<Event>> GetEventsByStartDateAsync(DateTime date);
        Task<ICollection<Event>> GetIncomingEventsAsync(int eventCount, string userId);
        Task<bool> DeleteManyAsync(ICollection<Guid> eventIds);
    }
}
