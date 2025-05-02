using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration; // JWT ayarlarını buradan alacağız
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IMailService _mailService;
        private readonly IEmailVerificationRepository _emailVerificationRepository; 

        public AuthService(
            IUserRepository userRepository,
            IEmailVerificationRepository emailVerificationRepository,
            IMailService mailService)
        {
            _userRepository = userRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _mailService = mailService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var refreshToken = GenerateRefreshToken(user.Id);
            await _userRepository.AddRefreshTokenAsync(refreshToken);
            var generatedAccesToken = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                AccessToken = generatedAccesToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task RegisterAsync(AddUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists.");

            var roleParsed = Enum.TryParse<UserRole>(dto.Role, out var role)
                ? role
                : throw new ArgumentException("Invalid user role.");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                FullName = dto.FullName,
                Role = roleParsed,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsVerified = false, 
                UserProfile = new UserProfile
                {
                    Address = dto.UserProfile.Address,
                    PhoneNumber = dto.UserProfile.PhoneNumber,
                    DateOfBirth = dto.UserProfile.DateOfBirth
                }
            };

            await _userRepository.AddAsync(user);

            var verificationToken = new EmailVerificationToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _emailVerificationRepository.AddAsync(verificationToken);

            var verificationLink = $"https://localhost:5001/api/auth/verify-email?token={verificationToken.Token}";
            await _mailService.SendEmailAsync(
                user.Email,
                "E-posta Doğrulama",
                $"Merhaba {user.FullName},\n\nLütfen e-postanı doğrulamak için şu linke tıkla:\n\n{verificationLink}\n\nBağlantı 24 saat içinde geçerlidir.");
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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

        private RefreshToken GenerateRefreshToken(int userId)
        {
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<RefreshResponseDto> RefreshTokenAsync(string refreshTokenStr)
        {
            var token = await _userRepository.GetRefreshTokenAsync(refreshTokenStr);

            if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Unauthorized access or expired refresh token.");
            }

            var user = await _userRepository.GetByIdAsync(token.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
             
            var accessToken = GenerateJwtToken(user); 

            return new RefreshResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = token.Token 
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await _userRepository.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return; 

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };

            await _passwordResetRepository.AddAsync(resetToken);

            var resetLink = $"https://frontend.com/reset-password?token={resetToken.Token}";

            var html = $@"
                            <p>Şifre sıfırlama talebinde bulundunuz.</p>
                            <p><a href='{resetLink}'>Şifrenizi sıfırlamak için buraya tıklayın</a></p>
                            <p>Bu bağlantı 15 dakika boyunca geçerlidir.</p>
            ";

            await _mailService.SendEmailAsync(user.Email, "Şifre Sıfırlama", html);
        }

        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            var resetToken = await _passwordResetRepository.GetValidTokenAsync(token);
            if (resetToken == null)
                throw new UnauthorizedAccessException("Unauthorized access or expired reset token.");

            var user = resetToken.User;
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user);
            await _passwordResetRepository.MarkAsUsedAsync(resetToken);
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        private bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
