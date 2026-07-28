namespace PMS.Application.Settings;

public sealed class ApplicationParameters
{
    public string Domain { get; set; } = null!;
    public EmailSettings EmailSettings { get; set; } = null!;
    public CookieSettings CookieSettings { get; set; } = null!;
}
