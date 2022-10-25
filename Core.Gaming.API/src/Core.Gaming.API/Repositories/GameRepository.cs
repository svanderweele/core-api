using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Core.Gaming.API.Contracts.Data;
using Core.Gaming.API.Settings;
using Microsoft.Extensions.Options;

namespace Core.Gaming.API.Repositories;

public class GameRepository : IGameRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DatabaseSettings> _databaseSettings;

    private string TableName => _databaseSettings.Value.GameTableName;


    public GameRepository(IAmazonDynamoDB dynamoDb, IOptions<DatabaseSettings> databaseSettings)
    {
        _dynamoDb = dynamoDb;
        _databaseSettings = databaseSettings;
    }

    public async Task<bool> CreateAsync(Game game, CancellationToken cancellationToken)
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

    public async Task<ScanResponse?> GetAllAsync(CancellationToken cancellationToken, string? startKey)
    {
        var scanRequest = new ScanRequest()
        {
            TableName = TableName,
            Limit = 2,
            ExclusiveStartKey = startKey != null
                ? new Dictionary<string, AttributeValue>()
                {
                    { "id", new AttributeValue() { S = startKey } }
                }
                : null,
            ConsistentRead = true
        };

        var response = await _dynamoDb.ScanAsync(scanRequest, cancellationToken);
        return response;
    }

    public async Task<IEnumerable<Game>> GetByCollectionId(Guid collectionId, CancellationToken cancellationToken)
    {
        var scanRequest = new ScanRequest()
        {
            TableName = TableName,
            Limit = _databaseSettings.Value.Limit,
            ConsistentRead = true,
            ScanFilter = new Dictionary<string, Condition>()
            {
                {
                    "collections", new Condition()
                    {
                        ComparisonOperator = ComparisonOperator.CONTAINS, AttributeValueList =
                            new List<AttributeValue>()
                            {
                                new()
                                {
                                    S = collectionId.ToString()
                                }
                            }
                    }
                }
            },
        };

        var response = await _dynamoDb.ScanAsync(scanRequest, cancellationToken);

        var itemsAsDocument = response.Items.Select(e =>
        {
            var document = Document.FromAttributeMap(e);
            return JsonSerializer.Deserialize<Game>(document.ToJson());
        }).OfType<Game>().ToArray();

        return itemsAsDocument;
    }


    public async Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken)
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
        return JsonSerializer.Deserialize<Game>(itemAsDocument.ToJson());
    }

    public async Task<bool> UpdateAsync(Game game, CancellationToken cancellationToken)
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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleteRequest = new DeleteItemRequest()
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>()
            {
                { "id", new AttributeValue() { S = id.ToString() } },
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(deleteRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}