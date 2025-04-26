using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWallet.Services
{
    public class CurrencyUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);
        private readonly Random _random = new Random();
        private readonly ICurrencyChangeNotifier _notifier;

        public CurrencyUpdateBackgroundService(IServiceProvider serviceProvider, ICurrencyChangeNotifier notifier)
        {
            _serviceProvider = serviceProvider;
            _notifier = notifier;
        }

        // change currency value every 30 seconds and notify the logger as the change happens --> until it is stopped
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var ct = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var currencies = await ct.currencies.ToListAsync(cancellationToken);

                foreach (var currency in currencies)
                {
                    double oldValue = currency.Value;
                    double newValue = Math.Round(_random.NextDouble() * (70.0 - 10.0) + 10.0, 2);
                    currency.Value = newValue;
                    //notify logger of the change
                    _notifier.NotifyChange(currency, oldValue, newValue);
                }


                await ct.SaveChangesAsync(cancellationToken);
                await Task.Delay(_interval, cancellationToken);
            }
        }
    }

}
