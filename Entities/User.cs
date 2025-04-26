namespace CryptoWallet.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Wallet wallet { get; set; } = new Wallet();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
