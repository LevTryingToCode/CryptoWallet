namespace CryptoWallet.Dtos
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public string TransactionType { get; set; } = null!;
        public double Amount { get; set; }
        public double Rate { get; set; }
        public DateTime Timestamp { get; set; }
        public string CurrencyName { get; set; } = null;
    }
}
