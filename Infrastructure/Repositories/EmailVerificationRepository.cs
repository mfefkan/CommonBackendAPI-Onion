using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database; 
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly AppDbContext _context;

        public EmailVerificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailVerificationToken token)
        {
            await _context.EmailVerificationTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
        {
            return await _context.EmailVerificationTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task MarkAsUsedAsync(EmailVerificationToken token)
        {
            token.IsUsed = true;
            _context.EmailVerificationTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
