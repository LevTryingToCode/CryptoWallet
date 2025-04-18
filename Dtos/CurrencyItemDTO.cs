namespace CryptoWallet.Dtos
{
    public class CurrencyItemDTO
    {
        public class CurrencyItemListDTO 
        { 
            public int CurrencyId { get; set; }
            public string CurrencyName { get; set; }
            public double CurrencyValue { get; set; }
        }
        public class CurrencyItemCreateDTO
        {
            public int CurrencyId { get; set; }
            public double BuyValue { get; set; }
        }
        public class CurrencyItemSellDTO
        { 
            public int CurrencyId { get; set; }
            public double SellValue { get; set; }
        }
    }
}
