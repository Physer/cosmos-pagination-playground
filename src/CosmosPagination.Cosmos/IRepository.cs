

namespace CosmosPagination.Cosmos;

public interface IRepository
{
    Task Delete();
    Task<IEnumerable<Product>> GetAll();
    Task Seed(int count = 1000);
}
