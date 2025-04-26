using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoWallet.Entities
{
    public class CurrencyItem
    {
        public int CurrencyItemId { get; set; }
        public int WalletId { get; set; }
        public Currency currency { get; set; } = null!;
        public double BuyValue { get; set; }
        public double CryptoAmount { get; set; } = 0;
        public Wallet Wallet { get; set; } = null!;

    }
}
