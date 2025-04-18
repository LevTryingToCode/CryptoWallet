    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using CryptoWallet.Entities;
    using CryptoWallet.Services;
    using CryptoWallet.Dtos;
    using static CryptoWallet.Dtos.UserDTO;

    namespace CryptoWallet.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            // GET: api/Users
            [HttpGet]
            public async Task<ActionResult<IEnumerable<UserListDTO>>> GetAll()
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }

            // GET: api/Users/5
            [HttpGet("{id}")]
            public async Task<ActionResult<UserListDTO>> GetById(int id)
            {
                var user = await _userService.GetUserByIdAsync(id);

                return user == null ? NotFound() : Ok(user);
            }
            // POST: api/Users/register
            [HttpPost("register")]
            public async Task<ActionResult<UserListDTO>> Register(UserRegisterDTO userRegisterDTO)
            {
                var createdUser = await _userService.CreateUserAsync(userRegisterDTO);
                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
            }

        [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, UserRegisterDTO dto)
            {
                var result = await _userService.UpdateUserAsync(id, dto);
                return result ? NoContent() : NotFound();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                var result = await _userService.DeleteUserAsync(id);
                return result ? NoContent() : NotFound();
            }
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDTO>> Login(UserLoginDTO loginDto)
        {
            var user = await _userService.LoginAsync(loginDto);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(user);
        }
    }
    }
