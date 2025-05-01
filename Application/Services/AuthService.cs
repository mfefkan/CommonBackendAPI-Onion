using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration; // JWT ayarlarını buradan alacağız

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            return GenerateJwtToken(user);
        }

        public async Task RegisterAsync(AddUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists.");

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
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        private bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
