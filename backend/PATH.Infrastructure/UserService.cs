using Microsoft.EntityFrameworkCore;
using PATH.Application.Exceptions;
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

        public async Task ChangeUserRole(Guid authorId, ChangeRoleModel model)
        {
            var authorOrgMembership = await GetOrgMembership(authorId, model.OrgId);
            var userOrgMembership = await GetOrgMembership(model.UserId, model.OrgId);

            if (authorOrgMembership.Role != OrganizationRole.Admin)
                throw new AppException("User is not authorized to perform this action.");

            userOrgMembership.Role = model.NewRole;

            await _context.SaveChangesAsync();

        }

        public async Task<List<UserDto>> GetAllUsers(Guid authorId, Guid organizationId)
        {
            var userOrgMembership = await GetOrgMembership(authorId, organizationId);

            if (userOrgMembership.Role != OrganizationRole.Admin)
                throw new AppException("User is not authorized to perform this action.");

            return await _context.OrganizationMembers.AsNoTracking()
                    .Include(o => o.User)
                    .Where(o => o.OrganizationId == organizationId).Select(o => new UserDto
                    {
                        Id = o.User.Id,
                        FirstName = o.User.FirstName,
                        LastName = o.User.LastName,
                        Email = o.User.Email,
                        CreatedAt = o.User.CreatedAt,

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

        private async Task<OrganizationMember> GetOrgMembership(Guid userId, Guid orgId)
        {
            return await _context.OrganizationMembers
                .FirstOrDefaultAsync(om => om.UserId.Equals(userId) && om.OrganizationId.Equals(orgId)) ??
                throw new AppException("User is not a member of the organization", 403);
        }

    }
}
