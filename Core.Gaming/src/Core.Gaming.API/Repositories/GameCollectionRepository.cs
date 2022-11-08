using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Settings;
using Microsoft.Extensions.Options;

namespace Core.Gaming.API.Repositories;

public class GameCollectionRepository : IGameCollectionRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DatabaseSettings> _databaseSettings;

    private string TableName => _databaseSettings.Value.GameCollectionTableName;


    public GameCollectionRepository(IAmazonDynamoDB dynamoDb, IOptions<DatabaseSettings> databaseSettings)
    {
        _dynamoDb = dynamoDb;
        _databaseSettings = databaseSettings;
    }

    public async Task<bool> CreateAsync(GameCollection game, CancellationToken cancellationToken)
    {
        var customerJson = JsonSerializer.Serialize(game);
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

    public async Task<IEnumerable<GameCollection>> GetAllAsync(CancellationToken cancellationToken)
    {
        var scanRequest = new ScanRequest()
        {
            TableName = TableName,
            Limit = 50,
            ConsistentRead = true
        };

        var response = await _dynamoDb.ScanAsync(scanRequest, cancellationToken);

        var itemsAsDocument = response.Items.Select(e =>
        {
            var document = Document.FromAttributeMap(e);
            return JsonSerializer.Deserialize<GameCollection>(document.ToJson());
        }).OfType<GameCollection>();

        return itemsAsDocument;
    }

    public async Task<IEnumerable<GameCollection>> GetSubCollections(Guid parentId, CancellationToken cancellationToken)
    {
        var scanRequest = new ScanRequest()
        {
            TableName = TableName,
            Limit = _databaseSettings.Value.Limit,
            ConsistentRead = true,
            ScanFilter = new Dictionary<string, Condition>()
            {
                {
                    "collection_id", new Condition()
                    {
                        ComparisonOperator = ComparisonOperator.EQ, AttributeValueList = new List<AttributeValue>()
                        {
                            new AttributeValue()
                            {
                                S = parentId.ToString()
                            }
                        }
                    }
                }
            }
        };

        var response = await _dynamoDb.ScanAsync(scanRequest, cancellationToken);

        var itemsAsDocument = response.Items.Select(e =>
        {
            var document = Document.FromAttributeMap(e);
            return JsonSerializer.Deserialize<GameCollection>(document.ToJson());
        }).OfType<GameCollection>().ToArray();

        return itemsAsDocument;
    }

    public async Task<GameCollection?> GetAsync(Guid id, CancellationToken cancellationToken)
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
        return JsonSerializer.Deserialize<GameCollection>(itemAsDocument.ToJson());
    }
}