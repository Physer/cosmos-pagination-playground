var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIRECOSMOSDB001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var cosmos = builder.AddAzureCosmosDB("cosmos").RunAsPreviewEmulator(emulator => emulator.WithDataExplorer());
#pragma warning restore ASPIRECOSMOSDB001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.AddProject<Projects.CosmosPagination_Web>("web").WithReference(cosmos);

builder.Build().Run();
