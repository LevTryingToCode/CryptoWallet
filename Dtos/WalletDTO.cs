using static CryptoWallet.Dtos.CurrencyItemDTO;

namespace CryptoWallet.Dtos
{
    public class WalletDTO
    {
        public class WalletListDTO
        {
            public int WalletId { get; set; }
            public int UserId { get; set; }
            public List<CurrencyItemListDTO>? Currencies { get; set; }
            public double TotalValue { get; set; } = 0;
            public double Balance { get; set; } = 0;
        }
        public class WalletCreateDTO
        {
            public int UserId { get; set; }
            public double Balance { get; set; } = 1000;
        }
        public class WalletUpdateDTO
        {
            public int UserId { get; set; }
            public double Balance { get; set; }
        }
    }
}
