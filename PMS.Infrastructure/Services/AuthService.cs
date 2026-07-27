using Microsoft.Data.SqlClient;
using PMS.Application.IRepositories.User;
using PMS.Application.IServices;
using PMS.Application.Models.Requests.User;
using PMS.Application.Models.Responses.Common;
using PMS.Application.Models.Responses.User;

namespace PMS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICookieAuthenticationService _cookieAuthenticationService;

        public AuthService(IUserRepository userRepository, ICookieAuthenticationService cookieAuthenticationService)
        {
            _userRepository = userRepository;
            _cookieAuthenticationService = cookieAuthenticationService;
        }
        public async Task<AppResponse<LoginUserResponse>> LoginAsync(LoginUserRequest request)
        {
            try
            {
                var result = await _userRepository.LoginUserAsync(request);
                return AppResponse<LoginUserResponse>.Success(result);
            }
            catch (SqlException ex)
            {
                return AppResponse<LoginUserResponse>.Failure(ex.Message);
            }
        }
    }
}
