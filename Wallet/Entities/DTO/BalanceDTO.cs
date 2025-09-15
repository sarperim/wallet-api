namespace Wallet.Entities.DTO
{
    public class BalanceDTO
    {
        public string CurrencyType { get; set; } = null!;
        public decimal Balance { get; set; } = 0;
    }
}
