using Microsoft.EntityFrameworkCore;
using PATH.Application.Exceptions;
using PATH.Domain;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Infrastructure
{
    public class OrganizationService
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;

        public OrganizationService(UserService userService, ApplicationDbContext context)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AddMemberToOrganizationResponse> AddMemberToOrganization(Guid adminId, AddMemberToOrganizationModel model)
        {
            // check if the admin exists as organization admin
            var organization = await _context.Organizations
                .Include(o => o.Members)
                .FirstOrDefaultAsync(o => o.Id == model.OrganizationId && o.Members.Any(m => m.UserId == adminId && m.Role == OrganizationRole.Admin));

            if (organization is null)
                throw new AppException("Organization not found or not an admin of the organization", 404);


            var user = _context.Users.FirstOrDefault(u => u.Email == model.UserEmail);
            if (user is null)
                throw new AppException($"User with email {model.UserEmail} not found.", 404);

            var existingMember = organization.Members.FirstOrDefault(m => m.UserId == user.Id);
            if (existingMember is not null)
                throw new AppException($"User with email {model.UserEmail} is already a member of the organization.", 400);

            var organizationMember = new OrganizationMember
            {
                OrganizationId = organization.Id,
                UserId = user.Id,
            };

            await _context.OrganizationMembers.AddAsync(organizationMember);
            await _context.SaveChangesAsync();

            return new AddMemberToOrganizationResponse
            {
                Id = organizationMember.Id,
                OrganizationId = organizationMember.OrganizationId,
                UserId = organizationMember.UserId,
                MemberName = $"{user.FirstName} {user.LastName}",
                Role = organizationMember.Role,
                JoinedAt = organizationMember.JoinedAt,
                OrganizationName = organization.Name,
                UserEmail = user.Email
            };

        }

        public async Task<CreateOrganizationResponse> CreateOrganization(Guid userId, CreateOrganizationModel model)
        {
            var userExists = await _userService.UserExists(u => u.Id == userId);
            if (!userExists)
                throw new AppException($"User with ID {userId} not found.", 404);


            var organization = new Organization
            {
                Name = model.Name,
                CreatedById = userId,
            };

            await _context.Organizations.AddAsync(organization);
            await _context.OrganizationMembers.AddAsync(new OrganizationMember
            {
                OrganizationId = organization.Id,
                UserId = userId,
                Role = OrganizationRole.Admin,
            });

            await _context.SaveChangesAsync();

            return new CreateOrganizationResponse
            {
                Id = organization.Id,
                Name = organization.Name,
                CreatedById = organization.CreatedById,
                CreatedAt = organization.CreatedAt
            };

        }

        public async Task<List<OrganizationBasicInfo>> GetAllOrganizations(Guid userId)
        {
            return await _context.Organizations
                 .Where(o => o.Members.Any(om => om.UserId == userId))
                 //  .Include(o => o.Members)
                 .Select(o => new OrganizationBasicInfo
                 {
                     Id = o.Id,
                     Name = o.Name,
                     CreatorName = $"{o.CreatedBy.FirstName} {o.CreatedBy.LastName}",
                     CreatedAt = o.CreatedAt

                 }).ToListAsync();
        }

        public async Task<GetOrganizationByIdModel> GetOrganizationById(Guid userId, Guid id)
        {
            var user = await _userService.GetUserById(userId);
            if (user is null)
                throw new AppException($"User with ID {userId} not found.", 404);

            var organization = await _context.Organizations
                .Include(o => o.Members)
                .ThenInclude(m => m.User)
                .Include(o => o.Projects)
                .FirstOrDefaultAsync(o => o.Id == id && o.Members.Any(m => m.UserId == userId));

            if (organization is null)
                throw new AppException($"Organization with ID {id} not found or user is not a member.", 404);

            var currentUserRole = organization.Members.FirstOrDefault(m => m.UserId == userId)?.Role ?? OrganizationRole.Member;
            return new GetOrganizationByIdModel
            {
                Id = organization.Id,
                Name = organization.Name,
                CreatedById = organization.CreatedById,
                CreatedAt = organization.CreatedAt,
                CreatorName = organization.Members.FirstOrDefault(m => m.UserId == organization.CreatedById)?.User?.FirstName + " " + organization.Members.FirstOrDefault(m => m.UserId == organization.CreatedById)?.User?.LastName ?? "Unknown",
                CurrentUserRole = currentUserRole,
                Members = organization.Members.Select(m => new GetOrganizationMembers
                {
                    UserId = m.UserId,
                    Role = m.Role,
                    Name = $"{m.User.FirstName} {m.User.LastName}",
                    Id = m.UserId,
                    JoinedAt = m.JoinedAt,
                    Email = m.User.Email

                }).ToList(),
                Projects = organization.Projects.Select(p => new ProjectBasicInfo
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    CreatorName = organization.Members.FirstOrDefault(m => m.UserId == p.CreatedById)?.User?.FirstName + " " + organization.Members.FirstOrDefault(m => m.UserId == p.CreatedById)?.User?.LastName ?? "Unknown",

                }).ToList()
            };
        }

        public async Task<UserOrgMembership> GetUserOrgMembership(Guid userId, Guid organizationId)
        {
            var membership = await _context.OrganizationMembers
                .Include(om => om.User)
                .FirstOrDefaultAsync(om => om.UserId == userId && om.OrganizationId == organizationId);

            if (membership is null)
                throw new AppException("User is not a member of the organization.", 404);

            return new UserOrgMembership
            {
                OrganizationId = membership.OrganizationId,
                UserId = membership.UserId,
                Role = membership.Role,
                Permissions = new UserOrgPermissions
                {
                    CanCreateProject = membership.Role == OrganizationRole.Admin || membership.Role == OrganizationRole.Manager,
                    CanAddMembers = membership.Role == OrganizationRole.Admin,
                    CanAssignTasks = membership.Role == OrganizationRole.Admin || membership.Role == OrganizationRole.Manager,
                },
            };
        }

        public async Task<List<OrganizationMemberBasicInfo>> GetOrganizationMembers(Guid authorId, Guid orgId)
        {
            var membership = await _context.OrganizationMembers
                .Where(om => om.UserId.Equals(authorId) && om.OrganizationId.Equals(orgId))
                .FirstOrDefaultAsync() ?? throw new AppException("User is not a member of the organization.", 403);

            return await _context.OrganizationMembers.Where(om => om.OrganizationId.Equals(orgId))
                .Include(om => om.User)
                .Select(om => new OrganizationMemberBasicInfo
                {
                    Id = om.Id,
                    JoinedAt = om.JoinedAt,
                    Name = $"{om.User.FirstName} {om.User.LastName}",
                    Email = om.User.Email,
                    Role = om.Role,
                    UserId = om.UserId
                }).ToListAsync();
        }

        public async Task RemoveMemberFromOrganization(Guid adminId, Guid organizationId, Guid userId)
        {
            var organizationMembership = await _context.OrganizationMembers
                .Include(om => om.Organization)
                .FirstOrDefaultAsync(om => om.OrganizationId == organizationId && om.UserId == adminId && om.Role == OrganizationRole.Admin);

            if (organizationMembership == null)
                throw new AppException("You are forbidden from performing this action", 403);

            var memberToRemove = await _context.OrganizationMembers.FirstOrDefaultAsync(om => om.UserId.Equals(userId));
            if (memberToRemove == null)
                throw new AppException("Member not found", 404);

            var adminCount = await _context.OrganizationMembers.CountAsync(om => om.OrganizationId == organizationId && om.Role == OrganizationRole.Admin);
            if (adminCount == 1 && memberToRemove.Role == OrganizationRole.Admin)
                throw new AppException("Cannot remove the last admin from the organization", 400);

            _context.OrganizationMembers.Remove(memberToRemove);
            await _context.SaveChangesAsync();
        }
    }
}

