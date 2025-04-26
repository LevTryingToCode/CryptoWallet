using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;
using static CryptoWallet.Dtos.WalletDTO;
using static CryptoWallet.Dtos.CurrencyItemDTO;
using static CryptoWallet.Dtos.CurrencyDTO;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CryptoWallet.Services
{
    public interface IWalletService
    {
        public Task<WalletListDTO?> GetWalletByIdAsync(int userid); //done
        public Task<List<WalletListDTO>?> GetAllWalletsAsync(); 
        public Task<bool> UpdateWalletAsync(int walletid, double balance);
        public Task<bool> CreateWalletAsync(int userId, double balance = 1000);
        public Task<bool> DeleteWalletByUserIdAsync(int userid);
    }
    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;
        public WalletService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<WalletListDTO?> GetWalletByIdAsync(int userid) { 
            var wallet = await _context.wallets.Include(w => w.currencyItems)
                                               .ThenInclude(c => c.currency)
                                               .FirstOrDefaultAsync(w => w.UserId == userid);
            if (wallet == null) {
                return null;
            }
            var currencyList = wallet.currencyItems.Select(c => new CurrencyItemListDTO
            {
                CurrencyId = c.CurrencyItemId,
                CurrencyName = c.currency.Name,
                CurrencyValue = c.BuyValue,
            }).ToList();

            double TotalValue = currencyList.Sum(c => c.CurrencyValue);

            return new WalletListDTO
            {
                WalletId = wallet.WalletId,
                UserId = wallet.UserId,
                Currencies = currencyList,
                TotalValue = TotalValue,
                Balance = wallet.Balance
            };
        }
        public async Task<List<WalletListDTO>?> GetAllWalletsAsync() 
        {
            return await _context.wallets.Include(w => w.currencyItems)
                                               .ThenInclude(c => c.currency)
                                               .Select(wallet => new WalletListDTO
                                               {
                                                   WalletId = wallet.WalletId,
                                                   UserId = wallet.UserId,
                                                   Currencies = wallet.currencyItems.Select(c => new CurrencyItemListDTO
                                                   {
                                                       CurrencyId = c.CurrencyItemId,
                                                       CurrencyName = c.currency.Name,
                                                       CurrencyValue = c.BuyValue,
                                                   }).ToList(),
                                                   TotalValue = wallet.currencyItems.Sum(c => c.BuyValue),
                                                   Balance = wallet.Balance
                                               }).ToListAsync();
        }

        public async Task<bool> UpdateWalletAsync(int walletid, double balance)
        {
            var wallet = await _context.wallets.FirstOrDefaultAsync(w => w.WalletId == walletid);
            if (wallet == null) {
                return false;
            }
            wallet.Balance = balance;
            await _context.SaveChangesAsync();
            return true;
        }
            

        public async Task<bool> CreateWalletAsync(int userid, double balance = 1000)
        {
            if (await _context.wallets.AnyAsync(w => w.UserId == userid)) { return false; }

            var wallet = new Wallet
            {
                UserId = userid,
                Balance = balance,
                currencyItems = new List<CurrencyItem>()
            };
            _context.wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWalletByUserIdAsync(int userid)
        {
            var wallet = await _context.wallets.FirstOrDefaultAsync(w => w.UserId == userid);
            if (wallet == null)
            {
                return false;
            }
            _context.wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
