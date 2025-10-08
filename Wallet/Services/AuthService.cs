using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Wallet.Data;
using Wallet.Entities;
using Wallet.Entities.DTO;

namespace Wallet.Services
{
    public class AuthService(WalletDbContext context,IConfiguration configuration,IUnitOfWork uow):IAuthService
    {

        public async Task<TokenDTO?> LoginAsync(UserDTO request)
        {
            var user = await uow.Users.Query().FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<TokenDTO> CreateTokenResponse(User? user)
        {
            return new TokenDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<User?> RegisterAsync(UserDTO request)
        {
            if(await uow.Users.Query().AnyAsync(u => u.Username == request.Username))
            {
                return null;
            }
            var user = new User
            {
                Username = request.Username
            };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            uow.Users.AddAsync(user);
            uow.SaveChangesAsync();
        
            return user ;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")));
            
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
               issuer: configuration.GetValue<string>("AppSettings:Issuer"),
               audience: configuration.GetValue<string>("AppSettings:Audience"),
               claims: claims,
               expires: DateTime.UtcNow.AddDays(1),
               signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<String> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await uow.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await uow.Users.GetByIdAsync(userId);
            if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }

        public async Task<TokenDTO?> RefreshTokensAsync(RefreshTokenDTO request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if(user is null)
            {
                return null;
            }
            return await CreateTokenResponse(user);
              
        }
    }
}
