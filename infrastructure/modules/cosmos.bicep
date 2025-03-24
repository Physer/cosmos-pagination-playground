param location string = resourceGroup().location

var databaseName = 'cosmosplayground'
var containerName = 'products'
var partitionKeyPath = '/id'
var locations = [
  {
    locationName: location
    failoverPriority: 0
    isZoneRedundant: false
  }
]
var throughputPolicy = {
  Manual: {
    throughput: 400
  }
  Autoscale: {
    autoscaleSettings: {
      maxThroughput: 1000
    }
  }
}

resource account 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' = {
  name: toLower('cosmos-playground-${uniqueString(resourceGroup().id)}')
  location: location
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
    locations: locations
    enableAnalyticalStorage: true
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-12-01-preview' = {
  parent: account
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' = {
  parent: database
  name: containerName
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          partitionKeyPath
        ]
        kind: 'Hash'
      }
    }
    options: throughputPolicy.Autoscale
  }
}

output accountName string = account.name
