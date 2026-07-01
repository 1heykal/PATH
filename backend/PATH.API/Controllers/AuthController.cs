using PATH.Domain.Models;
using PATH.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PATH.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }


        [HttpPost("Register")]
        public async Task<ActionResult> RegisterUser(RegisterUserModel model)
        {
            await _authService.RegisterUser(model);
            return Ok(new { message = "User registered successfully." });
        }


        [HttpPost("Login")]
        public async Task<ActionResult> LoginUser(AccessModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.LoginUser(model);
            SetRefreshCookie(refreshToken);
            return Ok(new { accessToken, user });
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var (accessToken, newRefreshToken, user) = await _authService.RefreshAccessToken(refreshToken);

            SetRefreshCookie(newRefreshToken);

            return Ok(new { accessToken, user });
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
                await _authService.LogoutUser(refreshToken);

            Response.Cookies.Delete("refreshToken");
            return Ok();
        }



        private void SetRefreshCookie(string newRefreshToken)
        {
            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(30)
            });
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test() => Ok("authenticated");



    }
}
