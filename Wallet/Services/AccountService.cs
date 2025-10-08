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

namespace Wallet.Services
{
    public class AccountService(WalletDbContext context, IConfiguration configuration, IUnitOfWork uow) : IAccountService
    {
        public async Task<Account> AddBalanceAsync(BalanceDTO dto, Guid userId)
        {
            var account = await uow.Accounts.Query().FirstOrDefaultAsync(x => x.UserId == userId && dto.CurrencyType == x.CurrencyType);

            if (account == null)
                return null;
            if (dto.Balance <= 0)
                return null;

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

            await uow.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> CreateAccountAsync(AccountDTO dto, Guid userId)
        {
          
            var existingAccount = await uow.Accounts
                   .Query().FirstOrDefaultAsync(a => a.UserId == userId && a.CurrencyType == dto.CurrencyType);

            if (existingAccount != null)
                return null; 
            
            var account = new Account
            {
                UserId = userId,
                CurrencyType = dto.CurrencyType,
            };
            uow.Accounts.AddAsync(account);
            await uow.SaveChangesAsync();
            return account;
        }

        public async Task<Account> WithdrawBalanceAsync(BalanceDTO dto,Guid userId)
        {
            var account = await uow.Accounts.Query().FirstOrDefaultAsync(x => x.UserId == userId && dto.CurrencyType == x.CurrencyType);

            if (account == null)
                return null;
            if (account.Balance < dto.Balance)
                return null;
            if (dto.Balance <= 0)
                return null;

            account.Balance -= dto.Balance;
            var transaction = new Transaction
            {
                AccountId = account.Id,
                TransactionType = TransactionType.Withdraw,
                Amount = dto.Balance,
                Description = $"Withdraw {dto.Balance} {dto.CurrencyType}",
                CreatedAt = DateTime.UtcNow
            };
            uow.Transactions.AddAsync(transaction);
            await uow.SaveChangesAsync();
            return account;
        }
    }
}
