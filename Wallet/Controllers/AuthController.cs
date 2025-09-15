using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wallet.Entities;
using Wallet.Entities.DTO;
using Wallet.Services;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            var user = await authService.RegisterAsync(request);
            if (user == null) {
                return BadRequest("User Exist.");
            }
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            var token = await authService.LoginAsync(request);
            if (token == null)
            {
                return BadRequest("Invalid Username or Password");
            }
            return Ok(token);
        }

        [Authorize]
        [HttpPost("Test")]
        public IActionResult test()
        {
            return Ok("You are user.");
        }
    }
}
