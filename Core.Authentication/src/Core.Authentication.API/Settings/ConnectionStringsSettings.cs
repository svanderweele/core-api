namespace Core.Authentication.API.Settings;

public class ConnectionStringsSettings
{
    public const string KeyName = "connectionStrings";

    public string UsersTableName { get; set; } = default!;
}