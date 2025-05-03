using Application.DTOs;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;
        private readonly IEmailVerificationRepository _emailVerificationRepository;

        public AuthController(IUserRepository userRepository, AuthService authService, IEmailVerificationRepository emailVerificationRepository)
        {
            _userRepository = userRepository;
            _authService = authService;
            _emailVerificationRepository = emailVerificationRepository; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddUserDto dto)
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
                LoginResponseDto loginResponse = await _authService.LoginAsync(dto);
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unexpected situation.", detail = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
            return Ok(new { message = "Logout success, token canceled." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                await _authService.ForgotPasswordAsync(dto.Email);
                return Ok(new { message = "Şifre sıfırlama bağlantısı gönderildi (varsa)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
                return Ok(new { message = "Şifreniz başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var emailVerification = await _emailVerificationRepository.GetByTokenAsync(token);
            if (emailVerification == null)
            {
                return BadRequest("Geçersiz veya süresi dolmuş doğrulama bağlantısı.");
            }
             
            var user = emailVerification.User!;
            user.IsVerified = true;

            await _userRepository.UpdateAsync(user);
             
            emailVerification.IsUsed = true;
            await _emailVerificationRepository.MarkAsUsedAsync(emailVerification);

            return Ok("E-posta adresiniz başarıyla doğrulandı.");
        }
         

    }
}
