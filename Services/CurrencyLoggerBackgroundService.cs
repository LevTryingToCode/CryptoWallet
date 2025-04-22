
using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;
using static CryptoWallet.Dtos.CurrencyDTO;

namespace CryptoWallet.Services
{
    public class CurrencyLoggerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
        private readonly Dictionary<int, double> _previousValues = new();
        public bool IsRunning { get; set; } = false;

        public CurrencyLoggerBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (IsRunning)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var currencies = await context.currencies.ToListAsync(cancellationToken);

                    foreach (var currency in currencies)
                    {
                        double currentValue = currency.Value;

                        if (_previousValues.TryGetValue(currency.CurrencyId, out double previousValue))
                        {
                            if (currentValue != previousValue)
                            {
                                var log = new CurrencyMarketLogDTO
                                {
                                    CurrencyId = currency.CurrencyId,
                                    CurrencyName = currency.Name,
                                    OldValue = previousValue,
                                    NewValue = currentValue,
                                    Change = currentValue - previousValue
                                };

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"[LOG] {log.CurrencyName}: {log.OldValue} -> {log.NewValue} CHANGE: ({log.Change})");
                            }
                        }
                        _previousValues[currency.CurrencyId] = currentValue;
                    }
                }
                await Task.Delay(_interval, cancellationToken);
            }
        }
    }

}

