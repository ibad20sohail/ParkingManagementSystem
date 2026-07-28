namespace PMS.Application.Settings;

public sealed class CookieSettings
{
    public string Name { get; set; } = null!;
    public int Expiration { get; set; }
    public string LoginPath { get; set; } = null!;
    public string LogoutPath { get; set; } = null!;
    public string AccessDeniedPath { get; set; } = null!;
    public bool SlidingExpiration { get; set; }
}
