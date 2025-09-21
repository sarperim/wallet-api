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
    public interface IAccountService
    {
        public Task<Account?> CreateAccountAsync(AccountDTO request, Guid userId);
        public Task<Account> AddBalanceAsync(BalanceDTO request, Guid userId);
        public Task<Account> WithdrawBalanceAsync(BalanceDTO request, Guid userId);
    }
}
