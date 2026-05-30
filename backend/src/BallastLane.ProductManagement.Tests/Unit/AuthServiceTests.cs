using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Application.Services;
using BallastLane.ProductManagement.Domain.Entities;
using BallastLane.ProductManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BallastLane.ProductManagement.Tests.Unit
{
    /// <summary>
    /// Unit tests for AuthService.
    /// </summary>
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _repositoryMock = new Mock<IUserRepository>();

            var inMemoryConfig = new Dictionary<string, string?>
            {
                { "Jwt:Secret", "BallastLane-SuperSecret-Key-For-JWT-2024!" },
                { "Jwt:Issuer", "BallastLane.ProductManagement.API" },
                { "Jwt:Audience", "BallastLane.ProductManagement.Client" },
                { "Jwt:ExpiresInMinutes", "60" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            _service = new AuthService(_repositoryMock.Object, _configuration);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUserAndReturnToken()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync((User)null!);
            _repositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => { u.Id = 1; return u; });

            var dto = new RegisterUserDto { Username = "john", Email = "john@example.com", Password = "secret123" };

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Username.Should().Be("john");
            result.Value.Token.Should().NotBeNullOrEmpty();
            result.Value.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task RegisterAsync_WhenUsernameAlreadyTaken_ShouldReturnFailure()
        {
            // Arrange
            var existingUser = new User { Id = 1, Username = "john", Email = "john@example.com", PasswordHash = "hash", CreatedAt = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(existingUser);

            var dto = new RegisterUserDto { Username = "john", Email = "john2@example.com", Password = "secret123" };

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("already taken");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("secret123");
            var user = new User { Id = 1, Username = "john", Email = "john@example.com", PasswordHash = passwordHash, CreatedAt = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

            var dto = new LoginDto { Username = "john", Password = "secret123" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().NotBeNullOrEmpty();
            result.Value.Username.Should().Be("john");
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnFailure()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("secret123");
            var user = new User { Id = 1, Username = "john", Email = "john@example.com", PasswordHash = passwordHash, CreatedAt = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

            var dto = new LoginDto { Username = "john", Password = "wrongpassword" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("Invalid username or password");
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ShouldReturnFailure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User)null!);

            var dto = new LoginDto { Username = "unknown", Password = "secret123" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("Invalid username or password");
        }
    }
}
