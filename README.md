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
