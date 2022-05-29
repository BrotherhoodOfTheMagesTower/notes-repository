using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Repositories
{
    public class DirectoryRepositoryShould
    {
        private readonly ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;

        public DirectoryRepositoryShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "User is able to add a directory to the database")]
        public async Task AddDirectory()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var directory = new Directory("Directory name", usr);

            // Act
            var result = await nr.AddAsync(directory);

            // Assert
            var directories = await _context.Directories.ToListAsync();
            Assert.True(result);
            Assert.Single(directories);
            await nr.DeleteAsync(directory);
        }


        [Fact(DisplayName = "User is able to delete a directory from the database")]
        public async Task DeleteDirectory()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var directory = new Directory("Directory name", usr);
            await nr.AddAsync(directory);

            // Act
            var result = await nr.DeleteAsync(directory);

            // Assert
            var directories = await _context.Directories.ToListAsync();
            Assert.True(result);
            Assert.Empty(directories);
        }

        [Fact(DisplayName = "User is able to delete a directory by id from the database")]
        public async Task DeleteDirectoryById()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var directory = new Directory("Directory name", usr);
            await nr.AddAsync(directory);

            // Act
            var result = await nr.DeleteByIdAsync(directory.DirectoryId);

            // Assert
            var directories = await _context.Directories.ToListAsync();
            Assert.True(result);
            Assert.Empty(directories);
        }

        [Fact(DisplayName = "User is able to update a directory in the database")]
        public async Task UpdateDirectory()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var usr2 = new ApplicationUser();
            var directory = new Directory("Directory name2", usr);
            await nr.AddAsync(directory);
            directory.Name = "Directory name - changed";
            directory.User = usr2;
            // Act
            var result = await nr.UpdateAsync(directory);

            // Assert
            var directories = await _context.Directories.ToListAsync();
            //Assert.True(result);
            Assert.Single(directories);
            Assert.Equal(directory.Name, directories.First(i => i.DirectoryId == directory.DirectoryId).Name);
            Assert.Equal(directory.User, directories.First(i => i.DirectoryId == directory.DirectoryId).User);
            await nr.DeleteAsync(directory);
        }

        [Fact(DisplayName = "User is able to get a directory by id from the database")]
        public async Task GetDirectoryById()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var directory = new Directory("Directory name", usr);
            await nr.AddAsync(directory);

            // Act
            var result = await nr.GetByIdAsync(directory.DirectoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(directory.DirectoryId, result!.DirectoryId);
            result.Should().BeAssignableTo<Directory>();
            await nr.DeleteAsync(directory);
        }

        [Fact(DisplayName = "User is able to get a directory by name from the database")]
        public async Task GetDirectoryByName()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var directory = new Directory("Directory name", usr);
            await nr.AddAsync(directory);

            // Act
            var result = await nr.GetDirectoryByNameAsync(directory.Name, usr.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(directory.DirectoryId, result!.DirectoryId);
            result.Should().BeAssignableTo<Directory>();
            await nr.DeleteAsync(directory);
        }

        [Fact(DisplayName = "User is able to get all subdirectories by directory id from the database")]
        public async Task GetAllSubDirectoriesByDirectoryId()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var directory = new Directory("Directory name", usr);


            var subDirectory1 = new Directory("subDirectory 1", usr);
            var subDirectory2 = new Directory("subDirectory 2", usr);
            var subDirectory3 = new Directory("subDirectory 3", usr);

            if(directory.SubDirectories==null)
                directory.SubDirectories = new List<Directory>();
            directory.SubDirectories.Add(subDirectory1);
            directory.SubDirectories.Add(subDirectory2);
            directory.SubDirectories.Add(subDirectory3);

            await nr.AddAsync(directory);

        
            // Act
            var result = await nr.GetAllSubDirectoriesOfParticularDirectoryAsync(directory.DirectoryId);


            // Assert

            await nr.DeleteAsync(directory);

            var directories = await _context.Directories.ToListAsync();
            directories.Should().HaveCount(3);


            await nr.DeleteAsync(subDirectory1);
            await nr.DeleteAsync(subDirectory2);
            await nr.DeleteAsync(subDirectory3);
            

       

            Assert.NotNull(result);
            result.Should().AllBeAssignableTo<Directory>();
            result.Should().HaveCount(3);
            //result.Should().Contain(x => x.SubDirectories. == directory.DirectoryId);


        }

        [Fact(DisplayName = "User is able to get all directories by user id from the database")]
        public async Task GetAllDirectoriesByUserId()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var usr2 = new ApplicationUser();

            var directory1 = new Directory("Directory 1", usr);
            var directory2 = new Directory("Directory 2", usr);
            var directory3 = new Directory("Directory 3", usr2);


            await nr.AddAsync(directory1);
            await nr.AddAsync(directory2);
            await nr.AddAsync(directory3);

            // Act
            var result = await nr.GetAllDirectoriesForParticularUserAsync(usr.Id);


            // Assert
            await nr.DeleteAsync(directory1);
            await nr.DeleteAsync(directory2);
            await nr.DeleteAsync(directory3);
            Assert.NotNull(result);
            result.Should().AllBeAssignableTo<Directory>();
            result.Should().HaveCount(2);

        }

        [Fact(DisplayName = "User is able to attach subdirectory to particular directory")]
        public async Task AttachSubDirectoryToParticularDirectory()
        {
            // Arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();

            var directory1 = new Directory("Directory 1", usr);
            var subDirectory1 = new Directory("subDirectory 1", usr);

            await nr.AddAsync(directory1);
            await nr.AddAsync(subDirectory1);

            // Act
            var result = await nr.ChangeParentDirectoryForSubDirectory(subDirectory1.DirectoryId, directory1.DirectoryId);  
            
            // Assert
            var directories = await _context.Directories.ToListAsync();
            var dir = await nr.GetByIdAsync(directory1.DirectoryId);
            
            Assert.True(result);
            Assert.NotNull(dir);
            Assert.Equal(subDirectory1, directories.First(i => i.DirectoryId == directory1.DirectoryId).SubDirectories.First());
            Assert.Equal(subDirectory1.DirectoryId, dir.SubDirectories.First().DirectoryId);
            await nr.DeleteAsync(subDirectory1);
            await nr.DeleteAsync(directory1);

            directories.Count.Should().Be(2);
            directories.Should().AllBeAssignableTo<Directory>();

        }


        [Fact(DisplayName = "User is able to delete all subdirectories from particular directory")]
        public async Task DeleteAllSubDirectoriesForParticularDirectory()
        {

            //arrange
            var nr = new DirectoryRepository(_context);
            var usr = new ApplicationUser();
            var directory = new Directory("Directory name", usr);

            var subDirectory1 = new Directory("subDirectory 1", usr);
            var subDirectory2 = new Directory("subDirectory 2", usr);
            var subDirectory3 = new Directory("subDirectory 3", usr);

            if (directory.SubDirectories == null)
                directory.SubDirectories = new List<Directory>();
            directory.SubDirectories.Add(subDirectory1);
            directory.SubDirectories.Add(subDirectory2);
            directory.SubDirectories.Add(subDirectory3);

            await nr.AddAsync(directory);

            // Act
            var result = await nr.DeleteAllSubDirectoriesForParticularDirectoryAsync(directory.DirectoryId);

            // Assert
            var directories = await _context.Directories.ToListAsync();
            var subDirectories = await nr.GetAllSubDirectoriesOfParticularDirectoryAsync(directory.DirectoryId);

            Assert.True(result);
            directories.Should().HaveCount(1);
            subDirectories.Should().HaveCount(0);
            await nr.DeleteAsync(directory);
        }


    }
}
