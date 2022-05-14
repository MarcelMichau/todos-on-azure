targetScope = 'subscription'

param appName string
param appShortName string

param location string = 'South Africa North'
var resourceGroupName = 'rg-${appName}'

module resourceGroup 'br:acrmarcelmichau.azurecr.io/bicep/modules/resource-group:v0.1' = {
  name: resourceGroupName
  params: {
    resourceGroupName: resourceGroupName
    location: location
  }
}

resource appResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' existing = {
  name: resourceGroup.name
}

module storageAccount 'br:acrmarcelmichau.azurecr.io/bicep/modules/storage-account:v0.1' = {
  name: 'st${appShortName}'
  params: {
    storageAccountName: 'st${appShortName}'
    location: location
  }
  scope: appResourceGroup
}

module appServicePlan 'br:acrmarcelmichau.azurecr.io/bicep/modules/app-service-plan:v0.1' = {
  name: 'plan-${appName}'
  params: {
    appServicePlanName: 'plan-${appName}'
    location: location
    kind: 'windows'
    sku: {
      name: 'F1'
      tier: 'Free'
    }
  }
  scope: appResourceGroup
}

module functionApp 'br:acrmarcelmichau.azurecr.io/bicep/modules/function-app:v0.1' = {
  name: 'func-${appName}'
  params: {
    functionAppName: 'func-${appName}'
    location: location
    appServicePlanName: appServicePlan.outputs.name
    storageAccountName: storageAccount.outputs.name
    netFrameworkVersion: 'v6.0'
  }
  scope: appResourceGroup
}

module sqlServer 'br:acrmarcelmichau.azurecr.io/bicep/modules/sql-server:v0.1' = {
  name: 'sql-${appName}'
  params: {
    serverName: 'sql-${appName}'
    location: location
    azureAdAdministratorLogin: 'azure-sql-administrators'
    azureAdAdministratorObjectId: '6fe16a87-222a-4a1a-b820-9b7fd37b44a8'
  }
  scope: appResourceGroup
}

module sqlDatabase 'br:acrmarcelmichau.azurecr.io/bicep/modules/sql-database:v0.1' = {
  name: 'sqldb-${appName}'
  params: {
    location: location
    databaseName: 'sqldb-${appName}'
    sqlServerName: sqlServer.outputs.name
  }
  scope: appResourceGroup
}
