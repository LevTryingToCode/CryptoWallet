using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoWallet.Entities;
using CryptoWallet.Services;
using CryptoWallet.Dtos;
using static CryptoWallet.Dtos.CurrencyDTO;

namespace CryptoWallet.Controllers
{
    [Route("api/crypto")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }


        //GET: /api/crypto
        [HttpGet]
        public async Task<ActionResult<List<CurrencyListDTO>>> GetAllCurrencies()
        { 
            var currencies = await _currencyService.GetAllCurrenciesAsync();
            return Ok(currencies);
        }


        // GET: /api/crypto/{cryptoId}
        [HttpGet("{cryptoId}")]
        public async Task<ActionResult<CurrencyListDTO>> GetCurrency(int cryptoId)
        {
            var currency = await _currencyService.GetCurrencyByIdAsync(cryptoId);
            if (currency == null)
                return NotFound("Currency not found.");
            return Ok(currency);
        }


        // POST: /api/crypto
        [HttpPost]
        public async Task<IActionResult> CreateCurrency([FromBody] CurrencyCreateDTO dto)
        {
            var success = await _currencyService.CreateCurrencyAsync(dto);
            if (!success)
                return BadRequest("Currency with this name already exists.");
            return CreatedAtAction(nameof(GetCurrency), new { cryptoId = dto.CurrencyName }, dto);
        }


        // DELETE: /api/crypto/{cryptoId}
        [HttpDelete("{cryptoId}")]
        public async Task<IActionResult> DeleteCurrency(int cryptoId)
        {
            var success = await _currencyService.DeleteCurrencyAsync(cryptoId);
            if (!success)
                return NotFound("Currency not found.");
            return NoContent();
        }
        // PUT: /api/crypto/price
        [HttpPut("price")]
        public async Task<IActionResult> UpdateCurrencyPrice([FromBody] CurrencyUpdateDTO dto)
        {
            var success = await _currencyService.UpdateCurrencyManuallyAsync(dto.CurrencyId, dto.CurrencyValue);
            if (!success)
                return NotFound("Currency not found.");
            return NoContent();
        }
    }
}
