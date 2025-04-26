using CryptoWallet.Entities;

namespace CryptoWallet.Services
{
    public interface ICurrencyChangeNotifier
    {
        event Action<Currency, double, double> OnCurrencyChanged;
        void NotifyChange(Currency currency, double oldValue, double newValue);
    }

    public class CurrencyChangeNotifier : ICurrencyChangeNotifier
    {
        public event Action<Currency, double, double>? OnCurrencyChanged;

        public void NotifyChange(Currency currency, double oldValue, double newValue)
        {
            OnCurrencyChanged?.Invoke(currency, oldValue, newValue);
        }
    }

}
