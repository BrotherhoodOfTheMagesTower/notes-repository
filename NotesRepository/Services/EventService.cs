using NotesRepository.Data.Models;
using NotesRepository.Repositories;

namespace NotesRepository.Services
{
    public class EventService
    {
        private readonly EventRepository _er;

        public EventService(EventRepository eventRepository)
        {
            _er = eventRepository;
        }

        public async Task<ICollection<Event>> GetAllUserEventsAsync(string userId)
            => await _er.GetAllUserEventsAsync(userId);

        public async Task<ICollection<Event>> GetEventsByContentAsync(string content)
            => await _er.GetEventsByContentAsync(content);

        public async Task<ICollection<Event>> GetEventsByStartDateAsync(DateTime date)
            => await _er.GetEventsByStartDateAsync(date);

        public async Task<Event?> GetByIdAsync(Guid eventId)
            => await _er.GetByIdAsync(eventId);

        public async Task<bool> AddAsync(Event _event)
        {
            if(_event.EndAt < _event.StartAt || _event.StartAt < DateTime.Now)
                return false;
            return await _er.AddAsync(_event);
        }

        public async Task<bool> UpdateAsync(Event _event)
            => await _er.UpdateAsync(_event);
        
        public async Task<bool> DeleteAsync(Event _event)
            => await _er.DeleteAsync(_event);

        public async Task<bool> DeleteByIdAsync(Guid eventId)
            => await _er.DeleteByIdAsync(eventId);

        public async Task<bool> DeleteManyAsync(ICollection<Guid> eventIds)
            => await _er.DeleteManyAsync(eventIds);
    }
}
