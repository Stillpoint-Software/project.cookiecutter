# project.cookiecutter

`project.cookiecutter` scaffolds a modern web API solution supporting [OAuth 2.0](https://oauth.net/2/), [Azure](https://azure.microsoft.com/), and auditing, with deployment options for both [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) and Docker environments.

## Solution Structure

The solution consists of the following projects:

- **Base Project** (Docker only)
- **API**
- **Abstractions**
- **HostingApp** (Aspire)
- **Database**
- **Migrations**
- **ServiceDefaults** (Aspire)
- **Tests**

---
## Prerequisites

***Software***   

1. Cruft
   -.  ``pip3 install cruft``
2. Cookiecutter
   -.  ``python3 -m pip install --user cookiecutter``
3. Docker Desktop
4. DotNet 9.X

If you need to include OAuth or Azure, you must have OAuth set up and an active Azure subscription. Below is the information you will need to proceed.

***OAuth information***
For OAuth, You will need to have the following information for all environments:

1.  Application Name
2.  Audience for each environment
3.  Domain for each environment

***Azure information***
For Azure, you will need to have the following information for all environments:

1. Tenant Id
2. Subscription Id
3. Location
4. Key Vault Name for each environment
5. Storage connection
6. Storage Container Name for each environment
7. Storage Account Name for each environment
8. Container Registry Server for each environment

### Project Setup

You can set up your project using either of the following methods:

**GitHub URI**: Point to the `project.cookiecutter` GitHub repository.
**Local Clone**: Clone the `project.cookiecutter` repository to your machine and run the setup locally.

Example command:

``cruft create {uri/path to project.cookiecutter}``

## Project Modes
During setup, you’ll be prompted to select a project mode: **Aspire** or **Docker**. If you don’t specify a mode, the setup defaults to **Aspire**.

### Aspire Mode

Aspire projects require an active Azure subscription and OAuth to be configured. Gather the necessary information before proceeding.

#### Setup

**Command Line Setup**

1.  Create a directory for your project
2.  Navigate to the project folder
3.  Run the command ``cruft create {uri/path to project.cookiecutter}``
4.  Open the solution in Visual Studio
5.  Run the **Hosting** project
   
**Cloning Setup**

1.  Clone the `project.cookiecutter`
2.  Create a project directory
3.  Navigate to the project folder
4.  Run the command ``cruft create {uri/path to project.cookiecutter}``
5.  Open the solution in Visual Studio
6.  Run the **Hosting** project

#### Deployment

As of April 4, 2025, Aspire does not support **Azure Cosmos DB for MongoDB** for deployment. Therefore, an **infra** folder will be created under the application host project, containing all the Bicep files needed for deployment, including MongoDB.

By default, the MongoDB Bicep file is configured with:

- MongoDB (RU) configuration.
- hidden-workload-type: Development/Testing.
- locationName: East US 2.

By default, the mongo bicep file is configured to use the **MongoDB (RU)** configuration, with the  *hidden-workload-type* to be 'Development/Testing' and the *locationName* of 'East US 2'

Refer to the Microsoft [documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/manage-with-bicep) for more information.

----

### Docker Mode

#### Setup 

**Command Line Setup**

1. Create a directory for your project
2. Navigate to the project folder
3. Run the command ``cruft create {uri/path to project.cookiecutter}``
4. Open the solution in Visual Studio
5. Run docker-compose in **Debug** mode
6. If using OAuth, retrieve your OAuth credentials and store them in **Manage User Secrets**.
   
**Cloning Setup**

1. Clone the `project.cookiecutter`
2. Create a project folder
3. Navigate to the project folder
4. Run the command ``cruft create {uri/path to project.cookiecutter}``
5. Open the solution in Visual Studio
6. Run docker-compose in **Debug** mode
7. If using OAuth, retrieve your OAuth credentials and store them in **Manage User Secrets**.