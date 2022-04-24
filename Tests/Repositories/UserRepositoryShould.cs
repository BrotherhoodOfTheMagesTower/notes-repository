using Microsoft.EntityFrameworkCore;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Tests
{
    public class UserRepositoryShould
    {
        private readonly ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;

        public UserRepositoryShould()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("In memory database - ApplicationUser")
                .Options;
            _context = new ApplicationDbContext(_options);
        }

        [Fact(DisplayName = "System is able to add a user to the database")]
        public async Task AddUser()
        {
            // Arrange
            var ur = new UserRepository(_context);
            var user = new ApplicationUser()
            {
                FirstName = "Frodo"
            };

            // Act
            var result = await ur.AddUserAsync(user);

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.True(result);
            Assert.Equal(users.FirstOrDefault(u => u.FirstName == "Frodo")!.FirstName, user.FirstName);
            result = await ur.DeleteUserByIdAsync(user.Id);
        }

        [Fact(DisplayName = "System is able to delete a user from the database")]
        public async Task DeleteUser()
        {
            // Arrange
            var ur = new UserRepository(_context);
            var user = new ApplicationUser()
            {
                FirstName = "Frodo"
            };
            await ur.AddUserAsync(user);
            var users = await _context.Users.ToListAsync();

            // Act
            var result = await ur.DeleteUserByIdAsync(user.Id);
            var usersAfterDelete = await _context.Users.ToListAsync();

            // Assert
            Assert.True(result);
            Assert.NotEqual(usersAfterDelete.Count, users.Count);
        }

        [Fact(DisplayName = "System is able to get user by id from the database")]
        public async Task GetUserById()
        {
            // Arrange
            var ur = new UserRepository(_context);
            var user = new ApplicationUser()
            {
                FirstName = "Frodo",
                LastName = "Baggins"
            };
            await ur.AddUserAsync(user);

            // Act
            var result = await ur.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result!.Id);
            result.Should().BeAssignableTo<ApplicationUser>();
            await ur.DeleteUserByIdAsync(user.Id);
        }

        [Fact(DisplayName = "System is able to get user by e-mail from the database")]
        public async Task GetUserByEmail()
        {
            // Arrange
            var ur = new UserRepository(_context);
            var user = new ApplicationUser()
            {
                FirstName = "Frodo",
                LastName = "Baggins",
                Email = "shire@o2.pl"
            };
            await ur.AddUserAsync(user);

            // Act
            var result = await ur.GetUserByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result!.Email);
            result.Should().BeAssignableTo<ApplicationUser>();
            await ur.DeleteUserByIdAsync(user.Id);
        }

        [Fact(DisplayName = "System is able to return all users from the database")]
        public async Task GetAllUsers()
        {
            // Arrange
            var ur = new UserRepository(_context);
            var usersData = new List<(string, string, string)>
            {
                ("Frodo", "Baggins", "shire@o2.pl"),
                ("Bilbo", "Baggins", "oldShire@wp.pl"),
                ("Tom", "Bombadil", "oldForset@gmail.com")
            };

            var users = new List<ApplicationUser>();
            foreach(var user in usersData)
            {
                users.Add(new ApplicationUser()
                    {
                        FirstName = user.Item1,
                        LastName = user.Item2,
                        Email = user.Item3
                    }
                );
                await ur.AddUserAsync(users.Last());
            }

            // Act
            var result = await ur.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.FirstOrDefault(u => u.FirstName == "Frodo")!.FirstName, users[0].FirstName);
            Assert.Equal(result.FirstOrDefault(u => u.FirstName == "Bilbo")!.LastName, users[1].LastName);
            Assert.Equal(result.FirstOrDefault(u => u.FirstName == "Tom")!.Email, users[2].Email);

            foreach (var user in result)
            {
                await ur.DeleteUserByIdAsync(user.Id);
            }
        }

        [Fact(DisplayName = "System is able to return all users with the same name from the database")]
        public async Task GetUsersByUsername()
        {
            // Arrange
            var ur = new UserRepository(_context);
            var usersData = new List<(string, string, string)>
            {
                ("Tony", "Hawk", "ProSkater@gmail.com"),
                ("Tony", "Montana", "scarface@gmail.com")
            };

            var users = new List<ApplicationUser>();
            foreach (var user in usersData)
            {
                users.Add(new ApplicationUser()
                {
                    UserName = user.Item1,
                    FirstName = user.Item1,
                    LastName = user.Item2,
                    Email = user.Item3
                }
                );
                await ur.AddUserAsync(users.Last());
            }

            // Act
            var result = await ur.GetUsersByUserNameAsync("Tony");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Count, usersData.Count);
            Assert.Equal(result.FirstOrDefault(u => u.LastName == "Hawk")!.UserName, users[0].UserName);
            Assert.Equal(result.FirstOrDefault(u => u.LastName == "Montana")!.UserName, users[0].UserName);

            foreach (var user in result)
            {
                await ur.DeleteUserByIdAsync(user.Id);
            }
        }

        [Fact(DisplayName = "System is able to return all users with the same names from the database")]
        public async Task GetUsersByFirstAndLastName()
        {
            // Arrange
            var ur = new UserRepository(_context);
            var usersData = new List<(string, string, string)>
            {
                ("Jan", "Kowalski", "KowalskiJan@gmail.com"),
                ("Jan", "Kowalski", "JanKowalski@gmail.com")
            };

            var users = new List<ApplicationUser>();
            foreach (var user in usersData)
            {
                users.Add(new ApplicationUser()
                {
                    FirstName = user.Item1,
                    LastName = user.Item2,
                    Email = user.Item3
                }
                );
                await ur.AddUserAsync(users.Last());
            }

            // Act
            var result = await ur.GetUsersByFirstAndLastNameAsync("Jan","Kowalski");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Count, usersData.Count);
            Assert.Equal(result.FirstOrDefault(u => u.Email == "KowalskiJan@gmail.com")!.FirstName, users[1].FirstName);
            Assert.Equal(result.FirstOrDefault(u => u.Email == "JanKowalski@gmail.com")!.LastName, users[0].LastName);

            foreach (var user in result)
            {
                await ur.DeleteUserByIdAsync(user.Id);
            }
        }
    }
}