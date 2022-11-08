using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Settings;
using Microsoft.Extensions.Options;

namespace Core.Gaming.API.Repositories;

public class GameCategoryRepository : IGameCategoryRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DatabaseSettings> _databaseSettings;

    private string TableName => _databaseSettings.Value.GameCategoryTableName;


    public GameCategoryRepository(IAmazonDynamoDB dynamoDb, IOptions<DatabaseSettings> databaseSettings)
    {
        _dynamoDb = dynamoDb;
        _databaseSettings = databaseSettings;
    }

    public async Task<bool> CreateAsync(GameCategory category, CancellationToken cancellationToken)
    {
        var customerJson = JsonSerializer.Serialize(category);
        var itemAsDocument = Document.FromJson(customerJson);
        var itemAttributes = itemAsDocument.ToAttributeMap();

        var updateRequest = new PutItemRequest()
        {
            TableName = TableName,
            Item = itemAttributes
        };

        var response = await _dynamoDb.PutItemAsync(updateRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<IEnumerable<GameCategory>> GetAllAsync(CancellationToken cancellationToken)
    {
        var scanRequest = new ScanRequest()
        {
            TableName = TableName,
            Limit = _databaseSettings.Value.Limit,
            ConsistentRead = true
        };

        var response = await _dynamoDb.ScanAsync(scanRequest, cancellationToken);

        var itemsAsDocument = response.Items.Select(e =>
        {
            var document = Document.FromAttributeMap(e);
            return JsonSerializer.Deserialize<GameCategory>(document.ToJson());
        }).OfType<GameCategory>();

        return itemsAsDocument;
    }

    public async Task<GameCategory?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var getRequest = new GetItemRequest()
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>()
            {
                { "id", new AttributeValue() { S = id.ToString() } }
            },
            //This will ensure latest up to date data at a cost of higher usage https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.ReadConsistency.html
            ConsistentRead = true
        };

        var response = await _dynamoDb.GetItemAsync(getRequest, cancellationToken);

        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<GameCategory>(itemAsDocument.ToJson());
    }
}