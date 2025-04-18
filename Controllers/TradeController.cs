using Microsoft.AspNetCore.Mvc;
using CryptoWallet.Services;
using static CryptoWallet.Dtos.TradeDTO;

namespace CryptoWallet.Controllers
{
    [ApiController]
    [Route("api/trade")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Buy([FromBody] BuyDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tradeService.BuyCryptoAsync(dto.UserId, dto.CurrencyId, dto.AmountToBuy);
            if (!result) return BadRequest("Buy failed. Check balance or currency.");
            return Ok("Buy successful.");
        }

        [HttpPost("sell")]
        public async Task<IActionResult> Sell([FromBody] SellDTO dto)
        {
            var result = await _tradeService.SellCryptoAsync(dto.UserId, dto.CurrencyId, dto.AmountToSell);
            if (!result) return BadRequest("Sell failed. Not enough assets.");
            return Ok("Sell successful.");
        }

        [HttpGet("/api/portfolio/{userId}")]
        public async Task<IActionResult> GetPortfolio(int userId)
        {
            var result = await _tradeService.GetPortfolioAsync(userId);
            if (result == null) return NotFound("Portfolio not found.");
            return Ok(result);
        }
    }
}