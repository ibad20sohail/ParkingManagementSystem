using PMS.Application.Models.Requests.User;
using PMS.Application.Models.Responses.Common;
using PMS.Application.Models.Responses.User;

namespace PMS.Application.IServices
{
    public interface IAuthService
    {
        Task<AppResponse<LoginUserResponse>> LoginAsync(LoginUserRequest request);
        Task<AppResponse<OperationResponse>> ForgetPasswordAsync(GetUserByUsernameRequest request);
        Task<AppResponse<OperationResponse>> ResetPasswordAsync(ResetUserPasswordRequest request);
        Task<AppResponse<OperationResponse>> LogoutAsync(LogoutUserRequest request);
    }
}
