namespace PMS.Application.Settings;

public sealed class ApplicationParameters
{
    public string BaseUrl { get; set; } = null!;
    public CookieSettings CookieSettings { get; set; } = null!;
}
