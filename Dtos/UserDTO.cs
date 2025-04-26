using CryptoWallet.Entities;

namespace CryptoWallet.Dtos
{
    public class UserDTO
    {
        public class UserListDTO 
        { 
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int WalletId { get; set; }
        }
        public class UserRegisterDTO
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }    
        }
        public class UserLoginDTO
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UserLoginResponseDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
