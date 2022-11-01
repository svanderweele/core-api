namespace Core.Gaming.API.Settings;

public class DatabaseSettings
{
    //TODO: Add Database Config to Parameter Store
    public const string KeyName = "Database";

    public int Limit { get; set; } = 15;
    public string GameTableName { get; set; } = default!;
    public string GameCollectionTableName { get; set; } = default!;
    public string GameCategoryTableName { get; set; } = default!;
}