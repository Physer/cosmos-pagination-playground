using Bogus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace CosmosPagination.Cosmos;

internal class Repository(CosmosClient cosmosClient, ILogger<Repository> logger) : IRepository
{
    private readonly CosmosClient _cosmosClient = cosmosClient;
    private readonly ILogger<Repository> _logger = logger;

    private const string _databaseName = "cosmosplayground";
    private const string _containerName = "products";
    private const string _partitionKey = "id";

    public async Task Seed(int count = 1000)
    {
        _logger.LogInformation("Seeding {Count} products", count);

        _logger.LogInformation("Getting database and creating container if it does not exist");
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
        var containerResponse = await databaseResponse
            .Database
            .CreateContainerIfNotExistsAsync(_containerName, $"/{_partitionKey}");
        _logger.LogInformation("Created {ContainerId} Cosmos container", containerResponse.Container.Id);

        _logger.LogInformation("Generating {Count} dummy products", count);
        var products = new Faker<Product>()
            .CustomInstantiator(faker => new(Guid.NewGuid().ToString(), faker.Commerce.ProductName(), decimal.Parse(faker.Commerce.Price())))
            .Generate(count);

        _logger.LogInformation("Upserting products...");
        foreach (var product in products)
            await containerResponse.Container.UpsertItemAsync(product, new(_partitionKey));
        _logger.LogInformation("Upserted {Count} products", count);
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        _logger.LogInformation("Retrieving all items from Cosmos");
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
        var containerResponse = await databaseResponse
            .Database
            .CreateContainerIfNotExistsAsync(_containerName, $"/{_partitionKey}");

        List<Product> products = [];
        using FeedIterator<Product> resultSet = containerResponse.Container.GetItemQueryIterator<Product>(queryDefinition: null, requestOptions: new QueryRequestOptions()
        {
            PartitionKey = new PartitionKey(_partitionKey)
        });
        
        while (resultSet.HasMoreResults)
        {
            _logger.LogInformation("Items are available in Cosmos");
            _logger.LogInformation("Reading next batch of items");
            FeedResponse<Product> response = await resultSet.ReadNextAsync();
            products.AddRange(response);
        }
        
        _logger.LogInformation("Retrieved {Count} items from Cosmos", products.Count);
        return products;
    }
}
