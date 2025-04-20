using CryptoWallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoWallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyLoggerController : ControllerBase
    {
        private readonly CurrencyLoggerBackgroundService _loggerService;

        public CurrencyLoggerController(CurrencyLoggerBackgroundService loggerService)
        {
            _loggerService = loggerService;
        }

        [HttpPost("start/{currencyId}")]
        public IActionResult StartLogging(int currencyId)
        {
            _loggerService.TrackedCurrencyId = currencyId;
            _loggerService.IsRunning = true;
            return Ok($"Started logging currency with ID {currencyId}.");
        }

        [HttpPost("stop")]
        public IActionResult StopLogging()
        {
            _loggerService.IsRunning = false;
            _loggerService.TrackedCurrencyId = null;
            return Ok("Stopped logging.");
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                _loggerService.IsRunning,
                _loggerService.TrackedCurrencyId
            });
        }
    }
}
