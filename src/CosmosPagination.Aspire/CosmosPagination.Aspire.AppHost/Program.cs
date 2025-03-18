var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CosmosPagination_Web>("web");

builder.Build().Run();
