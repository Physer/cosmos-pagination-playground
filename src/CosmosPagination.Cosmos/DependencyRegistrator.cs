using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CosmosPagination.Cosmos;

public static class DependencyRegistrator
{
    public static void RegisterCosmosDependencies(this IHostApplicationBuilder builder)
    {
        builder.AddAzureCosmosClient("cosmos");
        builder.Services.AddTransient<IRepository, Repository>();
    }
}
