using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;
        public UserController(IUserRepository userRepository,AuthService authService)
        {
            _userRepository = userRepository; 
            _authService = authService;
        }
         
        [HttpGet("test")]
        public IActionResult Test() {
            Console.WriteLine("Test endpoint hitttttttttttt");
            return Ok("AuthController is working!");
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto dto)
        {
            try
            {
                await _authService.RegisterAsync(dto); // DTO'yu direkt veriyoruz
                return Ok("User added successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Unauthorized token");

            var user = await _userRepository.GetByIdAsync(int.Parse(userId));

            if (user == null)
                return NotFound("User not found!");

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role
            });
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
