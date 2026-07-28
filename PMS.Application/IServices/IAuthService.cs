using PMS.Application.Models.Requests.User;
using PMS.Application.Models.Responses.Common;
using PMS.Application.Models.Responses.User;

namespace PMS.Application.IServices
{
    public interface IAuthService
    {
        Task<AppResponse<LoginUserResponse>> LoginAsync(LoginUserRequest request);
        Task<AppResponse<GetUserByUsernameResponse>> ForgetPasswordAsync(GetUserByUsernameRequest request);
        Task<AppResponse<OperationResponse>> ResetPasswordAsync(ResetUserPasswordRequest request);
    }
}
