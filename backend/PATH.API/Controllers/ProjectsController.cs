using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System.Security.Claims;

namespace PATH.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;

        public ProjectsController(ProjectService projectService, TaskService taskService)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProjectByIdDto>> GetProjectById(Guid id)
        {
            var project = await _projectService.GetProjectById(id);
            return Ok(project);
        }

        [HttpPost]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<GetProjectByIdDto>> AddProject(AddProjectModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var project = await _projectService.AddProject(userId, model);
            return CreatedAtAction(nameof(AddProject), new { id = project.Id }, project);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<GetProjectByIdDto>>> GetAllProjects()
        {
            var result = await _projectService.GetAllProjects();
            return Ok(result);
        }


        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<List<GetTaskItemResponse>>> GetTasksByProjectId(Guid id)
        {
            var result = await _taskService.GetTasksByProjectId(id);
            return Ok(result);
        }

        [HttpPost("{id}/members")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AddMemberToProjectResponse>> AddMemberToProject(Guid id, AddMemberToProjectModel model)
        {
            var result = await _projectService.AddMemberToProject(id, model);
            return CreatedAtAction(nameof(AddMemberToProject), new { id = result.ProjectId }, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteProject(Guid id)
        {
            await _projectService.DeleteProject(id);
            return NoContent();
        }
    }
}

