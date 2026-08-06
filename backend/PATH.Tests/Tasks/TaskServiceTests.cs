using Moq;
using PATH.Application.Exceptions;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Tests.Tasks
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task CreateTask_AuthorizedUser_CreatesTask()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = await CreateApplicationUser(context);

            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var projectMember = await CreateProjectMember(context, user.Id, project.Id);
            await context.SaveChangesAsync();


            var taskModel = new AddTaskModel
            {
                Title = "Test Task",
                Description = "This is a test task.",
                AssignedToId = user.Id,
                ProjectId = project.Id,
                Status = Status.Todo,
                Priority = Priority.Medium
            };


            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);

            // Act
            var taskItem = await taskService.AddTaskItem(user.Id, taskModel);

            // Assert
            Assert.NotNull(taskItem);
            Assert.Equal("Test Task", taskItem.Title);
            Assert.Equal("This is a test task.", taskItem.Description);
            Assert.Equal(user.Id, taskItem.AssignedToId);
        }

        [Fact]
        public async Task CreateTask_UnauthorizedUser_ThrowsAppException()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = await CreateApplicationUser(context);

            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            await context.SaveChangesAsync();


            var taskModel = new AddTaskModel
            {
                Title = "Test Task",
                Description = "This is a test task.",
                AssignedToId = user.Id,
                ProjectId = project.Id,
                Status = Status.Todo,
                Priority = Priority.Medium
            };
            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);
            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => taskService.AddTaskItem(user.Id, taskModel));
        }

        [Fact]
        public async Task AssignTask_AuthorizedUser_AssignsTask()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = await CreateApplicationUser(context);

            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var projectMember = await CreateProjectMember(context, user.Id, project.Id);
            var task = await CreateTask(context, user.Id, project.Id);

            var anotherUser = await CreateApplicationUser(context);

            var anotherOrganizationMember = await CreateOrganizationMember(context, anotherUser.Id, organization.Id, OrganizationRole.Member);

            var anotherProjectMember = await CreateProjectMember(context, anotherUser.Id, project.Id);
            await context.SaveChangesAsync();



            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);

            // Act
            var assignTaskModel = new AssignTaskModel
            {
                AssignedToId = anotherUser.Id
            };
            await taskService.AssignTask(user.Id, task.Id, assignTaskModel);

            // Assert
            var updatedTask = await context.TaskItems.FindAsync(task.Id);

            Assert.NotNull(updatedTask);
            Assert.Equal(anotherUser.Id, updatedTask!.AssignedToId);
        }

        [Fact]
        public async Task AssignTask_UnauthorizedUser_ThrowsAppException()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = await CreateApplicationUser(context);

            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var projectMember = await CreateProjectMember(context, user.Id, project.Id);

            var task = await CreateTask(context, user.Id, project.Id);
            var anotherUser = await CreateApplicationUser(context);
            var anotherOrganizationMember = await CreateOrganizationMember(context, anotherUser.Id, organization.Id, OrganizationRole.Member);
            var anotherProjectMember = await CreateProjectMember(context, anotherUser.Id, project.Id);
            await context.SaveChangesAsync();



            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);

            // Act
            // Assert
            var assignTaskModel = new AssignTaskModel
            {
                AssignedToId = anotherUser.Id
            };
            await Assert.ThrowsAsync<AppException>(() => taskService.AssignTask(anotherUser.Id, task.Id, assignTaskModel));
        }

        [Fact]
        public async Task UpdateStatus_AuthorizedUser_UpdatesStatus()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = await CreateApplicationUser(context);

            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var task = await CreateTask(context, user.Id, project.Id);
            await context.SaveChangesAsync();

            // Act
            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);
            await taskService.UpdateTaskStatus(user.Id, task.Id, Status.InProgress);
            // Assert
            Assert.Equal(Status.InProgress, task.Status);


        }

        [Fact]
        public async Task UpdateStatus_UnauthorizedUser_ThrowsAppException()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = await CreateApplicationUser(context);

            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var task = await CreateTask(context, user.Id, project.Id);

            var anotherUser = await CreateApplicationUser(context);
            var anotherOrganizationMember = await CreateOrganizationMember(context, anotherUser.Id, organization.Id, OrganizationRole.Member);
            var anotherProjectMember = await CreateProjectMember(context, anotherUser.Id, project.Id);
            await context.SaveChangesAsync();
            // Act
            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);
            // Assert
            await Assert.ThrowsAsync<AppException>(() => taskService.UpdateTaskStatus(anotherUser.Id, task.Id, Status.InProgress));


        }

        [Fact]
        public async Task DeleteTask_AuthorizedUser_DeletesTask()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();
            var user = await CreateApplicationUser(context);
            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var task = await CreateTask(context, user.Id, project.Id);
            await context.SaveChangesAsync();

            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);
            context.ChangeTracker.Clear();

            // Act
            await taskService.DeleteTask(user.Id, task.Id);

            // Assert
            var deletedTask = await context.TaskItems.FindAsync(task.Id);
            Assert.Null(deletedTask);

        }

        [Fact]
        public async Task DeleteTask_UnauthorizedUser_ThrowsAppException()
        {
            // Arrange
            var context = ApplicationDbContext.GetSqliteContext();

            var user = await CreateApplicationUser(context);

            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var task = await CreateTask(context, user.Id, project.Id);

            var anotherUser = await CreateApplicationUser(context);
            var anotherOrganizationMember = await CreateOrganizationMember(context, anotherUser.Id, organization.Id, OrganizationRole.Member);
            await context.SaveChangesAsync();

            var taskService = new TaskService(context, new UserService(context), new Mock<INotificationService>().Object);
            context.ChangeTracker.Clear();

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => taskService.DeleteTask(anotherUser.Id, task.Id));

        }

        private static async Task<ApplicationUser> CreateApplicationUser(ApplicationDbContext context)
        {
            var user = new ApplicationUser
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword",
                BirthDate = new DateOnly(1990, 1, 1)
            };
            await context.Users.AddAsync(user);

            return user;
        }
        private static async Task<Organization> CreateOrganization(ApplicationDbContext context, Guid userId)
        {
            var organization = new Organization
            {
                Name = "Test Organization",
                CreatedById = userId
            };
            await context.Organizations.AddAsync(organization);
            return organization;
        }

        private static async Task<Project> CreateProject(ApplicationDbContext context, Guid userId, Guid organizationId)
        {
            var project = new Project
            {
                Name = "Test Project",
                Description = "This is a test project.",
                OrganizationId = organizationId,
                CreatedById = userId
            };
            await context.Projects.AddAsync(project);
            return project;
        }

        private static async Task<TaskItem> CreateTask(ApplicationDbContext context, Guid userId, Guid projectId)
        {
            var task = new TaskItem
            {
                Title = "Test Task",
                Description = "This is a test task.",
                ProjectId = projectId,
                AssignedToId = userId,
                Status = Status.Todo,
                Priority = Priority.Medium
            };
            await context.TaskItems.AddAsync(task);
            return task;
        }

        private static async Task<OrganizationMember> CreateOrganizationMember(ApplicationDbContext context, Guid userId, Guid organizationId, OrganizationRole role)
        {
            var organizationMember = new OrganizationMember
            {
                UserId = userId,
                OrganizationId = organizationId,
                Role = role
            };
            await context.OrganizationMembers.AddAsync(organizationMember);
            return organizationMember;
        }

        private static async Task<ProjectMember> CreateProjectMember(ApplicationDbContext context, Guid userId, Guid projectId)
        {
            var projectMember = new ProjectMember
            {
                UserId = userId,
                ProjectId = projectId
            };
            await context.ProjectMembers.AddAsync(projectMember);
            return projectMember;
        }

        private static async Task<(ApplicationUser User, Organization Organization, Project Project)> CreateUserOrganizationProject(ApplicationDbContext context)
        {
            var user = await CreateApplicationUser(context);
            var organization = await CreateOrganization(context, user.Id);
            var organizationMember = await CreateOrganizationMember(context, user.Id, organization.Id, OrganizationRole.Admin);
            var project = await CreateProject(context, user.Id, organization.Id);
            var projectMember = await CreateProjectMember(context, user.Id, project.Id);

            return (user, organization, project);
        }
    }
}


