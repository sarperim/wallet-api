using Wallet.Entities.DTO;
using Wallet.Entities;

namespace Wallet.Services
{
    public interface ICurrencyService
    {
        Task<CurrencyRateDTO> CurrencyRateAsync(CurrencyToConvertDTO request);
        Task<decimal?> ConvertCurrencyAsync(CurrencyConversionDTO request);
        Task<Account?> BuyUsdAsync(USDamountDTO request, Guid userId);
    }
}
