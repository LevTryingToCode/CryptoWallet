using System;

namespace CryptoWallet.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int CurrencyId { get; set; }
        public string TransactionType { get; set; } = null!; 
        public double Amount { get; set; }
        public double Rate { get; set; } 
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public User User { get; set; } = null!;
        public Currency Currency { get; set; } = null!;

    }
}
