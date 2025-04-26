
using CryptoWallet.Entities;
using Microsoft.EntityFrameworkCore;
using static CryptoWallet.Dtos.CurrencyDTO;

namespace CryptoWallet.Services
{
    public class CurrencyLoggerBackgroundService : BackgroundService
    {
        private readonly ICurrencyChangeNotifier _notifier;
        public bool IsRunning { get; set; } = false;

        public CurrencyLoggerBackgroundService(ICurrencyChangeNotifier notifier)
        {
            _notifier = notifier;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _notifier.OnCurrencyChanged += LogCurrencyChange;
            return base.StartAsync(cancellationToken);
        }

        private void LogCurrencyChange(Currency currency, double oldValue, double newValue)
        {
            if (!IsRunning) return;

            double change = newValue - oldValue;
            double percentChange = oldValue != 0 ? (change / oldValue) * 100 : 0;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                $"[LOG] {currency.Name}: {oldValue:F2} -> {newValue:F2} CHANGE - {change:F2}, - > {percentChange:F2}%)"
            );
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _notifier.OnCurrencyChanged -= LogCurrencyChange;
            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }


}

