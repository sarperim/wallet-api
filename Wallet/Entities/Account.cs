using System.Text.Json.Serialization;
using System.Transactions;

namespace Wallet.Entities
{
    public class Account
    {
        public Guid Id { get; set; } // PK
        public Guid UserId { get; set; } // FK to User
        public string CurrencyType { get; set; } = null!;
        public decimal Balance { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
