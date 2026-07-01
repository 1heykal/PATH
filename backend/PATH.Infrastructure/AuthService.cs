using PATH.Domain.Entities;
using PATH.Domain.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using PATH.Application.Exceptions;
using PATH.Infrastructure;

namespace PATH.Infrastructure
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;


        public AuthService(ApplicationDbContext context, IConfiguration configuration, UserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }


        // Register User
        public async Task RegisterUser(RegisterUserModel model)
        {
            if (await _userService.UserExists(u => u.Email == model.Email))
            {
                throw new AppException("Email already exists.", 404);
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                Username = await GenerateUsername(model.FirstName, model.LastName),
                BirthDate = model.DateOfBirth,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return;

        }


        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        private async Task<string> GenerateUsername(string firstName, string lastName)
        {
            string username;
            bool exists;

            do
            {
                username = $"{firstName.ToLower()}_{lastName.ToLower()}{Random.Shared.Next(100000, 999999)}";
                exists = await _context.Users.AnyAsync(u => u.Username.Equals(username));

            } while (exists);

            return username;
        }



        // Login User -> Access & Refresh Tokens
        public async Task<(string accessToken, string refreshToken, UserBasicInfo userInfo)> LoginUser(AccessModel model)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(model.Email) || u.Username.Equals(model.Email));
            if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) throw new AppException("Invalid User Credentials.", 404);

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };


            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            var userInfo = new UserBasicInfo
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                Role = user.Role
            };

            return (newAccessToken, newRefreshToken, userInfo);

        }

        private string GenerateAccessToken(ApplicationUser user)
        {
            // 1. Header
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Payload

            var claims = new[]
            {
                // Identity
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // we will see future uses of this, Don't forget.


                //permissions
               //new Claim("scope", "read:profile", "write:profile"),
               //new Claim("TokenType", "AccessToken"),

            };

            // 3. Signature
            var token = new JwtSecurityToken(
                issuer: _configuration["jwt:Issuer"],
                audience: _configuration["jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["jwt:ExpiresAfter"])),
                signingCredentials: credentials
                );


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }

        }


        // logout

        public async Task LogoutUser(string refresshToken)
        {
            await _context.RefreshTokens.Where(rt => rt.Token.Equals(refresshToken)).ExecuteUpdateAsync(s => s.SetProperty(rt => rt.IsRevoked, true));
        }


        // access token expired

        public async Task<(string accessToken, string refreshToken, UserBasicInfo userInfo)> RefreshAccessToken(string oldRefreshToken)
        {
            var token = await _context.RefreshTokens.Include(u => u.User).FirstOrDefaultAsync(rt => rt.Token.Equals(oldRefreshToken));
            if (token is null)
                throw new AppException("Invalid or expired refresh token.", 401);


            if (token.IsRevoked)
            {
                var tokens = await _context.RefreshTokens.AsNoTracking().Where(rt => rt.UserId.Equals(token.UserId) && !rt.IsRevoked).ExecuteUpdateAsync(s => s.SetProperty(rt => rt.IsRevoked, true));
                throw new AppException("Invalid or expired refresh token.", 401);
            }

            if (token.ExpiresAt < DateTime.UtcNow)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
                throw new AppException("Invalid or expired refresh token.", 401);
            }

            var newAccessToken = GenerateAccessToken(token.User);
            var newRefreshToken = GenerateRefreshToken();


            token.IsRevoked = true;

            await _context.RefreshTokens.AddAsync(new RefreshToken
            {
                User = token.User,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Token = newRefreshToken,
            });

            var userInfo = new UserBasicInfo
            {
                Id = token.UserId,
                FirstName = token.User.FirstName,
                LastName = token.User.LastName,
                Email = token.User.Email,
                BirthDate = token.User.BirthDate,
                Role = token.User.Role
            };

            await _context.SaveChangesAsync();

            return (newAccessToken, newRefreshToken, userInfo);

        }


    }
}
