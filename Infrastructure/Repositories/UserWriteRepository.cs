using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserWriteRepository :IUserWriteRepository
    {
        private readonly AuthDBContext _context; 

        public UserWriteRepository(AuthDBContext context)
        {
            _context = context; 
        }



        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
