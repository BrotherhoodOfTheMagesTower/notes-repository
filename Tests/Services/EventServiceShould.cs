using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using NotesRepository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public class EventServiceShould
    {
        private readonly EventRepository _er;
        private readonly NoteRepository _nr;
        private readonly ApplicationDbContext ctx;
        private DbContextOptions<ApplicationDbContext> _options;

        public EventServiceShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            ctx = CreateDbContext();
            _er = new EventRepository(ctx);
            _nr = new NoteRepository(ctx);
        }
        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "Be able to get an event by ID")]
        public async Task GetEventById()
        {
            //Arrange
            var es = new EventService(_er);
            var _event = new Event(null, "GetEventById()", new DateTime(2031, 12, 12), new DateTime(2033, 12, 12), new ApplicationUser());
            await es.AddAsync(_event);

            // Act
            var result = await es.GetByIdAsync(_event.EventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result!.EventId, _event.EventId);
        }
        
        [Fact(DisplayName = "Be able to get all user events")]
        public async Task GetAllUserEvents()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event1 = new Event(null, "GetAllUserEvents()", new DateTime(2031, 12, 12), new DateTime(2033, 12, 12), usr);
            var _event2 = new Event(null, "GetAllUserEvents()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);
            await es.AddAsync(_event1);
            await es.AddAsync(_event2);

            // Act
            var result = await es.GetAllUserEventsAsync(usr.Id);

            // Assert
            Assert.NotNull(result);
            result.Should().Contain(_event1).And.Contain(_event2).And.HaveCount(2);
        }
        
        [Fact(DisplayName = "Be able to get events by content")]
        public async Task GetEventsByContent()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event1 = new Event(null, "GetEventsByContent()", new DateTime(2031, 12, 12), new DateTime(2033, 12, 12), usr);
            var _event2 = new Event(null, "none()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);
            await es.AddAsync(_event1);
            await es.AddAsync(_event2);

            // Act
            var result = await es.GetEventsByContentAsync("GetEven");

            // Assert
            Assert.NotNull(result);
            result.Should().Contain(_event1).And.NotContain(_event2);
        }
        
        [Fact(DisplayName = "Be able to get events by start date")]
        public async Task GetEventsByStartDate()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var startDay = new DateTime(2032, 12, 12);
            var _event1 = new Event(null, "GetEventsByContent()", startDay, new DateTime(2033, 12, 12), usr);
            var _event2 = new Event(null, "none()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);
            await es.AddAsync(_event1);
            await es.AddAsync(_event2);

            // Act
            var result = await es.GetEventsByStartDateAsync(startDay);

            // Assert
            Assert.NotNull(result);
            result.Should().Contain(_event1).And.NotContain(_event2);
        }
        
        [Fact(DisplayName = "Be able to add event")]
        public async Task AddEvent()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event = new Event(null, "AddEvent()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);

            // Act
            var result = await es.AddAsync(_event);

            // Assert
            var ev = await es.GetByIdAsync(_event.EventId);
            Assert.True(result);
            Assert.NotNull(ev);
        }

        [Fact(DisplayName = "Be able to handle wrong event properties")]
        public async Task AddWrongEvent()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event1 = new Event(null, "AddWrongEvent1()", new DateTime(2036, 12, 12), new DateTime(2034, 12, 12), usr);
            var _event2 = new Event(null, "AddWrongEvent2()", new DateTime(2010, 12, 12), new DateTime(2033, 12, 12), usr);

            // Act
            var result1 = await es.AddAsync(_event1);
            var result2 = await es.AddAsync(_event2);

            // Assert
            var ev1 = await es.GetByIdAsync(_event1.EventId);
            var ev2 = await es.GetByIdAsync(_event2.EventId);
            Assert.False(result1);
            Assert.False(result2);
            Assert.Null(ev1);
            Assert.Null(ev2);
        }

        [Fact(DisplayName = "Be able to update event")]
        public async Task UpdateEvent()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event = new Event(null, "none()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);
            await es.AddAsync(_event);
            _event.Content = "updated";
            _event.EndAt = new DateTime(2033, 12, 12);

            // Act
            var result = await es.UpdateAsync(_event);

            // Assert
            var ev = await es.GetByIdAsync(_event.EventId);
            Assert.True(result);
            Assert.NotNull(ev);
            ev!.Content.Should().Be("updated");
            ev.EndAt.Should().Be(new DateTime(2033, 12, 12));
        }
        
        [Fact(DisplayName = "Be able to delete event")]
        public async Task Delete()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event = new Event(null, "Delete()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);
            await es.AddAsync(_event);

            // Act
            var result = await es.DeleteAsync(_event);

            // Assert
            var ev = await es.GetByIdAsync(_event.EventId);
            Assert.True(result);
            Assert.Null(ev);
        }
        
        [Fact(DisplayName = "Be able to delete event by ID")]
        public async Task DeleteById()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event = new Event(null, "DeleteById()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);
            await es.AddAsync(_event);

            // Act
            var result = await es.DeleteByIdAsync(_event.EventId);

            // Assert
            var ev = await es.GetByIdAsync(_event.EventId);
            Assert.True(result);
            Assert.Null(ev);
        }

        [Fact(DisplayName = "Be able to delete many events by ID")]
        public async Task DeleteManyEventsById()
        {
            //Arrange
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var _event1 = new Event(null, "DeleteManyEventsById1()", new DateTime(2031, 12, 12), new DateTime(2034, 12, 12), usr);
            var _event2 = new Event(null, "DeleteManyEventsById2()", new DateTime(2031, 12, 12), new DateTime(2035, 12, 12), usr);
            await es.AddAsync(_event1);
            await es.AddAsync(_event2);

            // Act
            var result = await es.DeleteManyAsync(new List<Guid> { _event1.EventId, _event2.EventId });

            // Assert
            var ev1 = await es.GetByIdAsync(_event1.EventId);
            var ev2 = await es.GetByIdAsync(_event2.EventId);
            Assert.True(result);
            Assert.Null(ev1);
            Assert.Null(ev2);
        }

        [Fact(DisplayName = "Be able to attach event to a note")]
        public async Task AttachEventToNoteAsync()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var es = new EventService(_er);
            var usr = new ApplicationUser();
            var note = new Note(null, "Tst", "AttachEventToNote()", "def-ico", usr, new Directory("Default", usr));
            await ns.AddNoteAsync(note);
            var ev = new Event(Guid.NewGuid(), "Con", DateTime.Now, DateTime.Now.AddMinutes(2), usr);
            await _er.AddAsync(ev);

            // Act
            await es.AttachNoteToEventAsync(ev.EventId, note.NoteId);

            // Assert
            var noteFromDb = await ns.GetNoteByIdAsync(note.NoteId);
            var eventFromDb = await es.GetByIdAsync(ev.EventId);
            noteFromDb.Should().NotBeNull();
            noteFromDb!.Event!.Should().Be(ev);
            eventFromDb!.Note!.Should().Be(note);
            eventFromDb!.Note.NoteId!.Should().Be(note.NoteId);
        }
    }
}
