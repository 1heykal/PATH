using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PATH.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        public AdminController()
        {

        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            return Ok("This is your Dashboard - for admins only.");
        }

    }
}
