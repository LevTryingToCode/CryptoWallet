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
        public Task<WalletListDTO?> GetWalletByIdAsync(int userid); 
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

        //wallet by id
        public async Task<WalletListDTO?> GetWalletByIdAsync(int userid) {
            //get wallet by user id 
            var wallet = await _context.wallets.Include(w => w.currencyItems)
                                               .ThenInclude(c => c.currency)
                                               .FirstOrDefaultAsync(w => w.UserId == userid);

            //check if the wallet exists
            if (wallet == null) {
                return null;
            }

            //get all currencies in the wallet
            var currencyList = wallet.currencyItems.Select(c => new CurrencyItemListDTO
            {
                CurrencyId = c.CurrencyItemId,
                CurrencyName = c.currency.Name,
                CurrencyValue = c.BuyValue,
            }).ToList();

            //calculate total value of the wallet
            double TotalValue = currencyList.Sum(c => c.CurrencyValue);

            //return according to the DTO
            return new WalletListDTO
            {
                WalletId = wallet.WalletId,
                UserId = wallet.UserId,
                Currencies = currencyList,
                TotalValue = TotalValue,
                Balance = wallet.Balance
            };
        }

        //all wallets
        public async Task<List<WalletListDTO>?> GetAllWalletsAsync() 
        {
            //get all wallets with their currencies
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

        //wallet balance update
        public async Task<bool> UpdateWalletAsync(int walletid, double balance)
        {
            //check if the wallet exists
            var wallet = await _context.wallets.FirstOrDefaultAsync(w => w.WalletId == walletid);
            if (wallet == null) {
                return false;
            }
            //set balance 
            wallet.Balance = balance;
            await _context.SaveChangesAsync();
            return true;
        }

        //create a wallet with 1000 balance, if not already created for the user
        public async Task<bool> CreateWalletAsync(int userid, double balance = 1000)
        {
            //check if the wallet already exists
            if (await _context.wallets.AnyAsync(w => w.UserId == userid)) { return false; }
            //create wallet
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

        //delete wallet by user id

        public async Task<bool> DeleteWalletByUserIdAsync(int userid)
        {
            //check if the wallet exists
            var wallet = await _context.wallets.FirstOrDefaultAsync(w => w.UserId == userid);
            if (wallet == null)
            {
                return false;
            }
            //delete wallet
            _context.wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
