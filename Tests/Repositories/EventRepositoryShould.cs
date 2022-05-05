using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using Xunit;
using FluentAssertions;

namespace Tests.Repositories
{
    public class EventRepositoryShould : IDbContextFactory<ApplicationDbContext>
    {
        private readonly ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;

        public EventRepositoryShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = CreateDbContext();
        }

        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "User is able to add a event to the database")]
        public async Task AddEvent()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvent = new Event(null, "example content", date, date, new ApplicationUser());

            // Act
            var result = await ne.AddAsync(calendarEvent);

            // Assert
            var events = await _context.Events.ToListAsync();
            Assert.True(result);
            Assert.Single(events);
            await ne.DeleteAsync(calendarEvent);
        }

        [Fact(DisplayName = "User is able to delete a event from the database")]
        public async Task DeleteEvent()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvent = new Event(null, "example content", date, date, new ApplicationUser());
            await ne.AddAsync(calendarEvent);

            // Act
            var result = await ne.DeleteAsync(calendarEvent);

            // Assert
            var events = await _context.Events.ToListAsync();
            Assert.True(result);
            Assert.Empty(events);
        }

        [Fact(DisplayName = "User is able to delete many Events")]
        public async Task DeleteManyEvents()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvents = new List<Event>() {
                new Event(null, "example content", date, date, new ApplicationUser()),
                new Event(null, "example content", date, date, new ApplicationUser()),
                new Event(null, "example content", date, date, new ApplicationUser()),
                new Event(null, "example content", date, date, new ApplicationUser())};
            var guids = new List<Guid>();
            foreach (var e in calendarEvents)
            {
                await ne.AddAsync(e);
                guids.Add(e.EventId);
            }

            // Act
            var result = await ne.DeleteManyAsync(guids);

            // Assert
            var events = await _context.Events.ToListAsync();
            Assert.True(result);
            Assert.Empty(events);
        }

        [Fact(DisplayName = "User is able to delete Event by id")]
        public async Task DeleteEventById()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvent = new Event(null, "example content", date, date, new ApplicationUser());
            await ne.AddAsync(calendarEvent);

            // Act
            var result = await ne.DeleteByIdAsync(calendarEvent.EventId);

            // Assert
            var events = await _context.Events.ToListAsync();
            Assert.True(result);
            Assert.Empty(events);
        }

        [Fact(DisplayName = "Is possible to get all user's event")]
        public async Task GetAllUserEventsAsync()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var usr = new ApplicationUser();
            var calendarEvents = new List<Event>() {
                new Event(null, "example content 1", date, date, usr),
                new Event(null, "example content 2", date, date, usr),
                new Event(null, "example content 3", date, date, usr),
                new Event(null, "example content 4", date, date, usr)};
            var guids = new List<Guid>();
            foreach (var e in calendarEvents)
            {
                await ne.AddAsync(e);
                guids.Add(e.EventId);
            }

            // Act
            var result = await ne.GetAllUserEventsAsync(usr.Id);

            // Assert
            result.Should().AllBeAssignableTo<Event>();
            result.Should().HaveCount(4);
            foreach (var calendarEvent in result)
            {
                Assert.True(calendarEvent.User == usr);
            }
            await ne.DeleteManyAsync(guids);
        }


        [Fact(DisplayName = "Is possible to get event by id")]
        public async Task GetByIdAsync()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvent = new Event(null, "example content", date, date, new ApplicationUser());
            await ne.AddAsync(calendarEvent);

            // Act
            var result = await ne.GetByIdAsync(calendarEvent.EventId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Event>();
            if (result != null)
            {
                Assert.Equal(calendarEvent.EventId, result.EventId);
            }
            await ne.DeleteAsync(calendarEvent);
        }


        [Fact(DisplayName = "Is possible to get event by pattern in content")]
        public async Task GetEventsByContentAsync()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvent = new Event(null, "example content", date, date, new ApplicationUser());
            await ne.AddAsync(calendarEvent);

            // Act
            var result = await ne.GetEventsByContentAsync("con");

            // Assert
            Assert.NotNull(result);
            result.Should().AllBeAssignableTo<Event>();
            foreach (var e in result)
            {
                Assert.Contains("con", e.Content);
            }
            await ne.DeleteAsync(calendarEvent);
        }

        [Fact(DisplayName = "Is possible to get events by date")]
        public async Task GetEventsByDateAsync()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvents = new List<Event>() {
                new Event(null, "example content 1", date, date, new ApplicationUser()),
                new Event(null, "example content 2", date, date, new ApplicationUser())};
            var guids = new List<Guid>();
            foreach (var e in calendarEvents)
            {
                await ne.AddAsync(e);
                guids.Add(e.EventId);
            }

            // Act
            var calendarEvent = calendarEvents.First();
            var result = await ne.GetEventsByStartDateAsync(calendarEvent.StartAt);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllBeAssignableTo<Event>();
            foreach (var e in result)
            {
                Assert.Equal(calendarEvent.StartAt, e.StartAt);
            }
            await ne.DeleteManyAsync(guids);
        }

        [Fact(DisplayName = "Is possible to update the event")]
        public async Task UpdateAsync()
        {
            // Arrange
            var ne = new EventRepository(_context);
            DateTime date = DateTime.Now;
            var calendarEvent = new Event(null, "example content", date, date, new ApplicationUser());
            await ne.AddAsync(calendarEvent);

            // Act
            calendarEvent.Content = "cahnged content";
            var result = await ne.UpdateAsync(calendarEvent);

            // Assert
            var updateEvent = await ne.GetByIdAsync(calendarEvent.EventId);
            Assert.True(result);
            if (updateEvent != null)
            {
                Assert.Equal(updateEvent.Content, calendarEvent.Content);
            }
            await ne.DeleteAsync(calendarEvent);
        }

    }
}