namespace CryptoWallet.Entities
{
    public class CurrencyItem
    {
        public int CurrencyItemId { get; set; }
        public Currency currency { get; set; } = null!;
        public double BuyValue { get; set; }    
    }
}
