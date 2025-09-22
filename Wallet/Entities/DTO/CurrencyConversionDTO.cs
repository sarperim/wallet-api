namespace Wallet.Entities.DTO
{
    public class CurrencyConversionDTO
    {
        public string Currency { get; set; }
        public string CurrencyTo { get; set; }
        public decimal Amount{ get; set; }
    }
}
