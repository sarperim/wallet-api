using Wallet.Entities;
using Wallet.Repositories;

namespace Wallet.Data
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Transaction> Transactions { get; }
        IGenericRepository<Account> Accounts { get; }
        Task SaveChangesAsync();
    }
}
