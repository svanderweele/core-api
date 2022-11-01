namespace Core.Authentication.API.Settings;

public class JwtSettings
{
    public const string KeyName = "jwt";

    public string Secret { get; set; } = default!;
 
    public string Issuer { get; set; } = default!;
}