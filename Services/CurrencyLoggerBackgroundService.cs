
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

        //Start the logging process
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //event subscription
            _notifier.OnCurrencyChanged += LogCurrencyChange;
            return base.StartAsync(cancellationToken);
        }

        //Log the currency changes
        //THE LOGGING IS PRESENTED IN THE CONSOLE ATM
        private void LogCurrencyChange(Currency currency, double oldValue, double newValue)
        {
            // Check if the service is running
            if (!IsRunning) return;

            // Calculate the price diff between the old value and the new one + calculate the change in percentage
            double change = newValue - oldValue;
            double percentChange = oldValue != 0 ? (change / oldValue) * 100 : 0;

            // Log the change to the console
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                $"[LOG] {currency.Name}: {oldValue:F2} ==> {newValue:F2} CHANGE :: {change:F2}, --> {percentChange:F2}%)"
            );
        }

        //Stop the logging process
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //event sub
            _notifier.OnCurrencyChanged -= LogCurrencyChange;
            return base.StopAsync(cancellationToken);
        }

        //Needed because of the BackgroundService interface "does nothing"
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }


}

