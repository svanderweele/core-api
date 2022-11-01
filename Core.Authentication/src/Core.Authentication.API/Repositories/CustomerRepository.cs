using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Core.Authentication.API.Contracts.Data;
using Core.Authentication.API.Settings;
using Microsoft.Extensions.Options;

namespace Core.Authentication.API.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<ConnectionStringsSettings> _databaseSettings;

    private string TableName => _databaseSettings.Value.UsersTableName;


    public CustomerRepository(IAmazonDynamoDB dynamoDb, IOptions<ConnectionStringsSettings> databaseSettings)
    {
        _dynamoDb = dynamoDb;
        _databaseSettings = databaseSettings;
    }

    public async Task<bool> CreateAsync(CustomerDto customer, CancellationToken cancellationToken)
    {
        var customerJson = JsonSerializer.Serialize(customer);
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

    public async Task<CustomerDto?> GetAsync(string id, CancellationToken cancellationToken)
    {
        var getRequest = new GetItemRequest()
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>()
            {
                { "email", new AttributeValue() { S = id.ToString() } }
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
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<bool> UpdateAsync(CustomerDto customer, CancellationToken cancellationToken)
    {
        var customerJson = JsonSerializer.Serialize(customer);
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
                { "pk", new AttributeValue() { S = id.ToString() } },
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(deleteRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

}