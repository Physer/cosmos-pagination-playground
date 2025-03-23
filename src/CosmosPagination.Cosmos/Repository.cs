using Bogus;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Database = Microsoft.Azure.Cosmos.Database;

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
        _logger.LogInformation("Seeding database with {Count} entries", count);
        var (_, container) = await GetDatabaseAndContainer();
        var products = new Faker<Product>()
            .CustomInstantiator(faker => new(Guid.NewGuid().ToString(), faker.Commerce.ProductName(), decimal.Parse(faker.Commerce.Price())))
            .Generate(count);

        foreach (var product in products)
            await container.UpsertItemAsync(product, new(_partitionKey));
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        _logger.LogInformation("Retrieving all items unpaginated");
        var (_, container) = await GetDatabaseAndContainer();
        List<Product> products = [];
        using FeedIterator<Product> resultSet = container.GetItemQueryIterator<Product>(queryDefinition: null, requestOptions: new QueryRequestOptions()
        {
            PartitionKey = new PartitionKey(_partitionKey)
        });

        while (resultSet.HasMoreResults)
        {
            FeedResponse<Product> response = await resultSet.ReadNextAsync();
            products.AddRange(response);
        }
        return products;
    }

    public async Task<long> GetItemCount()
    {
        _logger.LogInformation("Retrieving item count");
        var (_, container) = await GetDatabaseAndContainer();
        var queryDefinition = new QueryDefinition("SELECT VALUE COUNT(1) FROM c");
        var queryRequestOptions = new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(_partitionKey)
        };

        using var queryIterator = container.GetItemQueryIterator<long>(queryDefinition, requestOptions: queryRequestOptions);
        long count = 0;

        while (queryIterator.HasMoreResults)
        {
            var response = await queryIterator.ReadNextAsync();
            count += response.Resource.FirstOrDefault();
        }

        return count;
    }

    public async Task<IEnumerable<Product>> GetPaginatedResults(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Retrieving paginated results with SKIP and TAKE for page {PageSize} on page {PageNumber}", pageSize, pageNumber);
        var (_, container) = await GetDatabaseAndContainer();
        List<Product> products = [];
        using FeedIterator<Product> resultSet = container.GetItemLinqQueryable<Product>().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToFeedIterator();
        while (resultSet.HasMoreResults)
        {
            FeedResponse<Product> response = await resultSet.ReadNextAsync();
            products.AddRange(response);
        }
        return products;
    }

    public async Task<(IEnumerable<Product>, string)> GetTokenPaginatedResults(int pageSize, string? continuationToken = null)
    {
        _logger.LogInformation("Retrieving paginated results with continuation token for page size {PageSize}", pageSize);
        var (_, container) = await GetDatabaseAndContainer();
        List<Product> products = [];

        using FeedIterator<Product> resultSet = container.GetItemQueryIterator<Product>(queryDefinition: null, requestOptions: new QueryRequestOptions()
        {
            MaxItemCount = pageSize,
            PartitionKey = new PartitionKey(_partitionKey)
        }, continuationToken: continuationToken);
        var response = await resultSet.ReadNextAsync();
        products.AddRange(response);

        continuationToken = response.ContinuationToken;
        return (products, continuationToken);
    }

    public async Task Delete()
    {
        _logger.LogInformation("Deleting database and container");
        var (database, _) = await GetDatabaseAndContainer();
        await database.DeleteAsync();
    }

    private async Task<(Database, Container)> GetDatabaseAndContainer()
    {
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
        var database = databaseResponse.Database;
        var container = await database.CreateContainerIfNotExistsAsync(_containerName, $"/{_partitionKey}");
        return (database, container);
    }
}
