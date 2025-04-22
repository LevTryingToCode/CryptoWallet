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

        [HttpPost("start")]
        public IActionResult StartLogging()
        {
            if (_loggerService.IsRunning) { return BadRequest("Logging already running!"); }
            _loggerService.IsRunning = true;
            return Ok("Started logging all currencies market change!");
        }

        [HttpPost("stop")]
        public IActionResult StopLogging()
        {
            if (!_loggerService.IsRunning) { return BadRequest("Logging already stopped!"); }
            _loggerService.IsRunning = false;
            return Ok("Stopped logging all currencies market change!");
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                _loggerService.IsRunning
            });
        }
    }
}
