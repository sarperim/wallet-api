using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
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
        public async Task<ActionResult<TokenDTO>> Login(UserDTO request)
        {
            var tokens = await authService.LoginAsync(request);
            if (tokens == null)
            {
                return BadRequest("Invalid Username or Password");
            }
            return Ok(tokens);
        }

        [Authorize]
        [HttpPost("Test")]
        public IActionResult test()
        {
            return Ok("You are user.");
        }

        [HttpPost("RefreshToken")]

        public async Task<ActionResult<TokenDTO>> RefreshToken(RefreshTokenDTO request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("invalid token");
            }
            return Ok(result);
        }

    }
}
