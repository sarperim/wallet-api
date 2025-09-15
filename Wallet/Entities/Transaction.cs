using System.Text.Json.Serialization;

namespace Wallet.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; } // PK
        public Guid  AccountId { get; set; } // FK to Account
        public TransactionType TransactionType { get; set; } // Enum: Deposit, Withdraw, Exchange
        public decimal Amount { get; set; }

        // Only for Exchange transactions
        public string? TargetCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }

        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [JsonIgnore]
        public Account Account { get; set; } = null!;
    }

    public enum TransactionType
    {
        Deposit,
        Withdraw,
        Exchange
    }
}
