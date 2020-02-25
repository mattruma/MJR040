# Introduction

This is an example of leveraging the [Microsoft.Azure.Cosmos.Table](https://www.nuget.org/packages/Microsoft.Azure.Cosmos.Table) NuGet package in a repository pattern.

## Prerequisites

The following prerequisites are required:

* [NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core)
* [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator)
* [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)
* [Visual Studio 2019](https://visualstudio.microsoft.com/vs/), Azure Function tools are included in the Azure developlment workload of Visual Studio 2019

## Projects

**ClassLibrary1.csproj**

This project contains the standard interfaces and abstract classed to support how Azure Table Storage should be implemente for reading and writing.

The `Entity` classes are for root entities, e.g. `ToDo`, while the `ChildEntity` classes are for children of root entities, e.g. `ToDoComment`. 

From a code perspective, the main difference between `Entity` and `ChildEntity` is that `ChildEntity` is open to having a `ParentId` property which is required for most calls using the `ChildEntityDataStore`. 

**ClassLibrary1.Tests.csproj**

Contains the tests for the `ClassLibrary1` project.

The tests cover valid configuration options.

**FunctionApp1.csproj**

This project contains the concrete implementations of the `ToDo` and `ToDoComment` entities and data stores - the repositories in a repository design pattern - that are persisted to Azure Table Storage.

It also contains Azure Functions, and supporting classes, for CRUD operations: create, retrieve, update and delete. 


**FunctionApp1.Tests.csproj**

Contains the tests for the `FunctionApp1` project.

The tests cover reading and writing data use the `EntityDataStore` classes.

Currently the tests are setup to run locally using the Azure Storage Emulator.

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "AzureTableStorageOptions:ConnectionString": "UseDevelopmentStorage=true"
}

```

To use storage in Azure, update the `AzureTableStorageOptions:ConnectionString` property with your Primary or Secondary connection strings.

## Related Articles

* https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-local#local-development-environments
