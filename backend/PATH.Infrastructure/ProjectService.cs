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

        public async Task<GetProjectByIdDto> AddProject(Guid CreatedById, AddProjectModel model)
        {
            var user = await _userService.GetUserById(CreatedById);
            if (user is null)
            {
                throw new AppException("User not found", 404);
            }

            var newProject = new Project
            {
                CreatedById = CreatedById,
                Description = model.Description,
                Name = model.Name,
            };

            await _dbcontext.Projects.AddAsync(newProject);

            await _dbcontext.SaveChangesAsync();

            return new GetProjectByIdDto
            {
                Id = newProject.Id,
                CreatedAt = newProject.CreatedAt,
                Description = newProject.Description,
                Name = newProject.Name,
                CreatedById = newProject.CreatedById,
                CreatorName = $"{user.FirstName} {user.LastName}",
                Members = new List<AddMemberToProjectResponse>(),
                Tasks = new List<GetTaskItemResponse>()
            };

        }

        public async Task<GetProjectByIdDto> GetProjectById(Guid id)
        {
            return await _dbcontext.Projects.AsNoTracking()
                .Select(p => new GetProjectByIdDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedById = p.CreatedById,
                    CreatorName = $"{p.CreatedBy.FirstName} {p.CreatedBy.LastName}",
                    CreatedAt = p.CreatedAt,

                    Members = p.Members.Select(m => new AddMemberToProjectResponse
                    {
                        ProjectId = m.ProjectId,
                        UserId = m.UserId,
                        UserName = $"{m.User.FirstName} {m.User.LastName}",
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
                .FirstOrDefaultAsync(p => p.Id == id) ?? throw new AppException("Project not found", 404);
        }

        public async Task<List<GetProjectByIdDto>> GetAllProjects()
        {
            return await _dbcontext.Projects.AsNoTracking().Select(p => new GetProjectByIdDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedById = p.CreatedById,
                CreatorName = $"{p.CreatedBy.FirstName} {p.CreatedBy.LastName}",
                CreatedAt = p.CreatedAt,
            }).ToListAsync();
        }

        public async Task<AddMemberToProjectResponse> AddMemberToProject(Guid projectId, AddMemberToProjectModel model)
        {
            var projectExists = await _dbcontext.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                throw new AppException("Project not found", 404);


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

        public async Task DeleteProject(Guid id)
        {
            var deleted = await _dbcontext.Projects.Where(p => p.Id == id).ExecuteDeleteAsync();
            if (deleted == 0)
                throw new AppException("Project not found", 404);
        }

        public async Task<bool> ProjectExists(Expression<Func<Project, bool>> predicate)
        {
            return await _dbcontext.Projects.AnyAsync(predicate);
        }
    }
}
