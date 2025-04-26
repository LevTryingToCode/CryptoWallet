using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;
using static CryptoWallet.Dtos.CurrencyDTO;

namespace CryptoWallet.Services
{
    public interface ICurrencyService
    {
        Task<List<CurrencyListDTO>> GetAllCurrenciesAsync();
        Task<CurrencyListDTO?> GetCurrencyByIdAsync(int id);
        Task<bool> CreateCurrencyAsync(CurrencyCreateDTO dto);
        Task<bool> DeleteCurrencyAsync(int id);
        Task<bool> UpdateCurrencyManuallyAsync(int id,double newprice);
    }
    public class CurrencyService : ICurrencyService
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyChangeNotifier _notifier;
        public CurrencyService(AppDbContext context, ICurrencyChangeNotifier notifier)
        {
            _context = context;
            _notifier = notifier;
        }

        // Create a new currency
        public async Task<bool> CreateCurrencyAsync(CurrencyCreateDTO dto)
        {
            // Check if the currency already exists
            if (await _context.currencies.AnyAsync(c => c.Name == dto.CurrencyName)) { return false; }

            // Create a new currency and add it to the database
            var currency = new Currency
            {
                Name = dto.CurrencyName,
                Value = dto.CurrencyValue
            };
            _context.currencies.Add(currency);
            await _context.SaveChangesAsync();
            return true;
        }
        // Delete an already existing currency
        public async Task<bool> DeleteCurrencyAsync(int id)
        {
            //Delete the currency by id
            var currency = await _context.currencies.FindAsync(id);
            if (currency == null) return false; 
            _context.currencies.Remove(currency);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all currencies
        public async Task<List<CurrencyListDTO>> GetAllCurrenciesAsync()
        {
            return await _context.currencies.Select(c => new CurrencyListDTO
            {
                CurrencyId = c.CurrencyId,
                CurrencyName = c.Name,
                CurrencyValue = c.Value
            }).ToListAsync();
        }

        // Get currency by id
        public async Task<CurrencyListDTO?> GetCurrencyByIdAsync(int id)
        {
            var currency = await _context.currencies.FindAsync(id);
            if (currency == null) return null;

            return new CurrencyListDTO
            {
                CurrencyId = currency.CurrencyId,
                CurrencyName = currency.Name,
                CurrencyValue = currency.Value
            };
        }

        // Update currency manually and notify logger as the change happens
        public async Task<bool> UpdateCurrencyManuallyAsync(int id, double newprice)
        {
            // Check if the currency exists
            var currency = await _context.currencies.FindAsync(id);
            if (currency == null) return false;

            // Check if the new price is different from the old price
            // If the new price is the same as the old price, do nothing
            double oldValue = currency.Value;
            if (oldValue != newprice)
            {
                currency.Value = newprice;
                await _context.SaveChangesAsync();
                //notify logger for the change 
                _notifier.NotifyChange(currency, oldValue, newprice);
            }

            return true;
        }

    }
}
