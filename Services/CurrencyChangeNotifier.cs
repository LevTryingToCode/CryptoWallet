using CryptoWallet.Entities;

namespace CryptoWallet.Services
{
    //Currency value change interface
    public interface ICurrencyChangeNotifier
    {
        //Event that is triggered when the currency value changes -> currency object + oldvalue + newvalue
        event Action<Currency, double, double> OnCurrencyChanged;

        //Method to notify the change
        void NotifyChange(Currency currency, double oldValue, double newValue);
    }

    //Class responsible to notify the classes which are subscribed to the event
    public class CurrencyChangeNotifier : ICurrencyChangeNotifier
    {
        //event for the Currency value change 
        public event Action<Currency, double, double>? OnCurrencyChanged;

        //This method is called when the OnCurrencyChanged event is triggered
        //It takes a currency object, the old value and the new value as parameters
        public void NotifyChange(Currency currency, double oldValue, double newValue)
        {
            //If a class subscribes to the event, it will be triggered
            OnCurrencyChanged?.Invoke(currency, oldValue, newValue);
        }
    }

}
