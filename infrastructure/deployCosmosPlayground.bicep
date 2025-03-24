targetScope = 'subscription'

param environment string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2024-11-01' = {
  name: 'rg-cosmos-playground-${environment}'
  location: deployment().location
}

module cosmos 'modules/cosmos.bicep' = {
  name: 'deployCosmos'
  scope: resourceGroup
}

module appService 'modules/appService.bicep' = {
  name: 'deployAppService'
  scope: resourceGroup
  params: {
    cosmosAccountName: cosmos.outputs.accountName
  }
}
