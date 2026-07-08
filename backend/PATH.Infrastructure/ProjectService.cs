using Microsoft.EntityFrameworkCore;
using PATH.Application.Exceptions;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PATH.Infrastructure
{
    public class ProjectService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserService _userService;

        public ProjectService(ApplicationDbContext dbContext, UserService userService)
        {
            _dbcontext = dbContext;
            _userService = userService;
        }

        public async Task<GetProjectByIdDto> AddProject(Guid authorId, AddProjectModel model)
        {

            var orgMembership = await GetOrgMembershipForProject(authorId, model.OrganizationId);

            if (orgMembership.Role != OrganizationRole.Admin && orgMembership.Role != OrganizationRole.Manager)
                throw new AppException("User not authorized to perform this action", 403);

            var user = await _userService.GetUserById(authorId) ?? throw new AppException("User not found", 404);

            var newProject = new Project(authorId, model);


            await _dbcontext.Projects.AddAsync(newProject);
            await _dbcontext.SaveChangesAsync();

            var projectMember = new ProjectMember
            {
                UserId = authorId,
                ProjectId = newProject.Id
            };

            await _dbcontext.ProjectMembers.AddAsync(projectMember);
            await _dbcontext.SaveChangesAsync();



            return new GetProjectByIdDto
            {
                Id = newProject.Id,
                CreatedAt = newProject.CreatedAt,
                Description = newProject.Description,
                Name = newProject.Name,
                CreatedById = newProject.CreatedById,
                CreatorName = $"{user.FirstName} {user.LastName}",
                OrganizationId = newProject.OrganizationId,
                CurrentUserRole = orgMembership.Role,
            };

        }

        public async Task<GetProjectByIdDto> GetProjectById(Guid authorId, Guid projectId)
        {
            var orgMembership = await _dbcontext.Projects
               .Where(p => p.Id.Equals(projectId))
               .Select(p => _dbcontext.OrganizationMembers.FirstOrDefault(om => om.OrganizationId.Equals(p.OrganizationId) && om.UserId.Equals(authorId)))
               .FirstOrDefaultAsync() ?? throw new AppException("User is not authorized to perform this action.", 403);

            return await _dbcontext.Projects.AsNoTracking()
                .Select(p => new GetProjectByIdDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedById = p.CreatedById,
                    CreatorName = $"{p.CreatedBy.FirstName} {p.CreatedBy.LastName}",
                    CreatedAt = p.CreatedAt,
                    OrganizationId = p.OrganizationId,
                    CurrentUserRole = orgMembership.Role,

                    Members = p.Members.Select(m => new ProjectMemberBasicInfo
                    {
                        ProjectId = m.ProjectId,
                        MemberId = m.UserId,
                        MemberName = $"{m.User.FirstName} {m.User.LastName}",
                        JoinedAt = m.JoinedAt
                    }).ToList(),

                    Tasks = p.Tasks.Select(t => new GetTaskItemResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        Status = t.Status,
                        Priority = t.Priority,
                        DueDate = t.DueDate,
                        AssignedToName = $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}",
                        CreatedAt = t.CreatedAt,
                        AssignedToId = t.AssignedToId,
                        ProjectId = t.ProjectId

                    }).ToList()
                })
                .FirstOrDefaultAsync(p => p.Id == projectId) ?? throw new AppException("Project not found", 404);

        }

        public async Task<List<GetProjectByIdDto>> GetAllProjects(Guid authorId, Guid organizationId)
        {
            var orgMembership = await GetOrgMembershipForProject(authorId, organizationId);

            return await _dbcontext.Projects
                .Where(p => p.OrganizationId.Equals(organizationId))
                .AsNoTracking().Select(p => new GetProjectByIdDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedById = p.CreatedById,
                    CreatorName = $"{p.CreatedBy.FirstName} {p.CreatedBy.LastName}",
                    CreatedAt = p.CreatedAt,
                    OrganizationId = p.OrganizationId,
                    CurrentUserRole = orgMembership.Role,

                }).ToListAsync();
        }

        public async Task<AddMemberToProjectResponse> AddMemberToProject(Guid authorId, Guid projectId, AddMemberToProjectModel model)
        {

            var orgMembership = await _dbcontext.Projects
                .Where(p => p.Id.Equals(projectId))
                .Select(p => _dbcontext.OrganizationMembers.FirstOrDefault(om => om.OrganizationId.Equals(p.OrganizationId) && om.UserId.Equals(authorId)))
                .FirstOrDefaultAsync() ?? throw new AppException("User is not authorized to perform this action.", 403);

            if (orgMembership.Role != OrganizationRole.Admin)
                throw new AppException("User not authorized to perform this action", 403);


            var user = await _userService.GetUserById(model.UserId);
            if (user is null)
                throw new AppException("User not found", 404);

            var member = await _dbcontext.ProjectMembers.AsNoTracking().FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == model.UserId);
            if (member is not null)
                throw new AppException("User is already a member of the project", 400);

            var projectMember = new ProjectMember
            {
                UserId = model.UserId,
                ProjectId = projectId
            };

            await _dbcontext.ProjectMembers.AddAsync(projectMember);
            await _dbcontext.SaveChangesAsync();

            return new AddMemberToProjectResponse
            {
                ProjectId = projectMember.ProjectId,
                UserId = projectMember.UserId,
                UserName = $"{user.FirstName} {user.LastName}",
                JoinedAt = projectMember.JoinedAt
            };
        }

        public async Task DeleteProject(Guid authorId, Guid projectId)
        {
            var orgMembership = await _dbcontext.Projects
               .Where(p => p.Id.Equals(projectId))
               .Select(p => _dbcontext.OrganizationMembers.FirstOrDefault(om => om.OrganizationId.Equals(p.OrganizationId) && om.UserId.Equals(authorId)))
               .FirstOrDefaultAsync() ?? throw new AppException("User is not authorized to perform this action.", 403);

            if (orgMembership.Role != OrganizationRole.Admin)
                throw new AppException("User not authorized to perform this action", 403);

            var deleted = await _dbcontext.Projects.Where(p => p.Id == projectId).ExecuteDeleteAsync();
            if (deleted == 0)
                throw new AppException("Project not found", 404);
        }

        public async Task<bool> ProjectExists(Expression<Func<Project, bool>> predicate)
        {
            return await _dbcontext.Projects.AnyAsync(predicate);
        }

        private async Task<OrganizationMember> GetOrgMembershipForProject(Guid userId, Guid organizationId)
        {
            return await _dbcontext.OrganizationMembers.AsNoTracking()
                .FirstOrDefaultAsync(om => om.OrganizationId.Equals(organizationId) && om.UserId.Equals(userId)) ??
                throw new AppException("User not Authorized to perfom this action.");
        }

        private async Task<ProjectMember> GetProjectMembership(Guid userId, Guid projectId)
        {
            return await _dbcontext.ProjectMembers.AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.UserId.Equals(userId) && pm.ProjectId.Equals(projectId))
                ?? throw new AppException("User is not a member of the project.", 404);
        }


    }
}
