using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Wallet.Entities
{
    public class User
    {
        public Guid Id { get; set; }  // PK
        [MaxLength(255)]
        public string Username { get; set; } = null!;
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
      
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
