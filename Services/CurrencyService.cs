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
        public CurrencyService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateCurrencyAsync(CurrencyCreateDTO dto)
        {
            if (await _context.currencies.AnyAsync(c => c.Name == dto.CurrencyName)) { return false; }

            var currency = new Currency
            {
                Name = dto.CurrencyName,
                Value = dto.CurrencyValue
            };
            _context.currencies.Add(currency);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCurrencyAsync(int id)
        {
            var currency = await _context.currencies.FindAsync(id);
            if (currency == null) return false; 
            _context.currencies.Remove(currency);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CurrencyListDTO>> GetAllCurrenciesAsync()
        {
            return await _context.currencies.Select(c => new CurrencyListDTO
            {
                CurrencyId = c.CurrencyId,
                CurrencyName = c.Name,
                CurrencyValue = c.Value
            }).ToListAsync();
        }

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

        public async Task<bool> UpdateCurrencyManuallyAsync(int id, double newprice)
        {
            var currency = await _context.currencies.FindAsync(id);
            if (currency == null) return false;
            
            currency.Value = newprice; 
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
