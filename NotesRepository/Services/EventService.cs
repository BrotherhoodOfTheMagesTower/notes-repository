using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using NotesRepository.Services.QuartzJobs;
using Quartz;
using Quartz.Impl;

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
            if (_event.EndAt < _event.StartAt || _event.StartAt < DateTime.Now)
                return false;
            if (_event.ReminderAt is not null)
            {
                if (_event.ReminderAt < DateTime.Now)
                    return false;
                await ScheduleEventReminderAsync(_event);
            }
            return await _er.AddAsync(_event);
        }

        public async Task<bool> UpdateAsync(Event _event)
        {
            if (_event.EndAt < _event.StartAt)
                return false;
            var _evFromDb = await _er.GetByIdAsync(_event.EventId);
            if (_evFromDb is not null)
            {
                if (_event.ReminderAt is not null)
                {
                    if (_evFromDb.ReminderAt is null) //if there was no reminder previously, but now the user added it
                    {
                        if (_event.ReminderAt < DateTime.Now)
                            return false;
                        await ScheduleEventReminderAsync(_event);
                    } 
                    else
                    {
                        if (_event.ReminderAt < DateTime.Now)
                            return false;
                        if (_evFromDb.ReminderAt != _event.ReminderAt) //if the user changed the reminder date/time
                            await EditEventReminderAsync(_event);
                    }
                }
                else
                    if (_evFromDb.ReminderAt is not null) //if there was a reminder previously, but now the user removed it
                        await CancelEventReminderAsync(_event);
            }
            else return false;
            return await _er.UpdateAsync(_event);
        }

        public async Task<bool> DeleteAsync(Event _event)
            => await _er.DeleteAsync(_event);

        public async Task<bool> DeleteByIdAsync(Guid eventId)
            => await _er.DeleteByIdAsync(eventId);

        public async Task<bool> DeleteManyAsync(ICollection<Guid> eventIds)
            => await _er.DeleteManyAsync(eventIds);

        public async Task<bool> AttachNoteToEventAsync(Guid eventId, Guid noteId)
        {
            var _event = await _er.GetByIdAsync(eventId);
            if (_event is not null)
            {
                _event.NoteId = noteId;
                return await _er.UpdateAsync(_event);
            }
            return false;
        }

        public async Task<bool> DeleteNoteFromEventAsync(Guid eventId)
        {
            var _event = await _er.GetByIdAsync(eventId);
            if (_event is not null)
            {
                _event.NoteId = null;
                return await _er.UpdateAsync(_event);
            }
            return false;
        }

        private async Task ScheduleEventReminderAsync(Event _event)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            if (!scheduler.IsStarted)
                await scheduler.Start();

            IJobDetail job = JobBuilder.Create<AddEventReminder>()
                .WithIdentity(_event.EventId.ToString(), _event.User.Email)
                .Build();

            var utcReminder = DateTime.SpecifyKind((DateTime)_event.ReminderAt!, DateTimeKind.Utc);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"{_event.EventId}-trigger", _event.User.Email)
                .StartAt(utcReminder)
                .ForJob(job)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        private async Task EditEventReminderAsync(Event _event)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            if (!scheduler.IsStarted)
                await scheduler.Start();

            await scheduler.DeleteJob(new JobKey(_event.EventId.ToString()));

            IJobDetail job = JobBuilder.Create<EditEventReminder>()
                .WithIdentity(_event.EventId.ToString(), _event.User.Email)
                .Build();

            var utcReminder = DateTime.SpecifyKind((DateTime)_event.ReminderAt!, DateTimeKind.Utc);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"{_event.EventId}-trigger", _event.User.Email)
                .StartAt(utcReminder)
                .ForJob(job)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
        
        private async Task CancelEventReminderAsync(Event _event)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            if (!scheduler.IsStarted)
                await scheduler.Start();

            await scheduler.DeleteJob(new JobKey(_event.EventId.ToString()));

            IJobDetail job = JobBuilder.Create<CancelEventReminder>()
                .WithIdentity(_event.EventId.ToString(), _event.User.Email)
                .Build();

            var utcReminder = DateTime.SpecifyKind((DateTime)_event.ReminderAt!, DateTimeKind.Utc);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"{_event.EventId}-trigger", _event.User.Email)
                .StartAt(utcReminder)
                .ForJob(job)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
