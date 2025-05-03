using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IOtpCodeRepository
    {
        Task<OtpCode?> GetLatestValidOtpAsync(int userId, string code);
        Task AddAsync(OtpCode otpCode); 
        Task MarkAsUsedAsync(OtpCode code);
    }
}
