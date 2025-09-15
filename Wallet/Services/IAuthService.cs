using Microsoft.Identity.Client;
using Wallet.Entities;
using Wallet.Entities.DTO;

namespace Wallet.Services
{
    public interface IAuthService
    {
        public Task<User?> RegisterAsync(UserDTO request);
        public Task<TokenDTO?> LoginAsync(UserDTO request);
        public Task<TokenDTO?> RefreshTokensAsync(RefreshTokenDTO request);
    }
}
