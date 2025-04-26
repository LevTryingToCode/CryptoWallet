
namespace CryptoWallet.Dtos
{
    public class DetailedProfitDTO
    {
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double BuyValue { get; set; }
        public double CurrentValue { get; set; }
        public double Profit { get; set; }
    }
}
