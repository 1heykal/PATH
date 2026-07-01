using PATH.Domain.Models;
using PATH.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PATH.Domain;

namespace PATH.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService userService) { _userService = userService ?? throw new ArgumentNullException(nameof(userService)); }

        [HttpPatch("role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeUserRole(ChangeRoleModel model)
        {
            await _userService.ChangeUserRole(model);
            return NoContent();
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }


    }
}

