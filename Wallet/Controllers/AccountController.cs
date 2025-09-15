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

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(WalletDbContext context, IConfiguration configuration) : ControllerBase
    {
        [Authorize]
        [HttpPost("NewAccount")]
        public async Task<IActionResult> CreateAccount(AccountDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var existingAccount = await context.Accounts
                   .FirstOrDefaultAsync(a => a.UserId == userId && a.CurrencyType == dto.CurrencyType);

            if (existingAccount != null)
                return BadRequest("Account with this currency already exists.");

            var account = new Account
            {
                UserId = userId,
                CurrencyType = dto.CurrencyType,
            };
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
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
            var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId && dto.CurrencyType == x.CurrencyType);

            if (account == null)
                return NotFound("Account not found.");
            if (dto.Balance <= 0)
                return BadRequest("Amount must be greater than zero.");

            account.Balance += dto.Balance;
            var transaction = new Transaction
            {
                AccountId = account.Id,
                TransactionType = TransactionType.Deposit,
                Amount = dto.Balance,
                Description = $"Deposited {dto.Balance} {dto.CurrencyType}",
                CreatedAt = DateTime.UtcNow
            };
            context.Transactions.Add(transaction);

            await context.SaveChangesAsync();
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
            var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId && dto.CurrencyType == x.CurrencyType);

            if (account == null)
                return NotFound("Account not found.");
            if (account.Balance < dto.Balance)
                return BadRequest("Insufficient funds.");
            if (dto.Balance <= 0)
                return BadRequest("Amount must be greater than zero.");

            account.Balance -= dto.Balance;
            var transaction = new Transaction
            {
                AccountId = account.Id,
                TransactionType = TransactionType.Withdraw,
                Amount = dto.Balance,
                Description = $"Withdraw {dto.Balance} {dto.CurrencyType}",
                CreatedAt = DateTime.UtcNow
            };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
            
            return Ok(account);
        }

    }
}
