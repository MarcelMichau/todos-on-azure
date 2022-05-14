# Todos on Azure

This is a simple todo app, re-implemented using various different Azure services.

Bicep Modules are used to deploy the Azure resources, and these modules can be found [in this repo](https://github.com/MarcelMichau/bicep-modules).

The current list of implementations are as follows:

| Name                        | Description                                                              |
| --------------------------- | ------------------------------------------------------------------------ |
| FunctionAppWithTableStorage | HTTP Trigger Azure Function with Table Storage for Todo persistence      |
| FunctionAppWithSql          | HTTP Trigger Azure Function with Azure SQL Database for Todo persistence |
| FunctionAppWithCosmosDb     | HTTP Trigger Azure Function with Azure Cosmos DB for Todo persistence    |