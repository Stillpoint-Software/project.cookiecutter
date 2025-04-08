# project.cookiecutter

**project.cookiecutter** provides a streamlined project setup, including OAuth, Azure, and Auditing configurations for both **Aspire** and **Docker**

## Solution Structure

The solution consists of the following projects:

- **Base Project** (Docker only)
- **API**
- **Abstractions**
- **Database**
- **Migrations**
- **Tests**

---

## Cookiecutter Setup

install ``python3 -m pip install --user cookiecutter``

## Project Setup

To setup your project, you can run the cookiecutter command setup in two ways.  The first, you can point to the **project.cookiecutter** GitHub Uri. Or, you can clone the `project.cookiecutter` Project and run the command from your machine.

Here is an example:
``cookiecutter {uri/path to project.cookiecutter} ``

*NOTE: You can't change the defaults if using the GitHub setup.*

## Default settings
If you cloned the repository, you can customize some of the default settings by editing the **.cookiecutterrc** file.

``cookiecutter {path to project.cookiecutter}   --overwrite-if-exists  --config-file={ path to the .cookiecutterrc file }``

# Project Modes
You can create a project in one of two modes: **Aspire** or **Docker**.
During setup, you’ll be asked whether you want to use Aspire. Selecting "No" defaults to Docker.

## Docker Mode

**Command Line Setup**

1. Create a directory for your project
2. Navigate to the project folder
3. Run the command
4. Open the solution in Visual Studio
5. Run docker-compose in **Debug** mode
6. If using OAuth, retrieve your OAuth credentials and store them in **Manage User Secrets**.
   

**Cloning Setup**

1. Clone the `project.cookiecutter`
2. Create a project folder
3. Navigate to the project folder
4. Run the command
5. Open the solution in Visual Studio
6. Run docker-compose in **Debug** mode
7. If using OAuth, retrieve your OAuth credentials and store them in **Manage User Secrets**.
------

## Aspire Mode

**Command Line Setup**

1.  Create a directory for your project
2.  Navigate to the project folder
3.  Run the command
4.  Open the solution in Visual Studio
5.  Run the **Hosting** project
   
**Cloning Setup**

1.  Clone the `project.cookiecutter`
2.  Create a project directory
3.  Navigate to the project folder
4.  Run the command
5.  Open the solution in Visual Studio
6.  Run the **Hosting** project

**Deployment**

You can use aspire for deployment.  However, you will need to manually create a bicep file when using MongoDb since CosmoDB for MongoDb is not integrated with Aspire.

1.  Open **Developer Powershell** in VS or open **Powershell**
2.  Navigate to the folder where the solution file resides.
3.  run `azd init`
    1.  This will ask for the environment (develop, staging, production) you want to create.
4.  run `azd pipeline config -e {environment-name}` which will setup Github
    1.  Select the Azure subscription
    2.  Select the location
    3.  Enter in the DbPassword (if using a database)
5.  If using MongoDb, DO NOT COMMIT AND PUSH
6.  Continue with **Deployment - MongoDb**
7.  If NOT using MongoDb, commit and push

**Deployment - MongoDb**

At this time (04/04/2025), aspire does not currently support **Azure Cosmos DB for MongoDB** deployment. Therefore; you will need to create a bicep file manually in order for aspire to create the database.

1.  Navigate to the folder where the colution file resides.
2.  run `azd infra synth`.  This will create an infra folder under the AppHost folder which will contain all the bicep files
3.  Add the following to the **main.bicep** file
    `module mongodb 'mongodb/mongodb.module.bicep' = {
      name: 'mongodb'
      scope: rg
      params: {
      location: location
      }
    }`

4.  Navigate to the infra folder
5.  Create a folder named **mongo**
6.  Navigate to the new folder
7.  Create a file called **mongo.module.bicep** 
8.  Add the fillowing
   >@description('The location for the resource(s) to be deployed.')
    param location string = resourceGroup().location<br>
    @description('Cosmos DB for MongoDb account name')
    param accountName string = 'mongodb-${uniqueString(resourceGroup().id)}' <br>
    resource mongoDb 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' = {
      name: accountName
      kind: 'MongoDB'
      location: location
      tags: {
        defaultExperience: 'Azure Cosmos DB for MongoDB API'
        'hidden-workload-type': 'Development/Testing'
        'hidden-cosmos-mmspecial': ''
      }
      identity: {
        type: 'None'
      }
      properties: {
        publicNetworkAccess: 'Enabled'
        enableAutomaticFailover: false
        enableMultipleWriteLocations: false
        isVirtualNetworkFilterEnabled: false
        virtualNetworkRules: []
        disableKeyBasedMetadataWriteAccess: false
        enableFreeTier: false
        enableAnalyticalStorage: false
        analyticalStorageConfiguration: {
          schemaType: 'FullFidelity'
        }
        databaseAccountOfferType: 'Standard'
        capacityMode: 'Serverless'
        defaultIdentity: 'FirstPartyIdentity'
        networkAclBypass: 'None'
        disableLocalAuth: false
        enablePartitionMerge: false
        enablePerRegionPerPartitionAutoscale: false
        enableBurstCapacity: false
        enablePriorityBasedExecution: false
        minimalTlsVersion: 'Tls12'
        consistencyPolicy: {
          defaultConsistencyLevel: 'Session'
          maxIntervalInSeconds: 5
          maxStalenessPrefix: 100
        }
        apiProperties: {
          serverVersion: '7.0'
        }
        locations: [
          {
            locationName: 'East US 2'
            failoverPriority: 0
            isZoneRedundant: false
          }
        ]
        cors: []
        capabilities: [
          {
            name: 'EnableMongo'
          }
        ]
        ipRules: []
        backupPolicy: {
          type: 'Periodic'
          periodicModeProperties: {
            backupIntervalInMinutes: 240
            backupRetentionIntervalInHours: 8
            backupStorageRedundancy: 'Geo'
          }
        }
        networkAclBypassResourceIds: []
        diagnosticLogSettings: {
          enableFullTextQuery: 'None'
        }
        capacity: {
          totalThroughputLimit: 4000
        }
      }
    }
9.  The **hidden-workload-type** options are 'Development/Testing' or 'Production'
10. The **location** is set for **East US 2**
11. Update settings as needed
12. Commit and push
   
---
## Oauth, Azure, Migrations

If you are not using Aspire for deployment, you will need to have the oAuth, Azure, and a database already setup.