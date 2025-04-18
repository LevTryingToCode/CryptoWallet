using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoWallet.Entities;
using CryptoWallet.Services;
using static CryptoWallet.Dtos.WalletDTO;

namespace CryptoWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // GET: /api/wallet/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<WalletListDTO>> GetWallet(int userId)
        {
            var wallet = await _walletService.GetWalletByIdAsync(userId);
            if (wallet == null) return NotFound();
            return Ok(wallet);
        }

        // GET: /api/wallet
        [HttpGet]
        public async Task<ActionResult<List<WalletListDTO>>> GetAllWallets()
        {
            var wallets = await _walletService.GetAllWalletsAsync();
            return Ok(wallets);
        }

        // PUT: /api/wallet/{walletId}
        [HttpPut("{walletId}")]
        public async Task<IActionResult> UpdateWallet(int walletId, [FromBody] WalletUpdateDTO dto)
        {
            var result = await _walletService.UpdateWalletAsync(walletId, dto.Balance);
            if (!result) return NotFound();
            return NoContent();
        }

        // POST: /api/wallet
        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromBody] WalletCreateDTO dto)
        {
            var result = await _walletService.CreateWalletAsync(dto.UserId, dto.Balance);
            if (!result) return BadRequest("Wallet already exists for this user.");
            return CreatedAtAction(nameof(GetWallet), new { userId = dto.UserId }, dto);
        }

        // DELETE: /api/wallet/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteWallet(int userId)
        {
            var result = await _walletService.DeleteWalletByUserIdAsync(userId);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
