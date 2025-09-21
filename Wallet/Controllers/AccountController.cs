using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Wallet.Data;
using Wallet.Entities;
using Wallet.Entities.DTO;
using Wallet.Services;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(WalletDbContext context, IConfiguration configuration, IAccountService AccountService) : ControllerBase
    {

        [Authorize]
        [HttpPost("NewAccount")]
        public async Task<IActionResult> CreateAccount(AccountDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var account = await AccountService.CreateAccountAsync(dto, userId);
            if (account == null)
            {
                return BadRequest("Account Exist.");
            }
            return Ok(account);
        }

        [Authorize]
        [HttpPost("Deposit")]
        public async Task<IActionResult> AddBalance(BalanceDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var account = await AccountService.AddBalanceAsync(dto, userId);
            if (account == null)
            {
                return BadRequest("Account doesn't exist");
            }
            return Ok(account);
        }

        [Authorize]
        [HttpPost("Withdraw")]
        public async Task<IActionResult> WithdrawBalance(BalanceDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var account = await AccountService.WithdrawBalanceAsync(dto, userId);
         if(account == null)
            {
                return BadRequest("Account doesn't exitst.");
            } 
            return Ok(account);
        }

    }
}
