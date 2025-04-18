using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using static CryptoWallet.Dtos.CurrencyItemDTO;
using static CryptoWallet.Dtos.TradeDTO;

namespace CryptoWallet.Services
{
    public interface ITradeService
    {
        Task<bool> BuyCryptoAsync(int userId, int currencyId, double amountToSpend);
        Task<bool> SellCryptoAsync(int userId, int currencyId, double amountToSell);
        Task<PortfolioDTO?> GetPortfolioAsync(int userId);
    }
    public class TradeService : ITradeService
    {
        private readonly AppDbContext _context;
        public TradeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BuyCryptoAsync(int userId, int currencyId, double amountToSpend)
        {
            var wallet = await _context.wallets.Include(w => w.currencyItems)
                                               .FirstOrDefaultAsync(w => w.UserId == userId);
            var currency = await _context.currencies.FindAsync(currencyId);

            if (wallet == null || currency == null || wallet.Balance < amountToSpend) { return false; }

            double cryptoAmount = amountToSpend / currency.Value;
            wallet.Balance -= amountToSpend;
            
            
            var alreadyInWallet = wallet.currencyItems.FirstOrDefault(ci => ci.CurrencyItemId == currencyId);
            if (alreadyInWallet != null)
            {
                alreadyInWallet.BuyValue += amountToSpend;
            }
            else
            {
                wallet.currencyItems.Add(new CurrencyItem
                {
                    CurrencyItemId = currencyId,
                    BuyValue = amountToSpend
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PortfolioDTO?> GetPortfolioAsync(int userId)
        {
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .ThenInclude(ci => ci.currency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return null;

            var cryptoList = wallet.currencyItems.Select(ci => new CurrencyItemListDTO
            {
                CurrencyId = ci.CurrencyItemId,
                CurrencyName = ci.currency.Name,
                CurrencyValue = ci.BuyValue
            }).ToList();

            double portfolioValue = wallet.currencyItems.Sum(ci => (ci.BuyValue / ci.currency.Value) * ci.currency.Value);

            return new PortfolioDTO
            {
                Balance = wallet.Balance,
                TotalPortfolioValue = portfolioValue + wallet.Balance,
                CurrencyItemList = cryptoList
            };
        }

        public async Task<bool> SellCryptoAsync(int userId, int currencyId, double amountToSell)
        {
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            var currency = await _context.currencies.FindAsync(currencyId);

            if (wallet == null || currency == null)
                return false;

            var item = wallet.currencyItems.FirstOrDefault(ci => ci.CurrencyItemId == currencyId);
            if (item == null || item.BuyValue < amountToSell)
                return false;

            double sellReturn = (amountToSell / currency.Value) * currency.Value; // = amountToSell
            wallet.Balance += sellReturn;
            item.BuyValue -= amountToSell;

            if (item.BuyValue <= 0)
            {
                wallet.currencyItems.Remove(item);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
