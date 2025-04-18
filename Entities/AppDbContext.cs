using Microsoft.EntityFrameworkCore;

namespace CryptoWallet.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Wallet> wallets { get; set; }
        public DbSet<Currency> currencies { get; set; }
        public DbSet<CurrencyItem> currencyItems { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(u => u.wallet)
                                       .WithOne(w => w.user)
                                       .HasForeignKey<Wallet>(w => w.UserId)
                                       .IsRequired()
                                       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
