namespace Core.Authentication.API.Settings;

public class DatabaseSettings
{
    //TODO: Add Database Config to Parameter Store
    public const string KeyName = "Database";

    public string TableName { get; set; } = default!;
}