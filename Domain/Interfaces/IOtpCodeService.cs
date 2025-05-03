using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IOtpService
    {
        Task SendOtpAsync(int userId);
        Task<bool> VerifyOtpAsync(int userId, string code);
    }
}
