using Microsoft.EntityFrameworkCore;
using PATH.Application.Exceptions;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Infrastructure
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;


        public TaskService(ApplicationDbContext context, UserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<GetTaskItemResponse> AddTaskItem(Guid authorId, AddTaskModel model)
        {
            var authorOrgMembership = await GetOrganizationMembershipByProjectId(authorId, model.ProjectId);
            var assignedToMembership = await GetProjectMembership(model.AssignedToId, model.ProjectId) ?? throw new AppException("User is not a member of the project.", 404);


            if (authorOrgMembership.Role != OrganizationRole.Admin && authorOrgMembership.Role != OrganizationRole.Manager)
                throw new AppException("User not authorized to perform this action", 403);

            var user = await _userService.GetUserById(model.AssignedToId);
            if (user is null)
                throw new AppException("User not found", 404);

            var taskItem = new TaskItem(model);

            await _context.TaskItems.AddAsync(taskItem);
            await _context.SaveChangesAsync();

            return new GetTaskItemResponse(taskItem, $"{user.FirstName} {user.LastName}");
        }

        public async Task AssignTask(Guid authorId, Guid taskId, AssignTaskModel model)
        {
            var authorOrgMembership = await GetOrgMembershipForTask(taskId, authorId);
            var assignedToOrgMembership = await GetOrgMembershipForTask(taskId, model.AssignedToId);
            var assignedToProjectMembership = await GetProjectMembershipForTask(taskId, model.AssignedToId);

            if (authorOrgMembership.Role != OrganizationRole.Admin && authorOrgMembership.Role != OrganizationRole.Manager)
                throw new AppException("User not authorized to perform this action", 403);

            var taskItem = await _context.TaskItems.FindAsync(taskId);

            taskItem!.AssignedToId = model.AssignedToId;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTask(Guid authorId, Guid taskId)
        {
            var orgMembership = await GetOrgMembershipForTask(taskId, authorId);
            var task = await _context.TaskItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == taskId);

            if (task!.AssignedToId != authorId && orgMembership.Role != OrganizationRole.Admin)
                throw new AppException("You are not authorized to delete this task.", 403);

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

        }

        public async Task<List<GetTaskItemResponse>> GetAllTasks(Guid authorId, Priority? priority, Status? status)
        {

            var query = _context.TaskItems.AsQueryable();

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            return await query.Select(t => GetTaskItemResponse.FromEntity(t)).AsNoTracking().ToListAsync();
        }

        public async Task<GetTaskItemResponse> GetTaskById(Guid authorId, Guid taskId)
        {
            var orgMembership = await GetOrgMembershipForTask(taskId, authorId);
            return await _context.TaskItems.Select(t => GetTaskItemResponse.FromEntity(t)).AsNoTracking().FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new AppException("Task not found", 404);
        }

        public async Task<List<GetTaskItemResponse>> GetTasksByProjectId(Guid authorId, Guid projectId)
        {
            var orgMembership = await GetProjectMembershipForTask(authorId, projectId);
            return await _context.TaskItems.Where(t => t.ProjectId == projectId).Select(t => GetTaskItemResponse.FromEntity(t)).AsNoTracking().ToListAsync();
        }



        public async Task UpdateTaskStatus(Guid authorId, Guid taskId, Status newStatus)
        {

            var orgMembership = await GetOrgMembershipForTask(taskId, authorId);
            var task = await _context.TaskItems.FindAsync(taskId);

            if (task!.AssignedToId != authorId && orgMembership.Role != OrganizationRole.Admin && orgMembership.Role != OrganizationRole.Manager)
                throw new AppException("You are not authorized to update this task.", 403);

            task.Status = newStatus;

            await _context.SaveChangesAsync();
        }


        private async Task<OrganizationMember> GetOrgMembershipForTask(Guid taskId, Guid userId)
        {
            var user = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!user)
                throw new AppException("User not found", 404);

            return await _context.TaskItems
                 .Where(t => t.Id == taskId)
                 .Select(t => _context.OrganizationMembers.FirstOrDefault(o => o.OrganizationId == t.Project.OrganizationId && o.UserId == userId))
                 .FirstOrDefaultAsync() ?? throw new AppException("User is not authorized to perform this action.", 403);
        }

        private async Task<ProjectMember> GetProjectMembershipForTask(Guid taskId, Guid userId)
        {
            var user = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!user)
                throw new AppException("User not found", 404);

            return await _context.TaskItems
                 .Where(t => t.Id == taskId)
                 .Select(t => _context.ProjectMembers.FirstOrDefault(p => p.ProjectId == t.ProjectId && p.UserId == userId))
                 .FirstOrDefaultAsync() ?? throw new AppException("User is not authorized to perform this action.", 403);
        }

        private async Task<OrganizationMember> GetOrganizationMembership(Guid userId, Guid orgId)
        {

            return await _context.OrganizationMembers.AsNoTracking()
                .FirstOrDefaultAsync(o => o.UserId.Equals(userId) && o.OrganizationId.Equals(orgId))
                ?? throw new AppException("User is not a member of the organization.", 403);
        }

        private async Task<ProjectMember?> GetProjectMembership(Guid authorId, Guid projectId)
        {
            var user = await _context.Users.AnyAsync(u => u.Id == authorId);
            if (!user)
                throw new AppException("User not found", 404);

            return await _context.ProjectMembers.AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.UserId.Equals(authorId) && pm.ProjectId.Equals(projectId));
        }

        private async Task<OrganizationMember> GetOrganizationMembershipByProjectId(Guid userId, Guid projectId)
        {
            return await _context.Projects
                .Where(p => p.Id.Equals(projectId))
                .Select(p => _context.OrganizationMembers.FirstOrDefault(om => om.UserId.Equals(userId) && om.OrganizationId.Equals(p.OrganizationId)))
                .FirstOrDefaultAsync() ?? throw new AppException("User is not a member of this organization.", 403);
        }

        // What differs project members from org members, is org admin allowed to mkae changes to project he is not a member of (like delete task etc)

    }
}
