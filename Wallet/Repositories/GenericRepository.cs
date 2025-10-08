using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Wallet.Data;

namespace Wallet.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    { 
        private readonly WalletDbContext _dbContext;
        protected readonly DbSet<T> _set;
        public GenericRepository(WalletDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = _dbContext.Set<T>();
        }

        public Task AddAsync(T entity)
        {
            return _set.AddAsync(entity).AsTask();
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _set.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _set.FindAsync(id);
        }

        public void Update(T entity)
        {
           _set.Update(entity); 
        }
        public IQueryable<T> Query()
        {
            return _set.AsQueryable();
        }
    }
}
