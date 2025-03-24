var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos").RunAsEmulator();

builder.AddProject<Projects.CosmosPagination_Web>("web").WithReference(cosmos);

builder.Build().Run();
