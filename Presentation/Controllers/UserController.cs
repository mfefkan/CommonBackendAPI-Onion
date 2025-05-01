using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository; 

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository; 
        }
         
        [HttpGet("test")]
        public IActionResult Test() {
            Console.WriteLine("Test endpoint hitttttttttttt");
            return Ok("AuthController is working!");
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Manuel mapping
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password), 
                FullName = dto.FullName,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                UserProfile = new UserProfile
                {
                    Address = dto.UserProfile.Address,
                    PhoneNumber = dto.UserProfile.PhoneNumber,
                    DateOfBirth = dto.UserProfile.DateOfBirth
                }
            };

            await _userRepository.AddAsync(user);

            return Ok("User added successfully!");
        }
         
        private string HashPassword(string password)
        { 
            return password; //  BCrypt.Net.BCrypt.HashPassword(password);
        }

         
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found!");

            return Ok(user);
        }




    }
}
