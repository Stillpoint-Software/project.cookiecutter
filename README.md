# project.cookiecutter

**project.cookiecutter** provides a streamlined process to setup a web API project with support for *OAuth*, *Azure*, and *Auditing* in both **Aspire** and **Docker** modes.  

If there are any updates to this template, there is a github process that will create a new branch and pull requested.

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

install Cookiecutter and Cruft using the following command:

 ``python3 -m pip install --user cookiecutter``
 ``pip3 install cruft``

### Project Setup

You can set up your project in two ways:

**GitHub URI**: Point to the project.cookiecutter GitHub repository.
**Local Clone**: Clone the project.cookiecutter repository locally and run the command from your machine.

Example command:

``cruft create {uri/path to project.cookiecutter}``

**Note: You cannot change the defaults if using the GitHub setup.**

### Default settings -- maybe
If you cloned the repository, you can customize the default settings by editing the **.cookiecutterrc** file located at the root of the cookiecutter project.

Example command:

``cookiecutter {path to project.cookiecutter}   --overwrite-if-exists  --config-file={ path to the .cookiecutterrc file }``


## Project Modes
You can create a project in one of two modes: **Aspire** or **Docker**. During setup, you’ll be asked whether you want to use Aspire. Selecting **"No"** defaults to Docker.

### Aspire Mode

When using Aspire mode, you must have OAuth set up and an active Azure subscription. Below is the information you will need to proceed.

#### Gather Information 

***OAuth information***
if using OAuth, You will need to have the following information for all environments:

1.  Application Name
2.  Audience
3.  Domain

#### Setup

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

When using this Mode, you will need to have OAuth and Azure already setup and configured.  Below is the information you will need to proceed.

#### Gather Information 

***OAuth information***
If you use OAuth, you will need to have the following information for all environments:

1.  Application Name
2.  Audience
3.  Domain
  
***Azure information***
If you use Azure, you will need to have the following information:

1. Tenant Id 
2. Subscription Id
3. Location
4. Key Vault (all environments) 
   1. ex:{projectName}-Staging
5. Storage connection string
6. Storage container name (all environments)
   1. ex: {projectName}assetsstaging
7. Container Name (all environments)
   1. ex: staging
8. Container Registry Name (all environments)
   1. ex: cr3hmn6weg7opbk.azurecr.io
9.  Container User (all environments)
    1.  ex: cr3hmn6weg7opbk

#### Setup 

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