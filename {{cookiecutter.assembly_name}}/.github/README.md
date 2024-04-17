# Inital Project Setup

## Stucture

### Source

All projects should be in a folder with the same name as the project.

### Tests

All test should be in a project related to their project in the tests folder.

### Solution Files

- .github/worflow files include

## Repo Settings

Update the repository settings

### Pull Request Labels

Add the following labels. Label case is important for workflows.

- feature
  - description: Improvements or additions to documentation
  - color: #0075ca
- critical
  - description: Major Release Issue
  - color: #B60205

### Variables

Add the following variable to the repository so that the github action work correctly

- PROJECT_NAME = "Hyperbee.---"
- SOLUTION_NAME = "Hyperbee.---.sln"

### Issue Labels

Default labels should line up with the settings in `issue-branch.yml` and any others that might be useful.

## Dependabot

- Enable and Group PRs
- dependabot.yml should be included with

## Nuget Config

# {{cookiecutter.assembly_name}}

Provides a dependency injection pattern for embedded resources

## Usage
```csharp
# Cool usage of the code!
```

# Status

| Branch     | Action                                                                                                                                                                                                                      |
|------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `develop`  | [![Build status](https://github.com/Stillpoint-Software/{{cookiecutter.assembly_name}}/actions/workflows/publish.yml/badge.svg?branch=develop)](https://github.com/Stillpoint-Software/{{cookiecutter.assembly_name}}/actions/workflows/publish.yml)  |
| `main`     | [![Build status](https://github.com/Stillpoint-Software/{{cookiecutter.assembly_name}}/actions/workflows/publish.yml/badge.svg)](https://github.com/Stillpoint-Software/{{cookiecutter.assembly_name}}/actions/workflows/publish.yml)                 |


[![{{cookiecutter.assembly_name}}](https://github.com/Stillpoint-Software/{{cookiecutter.assembly_name}}/blob/main/assets/hyperbee.jpg?raw=true)](https://github.com/Stillpoint-Software/{{cookiecutter.assembly_name}})

# Help

See [Todo](https://github.com/Stillpoint-Software/{{cookiecutter.assembly_name}}/blob/main/docs/todo.md)
