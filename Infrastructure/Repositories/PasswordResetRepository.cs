using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly AppDbContext _context;

        public PasswordResetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .Include(p => p.User)
                .FirstOrDefaultAsync(p =>
                    p.Token == token &&
                    p.ExpiresAt > DateTime.UtcNow &&
                    !p.IsUsed);
        }

        public async Task MarkAsUsedAsync(PasswordResetToken token)
        {
            token.IsUsed = true;
            await _context.SaveChangesAsync();
        }
    }
}
