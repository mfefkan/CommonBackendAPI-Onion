using Domain.Entities;
using Domain.Interfaces; 

namespace Application.Services
{
    public class AuthService
    {
        private readonly IUserReadRepository _userReadRepository;

        public AuthService(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        public async Task<string> LoginAsync(int id, string password)
        {
            var user = await _userReadRepository.GetByIdAsync(id);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid id or password.");
            }

            // Token oluşturma işlemleri burada yapılır
            return "fake-token";
        }
    }
}
