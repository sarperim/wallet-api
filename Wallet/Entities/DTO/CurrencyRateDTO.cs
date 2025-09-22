namespace Wallet.Entities.DTO
{
    public class CurrencyRateDTO
    {
        public string Isim { get; set; }
        public string CurrencyName { get; set; }
        public decimal ForexBuying { get; set; }
        public object ForexSelling { get; set; }
        public object BanknoteBuying { get; set; }
        public object BanknoteSelling { get; set; }
        public object CrossRateUSD { get; set; }
        public object CrossRateOther { get; set; }
    }

}
