using PMS.Application.IRepositories.User;
using PMS.Application.IServices;
using PMS.Application.Models.Requests;

namespace PMS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task LoginAsync(LoginUserRequest request)
        {
            try
            {
                var result = await _userRepository.LoginUserAsync(request);
                throw new NotImplementedException();
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}
