

namespace CosmosPagination.Cosmos;

public interface IRepository
{
    Task<IEnumerable<Product>> GetAll();
    Task Seed(int count = 1000);
}
