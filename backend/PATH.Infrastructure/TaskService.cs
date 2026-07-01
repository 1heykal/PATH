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

        public async Task<GetTaskItemResponse> AddTaskItem(AddTaskModel model)
        {
            var project = await _context.Projects.AsNoTracking().Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == model.ProjectId);
            if (project is null)
                throw new AppException("Project not found", 404);

            var user = await _userService.GetUserById(model.AssignedToId);
            if (user is null)
                throw new AppException("User not found", 404);

            if (project.Members.All(m => m.UserId != model.AssignedToId))
                throw new AppException("User is not a member of the project.", 400);



            var taskItem = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Priority = model.Priority,
                DueDate = model.DueDate,
                AssignedToId = model.AssignedToId,
                ProjectId = model.ProjectId
            };

            await _context.TaskItems.AddAsync(taskItem);
            await _context.SaveChangesAsync();

            return new GetTaskItemResponse
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Status = taskItem.Status,
                Priority = taskItem.Priority,
                DueDate = taskItem.DueDate,
                AssignedToId = taskItem.AssignedToId,
                AssignedToName = $"{user.FirstName} {user.LastName}",
                CreatedAt = taskItem.CreatedAt,
                ProjectId = taskItem.ProjectId
            };
        }

        public async Task AssignTask(Guid id, AssignTaskModel model)
        {
            var taskItem = await _context.TaskItems.Include(t => t.Project)
                .ThenInclude(p => p.Members)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (taskItem is null)
                throw new AppException("Task not found", 404);

            var userExists = await _userService.UserExists(u => u.Id == model.AssignedToId);
            if (!userExists)
                throw new AppException("User not found", 404);

            if (taskItem.Project.Members.All(m => m.UserId != model.AssignedToId))
                throw new AppException("User is not a member of the project.", 400);

            taskItem.AssignedToId = model.AssignedToId;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTask(Guid id, Guid userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                throw new AppException("User not found", 404);

            var taskItem = await _context.TaskItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (taskItem is null)
                throw new AppException("Task not found", 404);

            if (taskItem.AssignedToId != userId && user.Role != "admin")
                throw new AppException("You are not authorized to delete this task.", 403);

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

        }

        public async Task<List<GetTaskItemResponse>> GetAllTasks(Priority? priority, Status? status)
        {
            var query = _context.TaskItems.AsQueryable();

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            return await query.Select(t => GetTaskItemResponse.FromEntity(t)).AsNoTracking().ToListAsync();
        }

        public async Task<GetTaskItemResponse> GetTaskById(Guid id)
        {
            return await _context.TaskItems.Select(t => GetTaskItemResponse.FromEntity(t)).AsNoTracking().FirstOrDefaultAsync(t => t.Id == id) ?? throw new AppException("Task not found", 404);
        }

        public async Task<List<GetTaskItemResponse>> GetTasksByProjectId(Guid projectId)
        {
            return await _context.TaskItems.Where(t => t.ProjectId == projectId).Select(t => GetTaskItemResponse.FromEntity(t)).AsNoTracking().ToListAsync();
        }



        public async Task UpdateTaskStatus(Guid id, Status newStatus, Guid userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                throw new AppException("User not found", 404);

            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem is null)
                throw new AppException("Task not found", 404);

            if (taskItem.AssignedToId != userId && user.Role != "admin" && user.Role != "manager")
                throw new AppException("You are not authorized to update this task.", 403);

            taskItem.Status = newStatus;

            await _context.SaveChangesAsync();
        }
    }
}
