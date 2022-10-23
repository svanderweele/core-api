namespace Core.Authentication.API.Settings;

public class JwtSettings
{
    public const string KeyName = "Jwt";

    public string Secret { get; set; } = default!;
}