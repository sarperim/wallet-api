using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Wallet.Entities.DTO;
using Wallet.Services;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<UserDTO> _userValidator;
        private readonly HttpClient _httpClient;
        private readonly ICurrencyService _currencyService;
        public CurrencyController(IAuthService authService, IValidator<UserDTO> userValidator, IHttpClientFactory httpClientFactory, ICurrencyService currencyService)
        {
            _authService = authService;
            _userValidator = userValidator;
            _httpClient = httpClientFactory.CreateClient();
            _currencyService = currencyService;
        }

        [HttpPost("CurrencyRateTL")]
        public async Task<IActionResult> currencyRate(CurrencyToConvertDTO request)
        {
            var currency = await _currencyService.CurrencyRateAsync(request);

            if (currency == null)
                return NotFound($"Currency '{request.CurrencyToConvert}' not found.");

            return Ok(currency);
        }
        [HttpPost("CurrencyConversion")]
        public async Task<IActionResult> currencyConvert(CurrencyConversionDTO request)
        {
            var result = await _currencyService.ConvertCurrencyAsync(request);
            if (result == null) return NotFound("Currency not found or error occurred.");
            return Ok(new { ConvertedAmount = result });
        }
        [Authorize]
        [HttpPost("BuyUSD")]
        public async Task<IActionResult> buyUSD(USDamountDTO request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var account = await _currencyService.BuyUsdAsync(request, userId);
            if (account == null)
            {
                return BadRequest("Problem Occured.");
            }
            return Ok(account);
        }

    }
}
