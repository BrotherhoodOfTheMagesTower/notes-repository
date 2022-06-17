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

        public async Task<ICollection<Event>> GetAllUserEventsAsync(string userId)
            => await _er.GetAllUserEventsAsync(userId);

        public async Task<ICollection<Event>> GetEventsByContentAsync(string content)
            => await _er.GetEventsByContentAsync(content);

        public async Task<ICollection<Event>> GetEventsByStartDateAsync(DateTime date)
            => await _er.GetEventsByStartDateAsync(date);

        public async Task<ICollection<Event>> GetIncomingEventsAsync(int eventCount, string userId)
            => await _er.GetIncomingEventsAsync(eventCount, userId);

        public async Task<Event?> GetByIdAsync(Guid eventId)
            => await _er.GetByIdAsync(eventId);

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

        public async Task<bool> DeleteAsync(Event _event)
        {
            if (_event.ReminderAt is not null && _event.ReminderAt > DateTime.Now)
            {
                await UnscheduleEventReminder(_event);
            }
            return await _er.DeleteAsync(_event);
        }

        public async Task<bool> DeleteByIdAsync(Guid eventId)
        {
            var _event = _er.GetById(eventId);
            if (_event is not null && _event.ReminderAt is not null && _event.ReminderAt > DateTime.Now)
            {
                await UnscheduleEventReminder(_event);
            }
            return await _er.DeleteByIdAsync(eventId);
        }

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

        public async Task EditEventReminderAsync(Event _event)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            if (!scheduler.IsStarted)
                await scheduler.Start();

            await scheduler.UnscheduleJob(new TriggerKey($"{_event.EventId}-trigger", _event.User.Email));
            await ScheduleEventReminderAsync(_event);
        }

        private async Task UnscheduleEventReminder(Event _event)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            if (!scheduler.IsStarted)
                await scheduler.Start();

            await scheduler.UnscheduleJob(new TriggerKey($"{_event.EventId}-trigger", _event.User.Email));
        }

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
