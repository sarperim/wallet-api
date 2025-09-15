using System.Security.Principal;

namespace Wallet.Entities
{
    public class User
    {
        public Guid Id { get; set; }  // PK
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
      
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
