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
        public CurrencyService(IHttpClientFactory httpClientFactory, WalletDbContext context)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
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
            var usdAccount = await _context.Accounts
                  .FirstOrDefaultAsync(x => x.UserId == userId && x.CurrencyType == "US DOLLAR");

            if (usdAccount == null)
            {
                usdAccount = new Account
                {
                    UserId = userId,
                    CurrencyType = "US DOLLAR",
                    Balance = 0m
                };
                _context.Accounts.Add(usdAccount);
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
            _context.Transactions.Add(Transaction);
            await _context.SaveChangesAsync();

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
