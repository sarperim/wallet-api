using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using Wallet.Data;
using Wallet.Entities;
using Wallet.Entities.DTO;

namespace Wallet.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly WalletDbContext _context;
        private readonly IUnitOfWork _uow;
        public CurrencyService(IHttpClientFactory httpClientFactory, WalletDbContext context, IUnitOfWork uow)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
            _uow = uow;
        }

        public async Task<Account?> BuyUsdAsync(USDamountDTO request, Guid userId)
        {
            var response = await _httpClient.GetAsync("https://hasanadiguzel.com.tr/api/kurgetir");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            var currencyData = JsonSerializer.Deserialize<CurrencyRateJsonDTO>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (currencyData.TCMB_AnlikKurBilgileri == null)
            {
                return null;
            }
            var usd = currencyData.TCMB_AnlikKurBilgileri
    .FirstOrDefault(c => c.CurrencyName.Equals("US DOLLAR", StringComparison.OrdinalIgnoreCase));

            var usdRate = usd.ForexBuying;

            var tlAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId && "TL" == x.CurrencyType);
            if(tlAccount is null)
            {
                return null;
            }
            var requiredTl = request.usdAmount * usdRate;
            if (tlAccount.Balance < requiredTl)
            {
                return null;
            }
            tlAccount.Balance -= requiredTl;
            var usdAccount = await _uow.Accounts.Query()
                  .FirstOrDefaultAsync(x => x.UserId == userId && x.CurrencyType == "US DOLLAR");

            if (usdAccount == null)
            {
                usdAccount = new Account
                {
                    UserId = userId,
                    CurrencyType = "US DOLLAR",
                    Balance = 0m
                };
                _uow.Accounts.AddAsync(usdAccount);
            }
            usdAccount.Balance += request.usdAmount;
            var Transaction = new Transaction
            {
                AccountId = tlAccount.Id,
                TransactionType = TransactionType.Exchange,
                Amount = requiredTl,
                TargetCurrency = "USD",
                ExchangeRate = usdRate,
                Description = $"Bought {request.usdAmount} USD",
                CreatedAt = DateTime.UtcNow
            };
            _uow.Transactions.AddAsync(Transaction);
            await _uow.SaveChangesAsync();

            return usdAccount;
        }

        public async Task<decimal?> ConvertCurrencyAsync(CurrencyConversionDTO request)
        {
            var response = await _httpClient.GetAsync("https://hasanadiguzel.com.tr/api/kurgetir");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            var currencyData = JsonSerializer.Deserialize<CurrencyRateJsonDTO>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (currencyData.TCMB_AnlikKurBilgileri == null)
            {
                return null;
            }
            var fromRate = currencyData.TCMB_AnlikKurBilgileri
                    .FirstOrDefault(c => c.CurrencyName.Equals(request.Currency, StringComparison.OrdinalIgnoreCase))?.ForexBuying;
            var toRate = currencyData.TCMB_AnlikKurBilgileri
                             .FirstOrDefault(c => c.CurrencyName.Equals(request.CurrencyTo, StringComparison.OrdinalIgnoreCase))?.ForexBuying;
            if (fromRate == null || toRate == null) return null;

            var amountInTL =request.Amount * fromRate.Value;
            var convertedAmount = amountInTL / toRate.Value;

            return convertedAmount;
        }

        public async Task<CurrencyRateDTO> CurrencyRateAsync(CurrencyToConvertDTO request)
        {
            var response = await _httpClient.GetAsync("https://hasanadiguzel.com.tr/api/kurgetir");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            var currencyData = JsonSerializer.Deserialize<CurrencyRateJsonDTO>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (currencyData.TCMB_AnlikKurBilgileri == null)
            {
                return null;
            }
            var currency = currencyData.TCMB_AnlikKurBilgileri
                                      .FirstOrDefault(c => c.CurrencyName.Equals(request.CurrencyToConvert, System.StringComparison.OrdinalIgnoreCase));
            return currency;
        }
    }
}
