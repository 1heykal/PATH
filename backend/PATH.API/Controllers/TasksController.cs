using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PATH.Domain;
using PATH.Domain.Entities;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System.Security.Claims;


namespace PATH.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<GetTaskItemResponse>> AddTaskItem(AddTaskModel model)
        {
            var result = await _taskService.AddTaskItem(GetAuthorId(), model);
            return CreatedAtAction(nameof(AddTaskItem), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetTaskItemResponse>> GetTaskById(Guid id)
        {
            var result = await _taskService.GetTaskById(GetAuthorId(), id);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<ActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusModel model)
        {
            await _taskService.UpdateTaskStatus(GetAuthorId(), id, model.NewStatus);
            return NoContent();
        }

        [HttpPatch("{id}/assign")]
        [Authorize]
        public async Task<ActionResult> AssignTask(Guid id, [FromBody] AssignTaskModel model)
        {
            await _taskService.AssignTask(GetAuthorId(), id, model);
            return NoContent();
        }



        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTask(Guid id)
        {
            await _taskService.DeleteTask(GetAuthorId(), id);
            return NoContent();
        }

        private Guid GetAuthorId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}
