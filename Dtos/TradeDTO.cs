using System.ComponentModel.DataAnnotations;
using static CryptoWallet.Dtos.CurrencyItemDTO;

namespace CryptoWallet.Dtos
{
    public class TradeDTO
    {
        public class BuyDTO
        {
            [Required]
            [Range(1, int.MaxValue)]
            public int UserId { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int CurrencyId { get; set; }

            [Required]
            [Range(0.01, double.MaxValue)]
            public double AmountToBuy { get; set; }
        }
        public class SellDTO
        {
            public int UserId { get; set; }
            public int CurrencyId { get; set; }
            public double AmountToSell { get; set; }
        }
        public class PortfolioDTO
        {
            public double Balance { get; set; }
            public double TotalPortfolioValue { get; set; }
            public List<CurrencyItemListDTO>? CurrencyItemList { get; set; }
        }
    }
}
