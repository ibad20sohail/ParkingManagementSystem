using PMS.Application.Models.Requests;
using PMS.Application.Models.Responses;
using PMS.Application.Models.Responses.Common;

namespace PMS.Application.IServices
{
    public interface IAuthService
    {
        Task<AppResponse<LoginUserResponse>> LoginAsync(LoginUserRequest request);
    }
}
