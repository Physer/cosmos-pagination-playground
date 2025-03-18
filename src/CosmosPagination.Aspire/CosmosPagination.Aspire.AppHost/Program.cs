var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CosmosPagination_Web>("web");

builder.AddAzureCosmosDB("cosmos").RunAsEmulator();

builder.Build().Run();
