using Microsoft.EntityFrameworkCore;
using PATH.Domain;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using System.Linq.Expressions;

namespace PATH.Infrastructure
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ChangeUserRole(ChangeRoleModel model)
        {
            if (Shared.Roles.Contains(model.NewRole.ToLower()))
            {
                var user = await _context.Users.FindAsync(model.UserId);
                if (user is null)
                    throw new ApplicationException("There is no user with the specified id.");

                user.Role = model.NewRole;

                await _context.SaveChangesAsync();
                return;

            }
            throw new ApplicationException("Invalid role. Please provide a valid role.");

        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            return await _context.Users.AsNoTracking().Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            }).ToListAsync();

        }

        public async Task<ApplicationUser?> GetUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UserExists(Expression<Func<ApplicationUser, bool>> condition)
        {
            return await _context.Users.AnyAsync(condition);
        }

    }
}
