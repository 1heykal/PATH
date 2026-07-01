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
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<GetTaskItemResponse>> AddTaskItem(AddTaskModel model)
        {
            var result = await _taskService.AddTaskItem(model);
            return CreatedAtAction(nameof(AddTaskItem), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetTaskItemResponse>> GetTaskById(Guid id)
        {
            var result = await _taskService.GetTaskById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetTaskItemResponse>>> GetAllTasks(Priority? priority, Status? status)
        {
            var result = await _taskService.GetAllTasks(priority, status);
            return Ok(result);
        }


        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<ActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _taskService.UpdateTaskStatus(id, model.NewStatus, userId);
            return NoContent();
        }

        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult> AssignTask(Guid id, [FromBody] AssignTaskModel model)
        {
            await _taskService.AssignTask(id, model);
            return NoContent();
        }



        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTask(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _taskService.DeleteTask(id, userId);
            return NoContent();
        }

    }
}
