using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PATH.Domain;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System.Security.Claims;

namespace PATH.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService userService) { _userService = userService ?? throw new ArgumentNullException(nameof(userService)); }

        [HttpPatch("role")]
        [Authorize]
        public async Task<IActionResult> ChangeUserRole(ChangeRoleModel model)
        {
            await _userService.ChangeUserRole(GetAuthorId(), model);
            return NoContent();
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers(Guid organizationId)
        {
            var users = await _userService.GetAllUsers(GetAuthorId(), organizationId);
            return Ok(users);
        }

        private Guid GetAuthorId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}

