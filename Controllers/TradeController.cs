using Microsoft.AspNetCore.Mvc;
using CryptoWallet.Services;
using static CryptoWallet.Dtos.TradeDTO;
using Microsoft.EntityFrameworkCore;

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
            return Ok("Buy successful!");
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

        [HttpGet("profit/{userId}")]
        public async Task<IActionResult> GetTotalProfit(int userId)
        {
            var totalProfit = await _tradeService.GetTotalProfitAsync(userId);
            if (totalProfit == null)
                return NotFound();

            return Ok(totalProfit);
        }
        [HttpGet("profit/details/{userId}")]
        public async Task<IActionResult> GetDetailedProfit(int userId)
        {
            var detailedProfit = await _tradeService.GetDetailedProfitAsync(userId);
            if (detailedProfit == null)
                return NotFound();

            return Ok(detailedProfit);
        }
        [HttpGet("transactions/{userId}")]
        public async Task<IActionResult> GetTransactions(int userId)
        {
            var transactions = await _tradeService.GetTransactionsAsync(userId);
            if (!transactions.Any())
                return NotFound("No transactions found for the user.");

            return Ok(transactions);
        }
        [HttpGet("transactions/details/{transactionId}")]
        public async Task<IActionResult> GetTransactionDetails(int transactionId)
        {
            var transaction = await _tradeService.GetTransactionDetailsAsync(transactionId);
            if (transaction == null)
                return NotFound("Transaction not found.");

            return Ok(transaction);
        }

    }
}