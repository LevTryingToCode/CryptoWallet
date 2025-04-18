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
            var success = await _tradeService.BuyCryptoAsync(dto.UserId, dto.CurrencyId, dto.AmountToBuy);
            if (!success)
                return BadRequest("Buy failed! Not enough currency or error during cryptoCurrency load.");
            return Ok("Vásárlás sikeres.");
        }

        [HttpPost("sell")]
        public async Task<IActionResult> Sell([FromBody] SellDTO dto)
        {
            var success = await _tradeService.SellCryptoAsync(dto.UserId, dto.CurrencyId, dto.AmountToSell);
            if (!success)
                return BadRequest("Sell Failed!");
            return Ok("Sell successful!");
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