namespace PMS.Application.Settings
{
    public sealed class ClaimInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}
