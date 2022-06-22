using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using NotesRepository.Data;
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

        /// <summary>
        /// Gets all user events
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with events for particular user</returns>
        public async Task<ICollection<Event>> GetAllUserEventsAsync(string userId)
            => await _er.GetAllUserEventsAsync(userId);

        /// <summary>
        /// Gets all events by content
        /// </summary>
        /// <param name="content">Content of the event</param>
        /// <returns>A collection with events with particular content</returns>
        public async Task<ICollection<Event>> GetEventsByContentAsync(string content)
            => await _er.GetEventsByContentAsync(content);

        /// <summary>
        /// Gets events by start date
        /// </summary>
        /// <param name="date">The start date of event</param>
        /// <returns></returns>
        public async Task<ICollection<Event>> GetEventsByStartDateAsync(DateTime date)
            => await _er.GetEventsByStartDateAsync(date);

        /// <summary>
        /// Gets all upcoming events for particular user
        /// </summary>
        /// <param name="eventCount">Amount of events to return</param>
        /// <param name="userId">ID of the user</param>
        /// <returns>A collection with upcoming events</returns>
        public async Task<ICollection<Event>> GetIncomingEventsAsync(int eventCount, string userId)
            => await _er.GetIncomingEventsAsync(eventCount, userId);

        /// <summary>
        /// Gets the event entity by ID
        /// </summary>
        /// <param name="eventId">ID of the event</param>
        /// <returns>The event entity</returns>
        public async Task<Event?> GetByIdAsync(Guid eventId)
            => await _er.GetByIdAsync(eventId);

        /// <summary>
        /// Adds the event entity to the database
        /// </summary>
        /// <param name="_event">The event entity</param>
        /// <returns>True if successfully added; otherwise false</returns>
        public async Task<bool> AddAsync(Event _event)
        {
            if (_event.EndAt < _event.StartAt || _event.StartAt < DateTime.Now)
                return false;
            if (_event.ReminderAt is not null && _event.ReminderAt > _event.StartAt)
                return false;
            if (_event.ReminderAt is not null)
            {
                if (_event.ReminderAt < DateTime.Now)
                    return false;
                await ScheduleEventReminderAsync(_event);
            }
            return await _er.AddAsync(_event);
        }

        /// <summary>
        /// Updates the event entity
        /// </summary>
        /// <param name="_event">The event entity</param>
        /// <returns>True if successfully updated; otherwise false</returns>
        public async Task<bool> UpdateAsync(Event _event)
        {
            if (_event.EndAt < _event.StartAt || _event.StartAt < DateTime.Now)
                return false;
            if (_event.ReminderAt is not null && _event.ReminderAt > _event.StartAt)
                return false;
            var _evFromDb = await _er.GetByIdAsync(_event.EventId);
            if(_evFromDb is not null)
            {
                if (_evFromDb.ReminderAt is not null && _event.ReminderAt is null && _evFromDb.ReminderAt > DateTime.Now)
                {
                    await CancelEventReminderAsync(_event);
                } else if (_evFromDb.ReminderAt != _event.ReminderAt)
                {
                    await EditEventReminderAsync(_event);
                }
            }
            return await _er.UpdateAsync(_event);
        }

        /// <summary>
        /// Deletes the event entity
        /// </summary>
        /// <param name="_event">The event entity</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteAsync(Event _event)
        {
            if (_event.ReminderAt is not null && _event.ReminderAt > DateTime.Now)
            {
                await UnscheduleEventReminder(_event);
            }
            return await _er.DeleteAsync(_event);
        }

        /// <summary>
        /// Deletes the event entity
        /// </summary>
        /// <param name="eventId">ID of the event</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteByIdAsync(Guid eventId)
        {
            var _event = _er.GetById(eventId);
            if (_event is not null && _event.ReminderAt is not null && _event.ReminderAt > DateTime.Now)
            {
                await UnscheduleEventReminder(_event);
            }
            return await _er.DeleteByIdAsync(eventId);
        }

        /// <summary>
        /// Deletes event entities
        /// </summary>
        /// <param name="eventIds">IDs of the events</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
        public async Task<bool> DeleteManyAsync(ICollection<Guid> eventIds)
            => await _er.DeleteManyAsync(eventIds);

        /// <summary>
        /// Attached the note to event
        /// </summary>
        /// <param name="eventId">ID of the event</param>
        /// <param name="noteId">ID of the note</param>
        /// <returns>True if successfully attached; otherwise false</returns>
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

        /// <summary>
        /// Deletes the note from event
        /// </summary>
        /// <param name="eventId">ID of the event</param>
        /// <returns>True if successfully deleted; otherwise false</returns>
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

        /// <summary>
        /// Schedules an event reminder
        /// </summary>
        /// <param name="_event">The event entity</param>
        private async Task ScheduleEventReminderAsync(Event _event)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<AddEventReminder>();
            #if DEBUG
            serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=localhost,1433;Database=notes;User ID=sa;Password=MyPassword1234!;Integrated Security=false"));
            #else
            serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=tcp:notesrepository.database.windows.net,1433;Initial Catalog=notesrepositorydb;Persist Security Info=False;User ID=brotherhood;Password=MyPassword1234!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
            #endif
            var keyVaultEndpoint = new Uri("https://noterepo.vault.azure.net/");
            var builder = new ConfigurationBuilder();
            var conf = builder.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential()).Build();
            serviceCollection.AddSingleton<IConfiguration>(conf);

            var container = serviceCollection.BuildServiceProvider();
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            scheduler.JobFactory = new JobFactory(container, container.GetRequiredService<IConfiguration>());

            if (!scheduler.IsStarted)
                await scheduler.Start();

            IJobDetail job = JobBuilder.Create<AddEventReminder>()
                .WithIdentity(_event.EventId.ToString(), _event.User.Email)
                .Build();

            var utcReminder = DateTime.SpecifyKind((DateTime)_event.ReminderAt!, DateTimeKind.Local);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"{_event.EventId}-trigger", _event.User.Email)
                .StartAt(utcReminder)
                .ForJob(job)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Edits an event reminder: unschedules the previous reminder & schedules a new
        /// </summary>
        /// <param name="_event">The event entity</param>
        public async Task EditEventReminderAsync(Event _event)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            if (!scheduler.IsStarted)
                await scheduler.Start();

            await scheduler.UnscheduleJob(new TriggerKey($"{_event.EventId}-trigger", _event.User.Email));
            await ScheduleEventReminderAsync(_event);
        }

        /// <summary>
        /// Unschedules an event reminder
        /// </summary>
        /// <param name="_event">The event entity</param>
        private async Task UnscheduleEventReminder(Event _event)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            if (!scheduler.IsStarted)
                await scheduler.Start();

            await scheduler.UnscheduleJob(new TriggerKey($"{_event.EventId}-trigger", _event.User.Email));
        }

        /// <summary>
        /// Canceles an event reminder
        /// </summary>
        /// <param name="_event">The event entity</param>
        public async Task CancelEventReminderAsync(Event _event)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<CancelEventReminder>();
            #if DEBUG
            serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=localhost,1433;Database=notes;User ID=sa;Password=MyPassword1234!;Integrated Security=false"));
            #else
            serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=tcp:notesrepository.database.windows.net,1433;Initial Catalog=notesrepositorydb;Persist Security Info=False;User ID=brotherhood;Password=MyPassword1234!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
            #endif
            var keyVaultEndpoint = new Uri("https://noterepo.vault.azure.net/");
            var builder = new ConfigurationBuilder();
            var conf = builder.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential()).Build();
            serviceCollection.AddSingleton<IConfiguration>(conf);

            var container = serviceCollection.BuildServiceProvider();
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            scheduler.JobFactory = new JobFactory(container, container.GetRequiredService<IConfiguration>());

            if (!scheduler.IsStarted)
                await scheduler.Start();

            await scheduler.UnscheduleJob(new TriggerKey($"{_event.EventId}-trigger", _event.User.Email));

            IJobDetail job = JobBuilder.Create<CancelEventReminder>()
                .WithIdentity($"Cancel_{_event.EventId}", _event.User.Email)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"Cancel_{_event.EventId}-trigger", _event.User.Email)
                .StartNow()
                .ForJob(job)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
