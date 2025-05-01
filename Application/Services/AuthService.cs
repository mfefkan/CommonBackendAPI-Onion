using Domain.Entities;
using Domain.Interfaces; 

namespace Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userWriteRepository;

        public AuthService(IUserRepository userWriteRepository)
        {
            _userWriteRepository = userWriteRepository;
        }

        public async Task<string> LoginAsync(int id, string password)
        {
            var user = await _userWriteRepository.GetByIdAsync(id);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid id or password.");
            }

            // Token oluşturma işlemleri burada yapılır
            return "fake-token";
        }
    }
}
