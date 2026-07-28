using PMS.Application.Settings;
using System.Security.Claims;

namespace PMS.Application.Helper
{
    public static class ClaimsHelper
    {
        public static ClaimInfo GetUserInformation(this ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
            {
                return new ClaimInfo(); 
            }

            return new ClaimInfo
            {
                UserId = int.TryParse(user.FindFirst("user_id")?.Value, out int uid) ? uid : 0,
                RoleId = int.TryParse(user.FindFirst("role_id")?.Value, out int rid) ? rid : 0,

                UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                RoleName = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty
            };
        }
    }
}
