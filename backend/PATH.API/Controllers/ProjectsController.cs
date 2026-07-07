using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System.Security.Claims;
using System.Security.Cryptography;

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
            var project = await _projectService.GetProjectById(GetAuthorId(), id);
            return Ok(project);
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<GetProjectByIdDto>> AddProject(AddProjectModel model)
        {
            var project = await _projectService.AddProject(GetAuthorId(), model);
            return CreatedAtAction(nameof(AddProject), new { id = project.Id }, project);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<GetProjectByIdDto>>> GetAllProjects(Guid orgId)
        {
            var result = await _projectService.GetAllProjects(GetAuthorId(), orgId);
            return Ok(result);
        }


        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<List<GetTaskItemResponse>>> GetTasksByProjectId(Guid id)
        {
            var result = await _taskService.GetTasksByProjectId(GetAuthorId(), id);
            return Ok(result);
        }

        [HttpPost("{id}/members")]
        [Authorize]
        public async Task<ActionResult<AddMemberToProjectResponse>> AddMemberToProject(Guid id, AddMemberToProjectModel model)
        {
            var result = await _projectService.AddMemberToProject(GetAuthorId(), id, model);
            return CreatedAtAction(nameof(AddMemberToProject), new { id = result.ProjectId }, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteProject(Guid id)
        {
            await _projectService.DeleteProject(GetAuthorId(), id);
            return NoContent();
        }

        private Guid GetAuthorId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}

