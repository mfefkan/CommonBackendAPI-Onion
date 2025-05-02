using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IEmailVerificationRepository
    {
        Task AddAsync(EmailVerificationToken token);
        Task<EmailVerificationToken?> GetByTokenAsync(string token);
        Task MarkAsUsedAsync(EmailVerificationToken token);
    }

}
