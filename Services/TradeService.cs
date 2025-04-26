using CryptoWallet.Dtos;
using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;
using static CryptoWallet.Dtos.CurrencyItemDTO;
using static CryptoWallet.Dtos.TradeDTO;

namespace CryptoWallet.Services
{
    public interface ITradeService
    {
        Task<bool> BuyCryptoAsync(int userId, int currencyId, double amountToSpend);
        Task<bool> SellCryptoAsync(int userId, int currencyId, double amountToSell);
        Task<PortfolioDTO?> GetPortfolioAsync(int userId);
        Task<string?> GetTotalProfitAsync(int userId);
        Task<List<DetailedProfitDTO>> GetDetailedProfitAsync(int userid);
        Task<List<Transaction>> GetTransactionsAsync(int userId);
        Task<Transaction?> GetTransactionDetailsAsync(int transactionId);
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
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            var currency = await _context.currencies.FindAsync(currencyId);

            if (wallet == null || currency == null || wallet.Balance < amountToSpend)
                return false;

            double cryptoAmount = amountToSpend / currency.Value;
            wallet.Balance -= amountToSpend;

            var alreadyInWallet = wallet.currencyItems
                .FirstOrDefault(ci => ci.CurrencyItemId == currencyId);

            if (alreadyInWallet != null)
            {
                alreadyInWallet.BuyValue += amountToSpend;
                alreadyInWallet.CryptoAmount += cryptoAmount;
            }
            else
            {
                wallet.currencyItems.Add(new CurrencyItem
                {
                    CurrencyItemId = currencyId,
                    BuyValue = amountToSpend,
                    CryptoAmount = cryptoAmount
                });
            }

            _context.transactions.Add(new Transaction
            {
                UserId = userId,
                CurrencyId = currencyId,
                TransactionType = "Buy",
                Amount = cryptoAmount,
                Rate = currency.Value,
                Timestamp = DateTime.Now
            });

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
                CurrencyValue = ci.BuyValue,
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
            if (item == null || item.CryptoAmount < amountToSell)
                return false;

            double sellReturn = amountToSell * currency.Value;

            double buyPricePerUnit = item.BuyValue / item.CryptoAmount;
            double originalCost = buyPricePerUnit * amountToSell;

            double profit = sellReturn - originalCost;

            wallet.Balance += sellReturn;
            item.CryptoAmount -= amountToSell;
            item.BuyValue -= originalCost;

            if (item.CryptoAmount <= 0.000001)
            {
                wallet.currencyItems.Remove(item);
            }

            _context.transactions.Add(new Transaction
            {
                UserId = userId,
                CurrencyId = currencyId,
                TransactionType = "Sell",
                Amount = amountToSell,
                Rate = currency.Value,
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string?> GetTotalProfitAsync(int userId)
        {
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .ThenInclude(ci => ci.currency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return null;

            double totalProfit = wallet.currencyItems.Sum(ci =>
            {
                double currentValue = ci.CryptoAmount * ci.currency.Value;
                double buyValue = ci.BuyValue;
                return currentValue - buyValue;
            });

            return $"{totalProfit} is the current profit.";
        }

        public async Task<List<DetailedProfitDTO>?> GetDetailedProfitAsync(int userId)
        {
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .ThenInclude(ci => ci.currency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return null;

            var detailedProfits = wallet.currencyItems.Select(ci => new DetailedProfitDTO
            {
                CurrencyId = ci.CurrencyItemId,
                CurrencyName = ci.currency.Name,
                BuyValue = ci.BuyValue,
                CurrentValue = ci.CryptoAmount * ci.currency.Value,
                Profit = (ci.CryptoAmount * ci.currency.Value) - ci.BuyValue
            }).ToList();

            return detailedProfits;
        }
        public async Task<List<Transaction>> GetTransactionsAsync(int userId)
        {
            return await _context.transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionDetailsAsync(int transactionId)
        {
            return await _context.transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }
    }
}
