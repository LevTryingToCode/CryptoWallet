
using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;
using static CryptoWallet.Dtos.CurrencyDTO;

namespace CryptoWallet.Services
{
    public class CurrencyLoggerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);
        public bool IsRunning { get;  set; } = false;
        public int? TrackedCurrencyId { get; set; } = null;

        public CurrencyLoggerBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            double? previousValue = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (IsRunning && TrackedCurrencyId.HasValue)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var currency = await context.currencies
                        .FirstOrDefaultAsync(c => c.CurrencyId == TrackedCurrencyId.Value, cancellationToken);

                    if (currency != null)
                    {
                        double currentValue = currency.Value;

                        if (previousValue.HasValue && currentValue != previousValue.Value)
                        {
                            var log = new CurrencyMarketLogDTO
                            {
                                CurrencyId = currency.CurrencyId,
                                CurrencyName = currency.Name,
                                OldValue = previousValue.Value,
                                NewValue = currentValue,
                                Change = currentValue - previousValue.Value
                            };

                            Console.WriteLine($"[LOG] {log.CurrencyName}: {log.OldValue} -> {log.NewValue} CHANGE : ({log.Change})");
                        }

                        previousValue = currentValue;
                    }
                }

                await Task.Delay(_interval, cancellationToken);
            }
        }
    }
}

