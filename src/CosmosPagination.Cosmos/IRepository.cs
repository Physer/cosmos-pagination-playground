

namespace CosmosPagination.Cosmos;

public interface IRepository
{
    Task Delete();
    Task<IEnumerable<Product>> GetAll();
    Task<long> GetItemCount();
    Task<IEnumerable<Product>> GetPaginatedResults(int pageNumber, int pageSize);
    Task Seed(int count = 1000);
}
