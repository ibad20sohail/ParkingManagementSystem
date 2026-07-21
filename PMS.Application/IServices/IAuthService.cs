using PMS.Application.Models.Requests;

namespace PMS.Application.IServices
{
    public interface IAuthService
    {
        Task LoginAsync(LoginUserRequest request);
    }
}
