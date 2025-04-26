using CryptoWallet.Dtos;
using CryptoWallet.Entities;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using static CryptoWallet.Dtos.UserDTO;
namespace CryptoWallet.Services
{
    public interface IUserService 
    { 
        Task<List<UserListDTO>> GetAllUsersAsync();
        Task<UserListDTO?> GetUserByIdAsync(int id);
        Task<UserListDTO> CreateUserAsync(UserRegisterDTO userRegisterDTO);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UpdateUserAsync(int id, UserRegisterDTO userRegisterDTO);
        Task<UserLoginResponseDTO?> LoginAsync(UserLoginDTO loginDto);
    }
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }
        // Create a new user
        public async Task<UserListDTO> CreateUserAsync(UserRegisterDTO userRegisterDTO)
        {
            // Check if the email already exists
            if (await _context.users.AnyAsync(u => u.Email == userRegisterDTO.Email))
            {
                throw new Exception("Email already exists");
            }
            //create new user
            var newUser = new User
            {
                Name = userRegisterDTO.Name,
                Email = userRegisterDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Password),
                wallet = new Wallet()
            };

            //add user to DB
            _context.users.Add(newUser);
            await _context.SaveChangesAsync();

            //create wallet for user
            newUser.wallet.UserId = newUser.Id;
            await _context.SaveChangesAsync();

            //return according to the DTO
            return new UserListDTO
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
                WalletId = newUser.wallet.WalletId
            };
        }

        // Delete a user
        public async Task<bool> DeleteUserAsync(int id)
        {
            //find user by id + null check
            var user = await _context.users.Include(u => u.wallet).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) {
                return false;
            }
            if (user.wallet != null)
            {
                _context.wallets.Remove(user.wallet);
            }
            //delete user
            _context.Remove(user); 
            await _context.SaveChangesAsync();
            return true;
        }

        // Get a user by ID

        public async Task<UserListDTO?> GetUserByIdAsync(int id)
        {
            //find user by id and Wallet
            var user = await _context.users
                .Include(u => u.wallet)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? null : new UserListDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                WalletId = user.wallet.WalletId
            };
        }

        // Get all users
        public async Task<List<UserListDTO>> GetAllUsersAsync()
        {
            return await _context.users
                .Include(u => u.wallet) 
                .Select(user => new UserListDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    WalletId = user.wallet.WalletId
                })
                .ToListAsync();
        }

        // Update a user
        public async Task<bool> UpdateUserAsync(int id, UserRegisterDTO userRegisterDTO)
        {
            //find user by id + null check
            var user = await _context.users
                .Include(u => u.wallet) 
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // "registers" a new user according to input
            user.Name = userRegisterDTO.Name;
            user.Email = userRegisterDTO.Email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Password);

            //create "new" user in DB 
            new UserListDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                WalletId = user.wallet.WalletId
            };
            await _context.SaveChangesAsync();
            return true;
        }

        // Login a user? -> password encryption(?)
        public async Task<UserLoginResponseDTO?> LoginAsync(UserLoginDTO loginDto)
        {
            // Check if the user exists
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
                return null;

            // Check if the password is correct
            bool valid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
            if (!valid)
                return null;

            return new UserLoginResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}
