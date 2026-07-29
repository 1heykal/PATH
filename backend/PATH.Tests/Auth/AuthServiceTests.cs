using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PATH.Application.Exceptions;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Tests.Auth
{
    public class AuthServiceTests
    {
        private IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "this-is-a-test-secret-key-with-at-least-32-characters",
                ["Jwt:Issuer"] = "PATH.Tests",
                ["Jwt:Audience"] = "PATH.Tests",
                ["Jwt:ExpiresAfter"] = "15"
            })
            .Build();


        [Fact] // methodName_tested_condition_expectedBehavior
        public async Task RefreshAccessToken_RevokedToken_ThrowsAndRevokesAllTokens()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();

            var user = new ApplicationUser
            {
                Email = "test@example.com",
                BirthDate = new DateOnly(1990, 1, 1),
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                Username = "testuser"

            };

            context.Users.Add(user);

            var revokedToken = new RefreshToken
            {
                Token = "oldtoken",
                UserId = user.Id,
                IsRevoked = true,
                ExpiresAt = DateTime.UtcNow.AddDays(1),

            };
            context.RefreshTokens.Add(revokedToken);
            await context.SaveChangesAsync();

            var authService = new AuthService(context, _configuration, new UserService(context));

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => authService.RefreshAccessToken("oldtoken"));

            var tokens = await context.RefreshTokens.
                Where(rt => rt.UserId == user.Id)
                .ToListAsync();

            Assert.All(tokens, t => Assert.True(t.IsRevoked));

        }

        [Fact]
        public async Task RefreshAccessToken_ExpiredToken_ThrowsAndRevokesToken()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();

            var user = new ApplicationUser
            {
                Email = "test@example.com",
                BirthDate = new DateOnly(1990, 1, 1),
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                Username = "testuser"

            };

            context.Users.Add(user);

            var revokedToken = new RefreshToken
            {
                Token = "expiredtoken",
                UserId = user.Id,
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
            };
            context.RefreshTokens.Add(revokedToken);
            await context.SaveChangesAsync();

            var authService = new AuthService(context, _configuration, new UserService(context));

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => authService.RefreshAccessToken("expiredtoken"));

            var token = await context.RefreshTokens.
                Where(rt => rt.Token == "expiredtoken")
              .FirstOrDefaultAsync();

            Assert.True(token?.IsRevoked);
        }


        [Fact]
        public async Task RefreshAccessToken_ValidToken_ReturnsNewAccessTokenAndRefreshTokenAndUserBasicInfo()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();

            var user = new ApplicationUser
            {
                Email = "test@example.com",
                BirthDate = new DateOnly(1990, 1, 1),
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                Username = "testuser"

            };

            context.Users.Add(user);

            var revokedToken = new RefreshToken
            {
                Token = "validtoken",
                UserId = user.Id,
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
            };
            context.RefreshTokens.Add(revokedToken);
            await context.SaveChangesAsync();

            var authService = new AuthService(context, _configuration, new UserService(context));

            // Act & Assert
            var result = await authService.RefreshAccessToken("validtoken");
            Assert.True(result.accessToken != null && result.refreshToken != null && result.userInfo != null);
        }

        [Fact]
        public async Task RefreshAccessToken_InvalidToken_Throws()
        {
            // Arrange  
            var context = ApplicationDbContext.GetSqliteContext();
            var authService = new AuthService(context, _configuration, new UserService(context));

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => authService.RefreshAccessToken("invalidtoken"));
        }


        [Fact]
        public async Task LogoutUser_ValidRefreshToken_RevokesToken()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();

            var user = new ApplicationUser
            {
                Email = "test@example.com",
                BirthDate = new DateOnly(1990, 1, 1),
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                Username = "testuser"

            };

            context.Users.Add(user);

            var revokedToken = new RefreshToken
            {
                Token = "refreshToken",
                UserId = user.Id,
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
            };
            context.RefreshTokens.Add(revokedToken);
            await context.SaveChangesAsync();

            var authService = new AuthService(context, _configuration, new UserService(context));

            // Act & Assert
            await authService.LogoutUser("refreshToken");

            context.ChangeTracker.Clear();

            var token = await context.RefreshTokens.Where(rt => rt.Token.Equals("refreshToken"))
              .FirstOrDefaultAsync();

            Assert.True(token?.IsRevoked);
        }

        [Fact]
        public async Task RegisterUser_ValidModelWithNotExistingEmail_AddsNewUser()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();

            var newUser = new RegisterUserModel
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var authService = new AuthService(context, _configuration, new UserService(context));

            // Act

            await authService.RegisterUser(newUser);
            var userExists = await context.Users.AnyAsync(u => u.Email == newUser.Email);

            Assert.True(userExists);
        }


    }
}
