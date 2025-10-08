using Microsoft.Identity.Client;
using Wallet.Entities;
using Wallet.Repositories;

namespace Wallet.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WalletDbContext _dbContext;

        private IGenericRepository<User>? _users;
        private IGenericRepository<Transaction>? _transactions;
        private IGenericRepository<Account>? _accounts;

        public UnitOfWork(WalletDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Lazy-loaded properties bu kisim gpt tekrar bak
        public IGenericRepository<User> Users => _users ??= new GenericRepository<User>(_dbContext);
        public IGenericRepository<Transaction> Transactions => _transactions ??= new GenericRepository<Transaction>(_dbContext);
        public IGenericRepository<Account> Accounts => _accounts ??= new GenericRepository<Account>(_dbContext);

        public Task SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
} 