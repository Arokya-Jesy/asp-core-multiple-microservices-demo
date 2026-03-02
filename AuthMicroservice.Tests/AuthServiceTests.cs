using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AuthMicroservice.Data;
using AuthMicroservice.Models;
using AuthMicroservice.Services;

namespace AuthMicroservice.Tests;

public class AuthServiceTests
{
    private AuthDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AuthDbContext(options);
    }

    private IConfiguration GetConfiguration()
    {
        var config = new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "YourSuperSecretKeyThatIsAtLeast32CharactersLong123456"},
            {"JwtSettings:Issuer", "AuthMicroservice"},
            {"JwtSettings:Audience", "ProductMicroservice"}
        };
        return new ConfigurationBuilder().AddInMemoryCollection(config).Build();
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var config = GetConfiguration();
        var logger = Mock.Of<ILogger<AuthService>>();
        
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "user"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthService(context, config, logger);
        var request = new LoginRequest { Username = "testuser", Password = "password123" };

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("user", result.Role);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var config = GetConfiguration();
        var logger = Mock.Of<ILogger<AuthService>>();
        var service = new AuthService(context, config, logger);
        var request = new LoginRequest { Username = "nonexistent", Password = "wrong" };

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }
}
