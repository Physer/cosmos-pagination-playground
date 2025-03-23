

namespace CosmosPagination.Cosmos;

public interface IRepository
{
    Task Delete();
    Task<IEnumerable<Product>> GetAll();
    Task<long> GetItemCount();
    Task<IEnumerable<Product>> GetPaginatedResults(int pageNumber, int pageSize);
    Task<(IEnumerable<Product>, string)> GetTokenPaginatedResults(int pageSize, string? continuationToken = null);
    Task Seed(int count = 1000);
}
