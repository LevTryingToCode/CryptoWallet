namespace CryptoWallet.Entities
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}
