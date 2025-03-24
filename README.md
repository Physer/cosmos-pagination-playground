# cosmos-pagination-playground

## Introduction

This project serves as a small sample application on the effect and usage of querying large datasets in Azure's Cosmos NoSQL database.
The application implements three different solutions to querying the data.

1. Load all data unpaginated in one go using batches
2. Load paginated data using [LINQ with the OFFSET and LIMIT operators](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/linq-to-sql#supported-linq-operators)
3. Load paginated data based [on Continuation Tokens](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/pagination#continuation-tokens)

## How to run

This section of the README describes how to run the application. The project uses .NET Aspire to run both the Web project and the Cosmos emulator.

The Cosmos emulator is Microsoft's official Cosmos Emulator which runs in a Docker container, using .NET Aspire's [official integration](https://learn.microsoft.com/en-us/dotnet/aspire/database/azure-cosmos-db-integration?tabs=dotnet-cli).

The Web project is an ASP.NET Core Razor Pages project with 2 added pages:

- `Data Management`
- `View Data`

For more information about these pages, refer to the [Instructions](#instructions).

### Prerequisites

- Docker Desktop
- .NET 9
- Aspire workload

### Instructions

1. Run the Aspire App Host project by running `dotnet run --project .\src\CosmosPagination.Aspire\CosmosPagination.Aspire.AppHost`.
   - Alternatively you can open Visual Studio and launch the application through the IDE
2. Open the Aspire Dashboard
3. Wait for all services (the Web project and the Cosmos emulator) to be up, running and healthy
   - Note that the Cosmos emulator might take a while to appear as healthy due to TLS certificates
4. Navigate to the URL of the Web project as shown in the Aspire Dashboard
5. Open the `Data Management` page in the top menu
6. Press `Seed Data`
   - Note that this might take a while, the database will be seeded with a 1000 random fake products
7. Navigate to the `View Data` page in the top menu
8. Press any of the buttons to play around with the different options

- Upon loading the `View Data` page, a call is done to the Cosmos database to retrieve the item count in the database. This information is subsequently stored in `MemoryCache` so every method can retrieve the Cosmos items without hitting Cosmos again
- The unpaginated option does not contain any navigation options for navigating between pages as there is only one page available
- The OFFSET and LIMIT option allows for a pagination setup where you can toggle freely between all available pages. The pagesize is fixed on 25 items per page.
- The Continuation Tokens option allows for moving forwards and backwards between pages but does not allow you to switch to a random page

## Deploy to Azure

If you wish to deploy this setup to Azure, you can do so by deploying the provided Bicep files in `~/infrastructure`.

1. Ensure you have a valid Azure subscription and are logged in (`az login` and `az account show`)
2. Run the following command `az deployment sub create --location _location_here_ --template-file .\infrastructure\deployCosmosPlayground.bicep --parameters environment=_environment_here_`
3. Navigate to the Azure Portal and verify you can see the newly created resource group. The name of the resource group will be `rg-cosmos-playground-${environment}`
