using System;
using System.Threading;
using System.Threading.Tasks;
using backend.context.identity.application.commands;
using backend.context.identity.domain.repo;
using backend.context.identity.infrastructure.persistence;
using backend.context.identity.infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using backend.context.identity.domain.vo;
using backend.context.common.application;
using backend.context.identity.domain.entity;

namespace Nailshop.Backend.Tests
{
    public class IdentityApplicationTests
    {
        private readonly global::AppDbContext _context;
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public IdentityApplicationTests()
        {
            var options = new DbContextOptionsBuilder<global::AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new global::AppDbContext(options);
            _repository = new UserRepository(_context);
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public async Task RegisterUser_WithValidData_ShouldSaveToDatabase()
        {
            // Arrange
            var handler = new RegisterUserCommandHandler(_repository, _passwordHasher);
            var command = new RegisterUserCommand("Nhat Doo", "0912345678", "nhatdoo@test.com", "Password123!", "Customer");

            // Act
            await handler.HandleAsync(command);

            // Assert - Cần dùng .Value cho Value Objects
            var user = await _context.Users.Include(u => u.Auth).FirstOrDefaultAsync(u => u.Email.Value == "nhatdoo@test.com");
            Assert.NotNull(user);
            Assert.Equal("Nhat Doo", user.Ten);
            Assert.NotEqual("Password123!", user.Auth.PasswordHash);
        }

        [Fact]
        public async Task RegisterUser_WithDuplicateEmail_ShouldThrowException()
        {
            // Arrange
            var handler = new RegisterUserCommandHandler(_repository, _passwordHasher);
            var command = new RegisterUserCommand("User 1", "0912345671", "duplicate@test.com", "Password123!", "Customer");
            var command2 = new RegisterUserCommand("User 2", "0912345672", "duplicate@test.com", "PasswordXYZ!", "Customer");

            // Act & Assert
            await handler.HandleAsync(command);
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(command2));
        }

        [Fact]
        public async Task RegisterUser_ShouldAssignCorrectRole()
        {
            // Arrange
            var handler = new RegisterUserCommandHandler(_repository, _passwordHasher);
            var command = new RegisterUserCommand("Admin User", "0988888888", "admin@test.com", "AdminPass123!", "Admin");

            // Act
            await handler.HandleAsync(command);

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == "admin@test.com");
            Assert.Equal(UserRole.Admin, user.Role.Value);
        }
    }
}
