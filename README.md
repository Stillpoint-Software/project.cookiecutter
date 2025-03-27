# project.cookiecutter

This is our basic project setup that includes, OAuth and Azure parameters.

The solution includes the following projects:

- Base Project (docker only)
- Api
- Abstractions
- Database
- Migrations
- Tests

# How to run

You can use the cookiecutter project in two ways. Cloning the project or just run the cookiecutter command.

Install cookiecutter at the command line: python3 -m pip install --user cookiecutter

**Command Line Usage**

1. Create a director for your project
2. Go to the project folder
3. At the command line: cookiecutter {Uri to github project.cookiecutter}
4. Open the solution in VS, and run "docker-compose" in "Debug" mode
5. if using oAuth, get the oauth information and enter in the information into "Manage User Secrets"

**Cloning Usage**

1.  Clone the project.cookiecutter
2.  Create a project folder
3.  At the command line: cookiecutter {path to the cookiecutter.json file}
4.  Open the solution in VS, and run "docker-compose" in "Debug" mode
5.  if using oAuth, get the oauth information and enter in the information into "Manage User Secrets"
