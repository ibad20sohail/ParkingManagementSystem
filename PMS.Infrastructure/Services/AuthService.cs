using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PMS.Application.Constants;
using PMS.Application.IRepositories.ResetPasswordLink;
using PMS.Application.IRepositories.User;
using PMS.Application.IServices;
using PMS.Application.Models.Requests.ResetPasswordLink;
using PMS.Application.Models.Requests.User;
using PMS.Application.Models.Responses.Common;
using PMS.Application.Models.Responses.User;
using PMS.Application.Settings;

namespace PMS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordLinkRepository _resetPasswordLinkRepository;
        private readonly IEmailService _emailService;
        private readonly ApplicationParameters _settings;

        public AuthService(IUserRepository userRepository, IResetPasswordLinkRepository resetPasswordLinkRepository, IEmailService emailService, IOptions<ApplicationParameters> settings)
        {
            _userRepository = userRepository;
            _resetPasswordLinkRepository = resetPasswordLinkRepository;
            _emailService = emailService;
            _settings = settings.Value;
        }

        public async Task<AppResponse<GetUserByUsernameResponse>> ForgetPasswordAsync(GetUserByUsernameRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(request);
                var link = await _resetPasswordLinkRepository.GenerateResetPasswordLinkAsync(
                    new GenerateResetPasswordLinkRequest { UserId = user.UserId });

                var emailBody = await _emailService.GetTemplateAsync(
                    Cons.ForgetPasswordEmailTemplateName,
                    new Dictionary<string, string>
                    {
                        ["user_name"] = user.UserName,
                        ["reset_link"] = $"{_settings.Domain}{link.GeneratedLink}",
                        ["expire_in"] = $"{link.ExpireIn} minutes"
                    });

                await _emailService.SendAsync(user.Email, "Reset Password", emailBody);

                return AppResponse<GetUserByUsernameResponse>.Success(user);
            }
            catch (SqlException ex)
            {
                return AppResponse<GetUserByUsernameResponse>.Failure(ex.Message);
            }
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

        public async Task<AppResponse<OperationResponse>> LogoutAsync(LogoutUserRequest request)
        {
            try
            {
                var result = await _userRepository.LogoutUserAsync(request);

                return AppResponse<OperationResponse>.Success(result);
            }
            catch (SqlException ex)
            {
                return AppResponse<OperationResponse>.Failure(ex.Message);
            }
        }

        public async Task<AppResponse<OperationResponse>> ResetPasswordAsync(ResetUserPasswordRequest request)
        {
            try
            {
                var result = await _userRepository.ResetUserPasswordAsync(request);

                return AppResponse<OperationResponse>.Success(result);
            }
            catch (SqlException ex)
            {
                return AppResponse<OperationResponse>.Failure(ex.Message);
            }
        }
    }
}
