using Wallet.Entities;
using Wallet.Entities.DTO;

namespace Wallet.Services
{
    public interface IAuthService
    {
        public Task<User?> RegisterAsync(UserDTO request);
        public Task<string?> LoginAsync(UserDTO request);
    }
}
