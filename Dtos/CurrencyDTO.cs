namespace CryptoWallet.Dtos
{
    public class CurrencyDTO
    {
        public class CurrencyCreateDTO
        {
            public string CurrencyName { get; set; }
            public double CurrencyValue { get; set; }
        }
        public class CurrencyUpdateDTO
        {
            public int CurrencyId { get; set; }
            public double CurrencyValue { get; set; }
        }
        public class CurrencyListDTO
        {
            public int CurrencyId { get; set; }
            public string CurrencyName { get; set; }
            public double CurrencyValue { get; set; }
        }
    }
}
