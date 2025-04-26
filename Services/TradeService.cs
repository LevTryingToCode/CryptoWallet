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

        // Buy crypto -> update wallet balance and add transaction
        public async Task<bool> BuyCryptoAsync(int userId, int currencyId, double amountToSpend)
        {
            //wallet by id and/or userid 
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            //currency by id
            var currency = await _context.currencies.FindAsync(currencyId);

            //if any of it is empty or null -> return false
            if (wallet == null || currency == null || wallet.Balance < amountToSpend)
                return false;

            // amount of crypto bought for money divided by the currency's current value in the set moment
            double cryptoAmount = amountToSpend / currency.Value;
            wallet.Balance -= amountToSpend;

            // Check if the currency item already exists in the wallet then increment it, if it is the same currency
            var alreadyInWallet = wallet.currencyItems
                .FirstOrDefault(ci => ci.CurrencyItemId == currencyId);

            // If it exists, update purchase details -> the sum of money spent and the amount of crypto
            if (alreadyInWallet != null)
            {
                alreadyInWallet.BuyValue += amountToSpend;
                alreadyInWallet.CryptoAmount += cryptoAmount;
            }
            else
            {
                // If it doesn't exist, create a new currency item and add it to the wallet
                wallet.currencyItems.Add(new CurrencyItem
                {
                    CurrencyItemId = currencyId,
                    BuyValue = amountToSpend,
                    CryptoAmount = cryptoAmount
                });
            }
            //add it to the transaction table in the DB -> rate = currency price at the set moment in time
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

        //return portfolio -> listing wallets content if it exists, and show portfolios current value 
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

        //sell crypto -> update wallet balance and add transaction
        public async Task<bool> SellCryptoAsync(int userId, int currencyId, double amountToSell)
        {
            //wallet by id and/or userid
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            //currency by id
            var currency = await _context.currencies.FindAsync(currencyId);

            //if any of it is empty or null -> return false
            if (wallet == null || currency == null)
                return false;

            // Check if the user tries to sell more than he/she already has, or if the currencyitem is not in the wallet or NULL
            var item = wallet.currencyItems.FirstOrDefault(ci => ci.CurrencyItemId == currencyId);
            if (item == null || item.CryptoAmount < amountToSell)
                return false;

            // Calculate the selling price and profit
            double sellReturn = amountToSell * currency.Value;

            // Calculate the original cost of the amount being sold
            double buyPricePerUnit = item.BuyValue / item.CryptoAmount;
            double originalCost = buyPricePerUnit * amountToSell;
            
            //Calculate raw profit
            double profit = sellReturn - originalCost;

            // Update the wallet balance and the currency item
            wallet.Balance += sellReturn;
            item.CryptoAmount -= amountToSell;
            item.BuyValue -= originalCost;

            //for rounding issues if the amount of crypto in users wallet too small remove from the wallet + currencyitems table
            if (item.CryptoAmount <= 0.000001)
            {
                wallet.currencyItems.Remove(item);
            }

            //add it to the transactions table in the DB
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

        //get the total profit of the user for the whole wallet 
        public async Task<string?> GetTotalProfitAsync(int userId)
        {
            //wallet by id and/or userid
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .ThenInclude(ci => ci.currency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            //if wallet empty or null -> return null
            if (wallet == null)
                return null;

            // Calculate the total profit by summing up the profit for each currency item (FOR THE CURRENCIES'S CURRENT PRICE UPON THE REQUEST)
            double totalProfit = wallet.currencyItems.Sum(ci =>
            {
                double currentValue = ci.CryptoAmount * ci.currency.Value;
                double buyValue = ci.BuyValue;
                return currentValue - buyValue;
            });

            return $"{totalProfit} is the current profit.";
        }

        //get the detailed profit of the user for the whole wallet
        public async Task<List<DetailedProfitDTO>?> GetDetailedProfitAsync(int userId)
        {
            //wallet by id and/or userid
            var wallet = await _context.wallets
                .Include(w => w.currencyItems)
                .ThenInclude(ci => ci.currency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            //if wallet empty or null -> return null
            if (wallet == null)
                return null;

            // Calculate the detailed profit for each currency item
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
        //get the transactions of the user
        public async Task<List<Transaction>> GetTransactionsAsync(int userId)
        {
            //return all transactions from the same userid in a descending order according to the transactions timestamp (without DTO)
            return await _context.transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionDetailsAsync(int transactionId)
        {
            //return the transaction with the given transactionId  (without DTO)
            return await _context.transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }
    }
}
