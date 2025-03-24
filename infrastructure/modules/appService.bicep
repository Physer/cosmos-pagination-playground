param location string = resourceGroup().location
param sku string = 'B1'
param cosmosAccountName string

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' existing = {
  name: cosmosAccountName
}

var cosmosConnectionString = cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: 'asp-cosmos-playground-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
  kind: 'linux'
}

resource appService 'Microsoft.Web/sites@2024-04-01' = {
  name: 'app-cosmos-playground-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|9.0'
      connectionStrings: [
        {
          connectionString: cosmosConnectionString
          name: 'cosmos'
          type: 'Custom'
        }
      ]
    }
  }
}
