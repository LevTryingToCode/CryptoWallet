using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWallet.Services
{
    public class CurrencyUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); 
        private readonly Random _random = new Random();
        public CurrencyUpdateBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                { 
                    var ct = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var currencies = await ct.currencies.ToListAsync(cancellationToken);

                    foreach (var currency in currencies)
                    {
                        currency.Value = _random.NextDouble() * (70.0 - 10.0) + 10.0;
                    }
                    await ct.SaveChangesAsync(cancellationToken);
                }
                await Task.Delay(_interval, cancellationToken);
            }
        }
    }
}
