using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OtpCodeRepository : IOtpCodeRepository
    {
        private readonly AppDbContext _context;

        public OtpCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OtpCode code)
        {
            await _context.OtpCodes.AddAsync(code);
            await _context.SaveChangesAsync();
        }

        public async Task<OtpCode?> GetLatestValidOtpAsync(int userId, string code)
        {
            return await _context.OtpCodes
                .Where(x => x.UserId == userId && x.Code == code && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task MarkAsUsedAsync(OtpCode code)
        {
            code.IsUsed = true;
            _context.OtpCodes.Update(code);
            await _context.SaveChangesAsync();
        }
    }

}
