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

    public class DirectoryServiceShould
    {
        private readonly NoteRepository _nr;
        private readonly UserRepository _ur;
        private readonly DirectoryRepository _dr;
        private readonly ApplicationDbContext ctx;
        private DbContextOptions<ApplicationDbContext> _options;


        public DirectoryServiceShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            ctx = CreateDbContext();
            _nr = new NoteRepository(ctx);
            _ur = new UserRepository(ctx);
            _dr = new DirectoryRepository(ctx);
        }

        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }

       

        [Fact(DisplayName = "Be able to cascade mark directory with all subirectories and notes as deleted and to delete them from bin")]
        public async Task CascadeDeleteDirectorySubdirectoriesAndNotesFromBin()
        {
            //Arrange
            var ns = new NoteService(_nr);
            var ds = new DirectoryService(_nr, _dr, _ur);
            var usr = new ApplicationUser();

            var directory1 = new Directory("Directory name1", usr);
            var directory2 = new Directory("Directory name2", usr);
            var directory3 = new Directory("Directory name3", usr);
            var directory4 = new Directory("Directory name4", usr);
            var directory5 = new Directory("Directory name5", usr);
            var directory6 = new Directory("Directory name6", usr);
            var directory7 = new Directory("Directory name6", usr);
            var bin = new Directory("Bin", usr);

            await _dr.AddAsync(directory1);
            await _dr.AddAsync(directory2);
            await _dr.AddAsync(directory3);
            await _dr.AddAsync(directory4);
            await _dr.AddAsync(directory5);
            await _dr.AddAsync(directory6);
            await _dr.AddAsync(directory7);
            await _dr.AddAsync(bin);

            var note1 = new Note(null, "Test note1", "GetNoteById()", "def-ico", usr, directory1);
            var note2 = new Note(null, "Test note2", "GetNoteById()", "def-ico", usr, directory2);
            var note3 = new Note(null, "Test note3", "GetNoteById()", "def-ico", usr, directory2);
            var note4 = new Note(null, "Test note4", "GetNoteById()", "def-ico", usr, directory3);
            var note5 = new Note(null, "Test note5", "GetNoteById()", "def-ico", usr, directory4);
            var note6 = new Note(null, "Test note6", "GetNoteById()", "def-ico", usr, directory5);
            var note7 = new Note(null, "Test note7", "GetNoteById()", "def-ico", usr, directory6);
            var note8 = new Note(null, "Test note8", "GetNoteById()", "def-ico", usr, directory6);
            var note9 = new Note(null, "Test note8", "GetNoteById()", "def-ico", usr, directory7);

            await _nr.AddAsync(note1);
            await _nr.AddAsync(note2);
            await _nr.AddAsync(note3);
            await _nr.AddAsync(note4);
            await _nr.AddAsync(note5);
            await _nr.AddAsync(note6);
            await _nr.AddAsync(note7);
            await _nr.AddAsync(note8);
            await _nr.AddAsync(note9);


            await _dr.ChangeParentDirectoryForSubDirectory(directory2.DirectoryId, directory1.DirectoryId);
            await _dr.ChangeParentDirectoryForSubDirectory(directory3.DirectoryId, directory1.DirectoryId);
            await _dr.ChangeParentDirectoryForSubDirectory(directory4.DirectoryId, directory2.DirectoryId);
            await _dr.ChangeParentDirectoryForSubDirectory(directory5.DirectoryId, directory3.DirectoryId);
            await _dr.ChangeParentDirectoryForSubDirectory(directory6.DirectoryId, directory5.DirectoryId);

            var dirFromDatabase = await ds.GetDirectoryByIdAsync(directory1.DirectoryId);

            // Act
            var result = await ds.MoveDirectorySubdirectoriesAndNotesToBin(dirFromDatabase.DirectoryId);

            var dirFromDatabase1 = await ds.GetDirectoryByIdAsync(directory1.DirectoryId);
            var dirFromDatabase2 = await ds.GetDirectoryByIdAsync(directory2.DirectoryId);
            var dirFromDatabase3 = await ds.GetDirectoryByIdAsync(directory3.DirectoryId);
            var dirFromDatabase4 = await ds.GetDirectoryByIdAsync(directory4.DirectoryId);
            var dirFromDatabase5 = await ds.GetDirectoryByIdAsync(directory5.DirectoryId);
            var dirFromDatabase6 = await ds.GetDirectoryByIdAsync(directory6.DirectoryId);
            var dirFromDatabase7 = await ds.GetDirectoryByIdAsync(directory7.DirectoryId);

            var noteFromDatabase1 = await ns.GetNoteByIdAsync(note1.NoteId);
            var noteFromDatabase2 = await ns.GetNoteByIdAsync(note2.NoteId);
            var noteFromDatabase3 = await ns.GetNoteByIdAsync(note3.NoteId);
            var noteFromDatabase4 = await ns.GetNoteByIdAsync(note4.NoteId);
            var noteFromDatabase5 = await ns.GetNoteByIdAsync(note5.NoteId);
            var noteFromDatabase6 = await ns.GetNoteByIdAsync(note6.NoteId);
            var noteFromDatabase7 = await ns.GetNoteByIdAsync(note7.NoteId);
            var noteFromDatabase8 = await ns.GetNoteByIdAsync(note8.NoteId);

            var binFromDatabase = await ds.GetBinForParticularUserAsync(dirFromDatabase1.User.Id);

            // Assert
            Assert.NotNull(result);

            Assert.True(dirFromDatabase1.IsMarkedAsDeleted);
            Assert.True(dirFromDatabase2.IsMarkedAsDeleted);
            Assert.True(dirFromDatabase3.IsMarkedAsDeleted);
            Assert.True(dirFromDatabase4.IsMarkedAsDeleted);
            Assert.True(dirFromDatabase5.IsMarkedAsDeleted);
            Assert.True(dirFromDatabase6.IsMarkedAsDeleted);
            Assert.False(binFromDatabase.IsMarkedAsDeleted);
            Assert.False(dirFromDatabase7.IsMarkedAsDeleted);

            Assert.True(noteFromDatabase1.IsMarkedAsDeleted);
            Assert.True(noteFromDatabase2.IsMarkedAsDeleted);
            Assert.True(noteFromDatabase3.IsMarkedAsDeleted);
            Assert.True(noteFromDatabase4.IsMarkedAsDeleted);
            Assert.True(noteFromDatabase5.IsMarkedAsDeleted);
            Assert.True(noteFromDatabase6.IsMarkedAsDeleted);
            Assert.True(noteFromDatabase7.IsMarkedAsDeleted);
            Assert.True(noteFromDatabase8.IsMarkedAsDeleted);

            Assert.Equal(dirFromDatabase1.ParentDir.DirectoryId, binFromDatabase.DirectoryId);

            var directories = await ds.GetAllDirectoriesForParticularUserAsync(binFromDatabase.User.Id);
            Assert.Equal(8, directories.Count);

            var notes = await ns.GetAllUserNotesByIdAsync(binFromDatabase.User.Id);
            Assert.Equal(9, notes.Count);

            // Act
            var result2 = ds.RemoveDirectoriesSubdirectoriesAndNotesFromBinAndDb(0);
            Assert.True(result2);

            var directories2 = await ds.GetAllDirectoriesForParticularUserAsync(binFromDatabase.User.Id);
            Assert.Equal(2, directories2.Count);

            var notes2 = await ns.GetAllUserNotesByIdAsync(binFromDatabase.User.Id);
            Assert.Equal(1, notes2.Count);


        }

    }
}
