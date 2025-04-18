namespace CryptoWallet.Entities
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public int UserId { get; set; }
        public User user { get; set; }
        public List<CurrencyItem>? currencyItems { get; set; }
    }
}
