using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpCodeRepository _otpRepository;
        private readonly IMailService _mailService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public OtpService(IOtpCodeRepository otpRepository, IMailService mailService, IUserRepository userRepository, IConfiguration configuration)
        {
            _otpRepository = otpRepository;
            _mailService = mailService;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task SendOtpAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new Exception("User not found.");

            var otp = new OtpCode
            {
                UserId = user.Id,
                Code = GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _otpRepository.AddAsync(otp);

            await _mailService.SendEmailAsync(user.Email, "OTP Kodu", $"Giriş için kodunuz: {otp.Code}");
        }

        public async Task<bool> VerifyOtpAsync(int userId, string code)
        {
            var otp = await _otpRepository.GetLatestValidOtpAsync(userId, code);

            if (otp == null) return false;

            otp.IsUsed = true;
            await _otpRepository.MarkAsUsedAsync(otp);

            return true;
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
