# `project.cookiecutter`

A template for scaffolding a modern .NET 9 Web API solution with support for:

- [OAuth 2.0](https://oauth.net/2/)
- [Azure services](https://azure.microsoft.com/)
- Auditing
- Deployment options for both [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) and Docker environments

---

## 🧱 Solution Structure

The generated solution contains the following projects:

| Project           | Description                              |
| ----------------- | ---------------------------------------- |
| `Core`            | Core application logic                   |
| `Infrastructure`  | Azure, logging, storage, telemetry, etc. |
| `API`             | Web API endpoints                        |
| `Abstractions`    | Shared contracts and interfaces          |
| `HostingApp`      | Aspire host process (Aspire only)        |
| `ServiceDefaults` | Aspire extensions and configuration      |
| `Database`        | Entity definitions and configuration     |
| `Migrations`      | EF Core migrations                       |
| `Tests`           | Unit/integration test projects           |

---

## 🛠 Prerequisites

### Required Software

- [Cookiecutter](https://cookiecutter.readthedocs.io/)
  ```bash
  python3 -m pip install --user cookiecutter
  ```
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 9.x SDK](https://dotnet.microsoft.com/)

### Optional: OAuth & Azure Integration

If you plan to enable OAuth or Azure integrations (Key Vault, Application Insights, Storage, Service Bus), you'll need the following information available for **each environment**.

#### OAuth Configuration

- Application Name
- Audience (per environment)
- Domain (per environment)

#### Azure Configuration

- Tenant ID
- Subscription ID
- Region/Location

##### Key Vault

- Key Vault Name (per environment)

##### Storage

- Connection string
- Storage container name
- Storage account name

##### Service Bus

- Service Bus namespace name

---

## 🚀 Project Setup

You can generate the solution from either:

- The GitHub repository URL
- A local clone of `project.cookiecutter`

Example:
```bash
cookiecutter gh:your-org/project.cookiecutter
# or
cookiecutter path/to/local/project.cookiecutter
```

---

## 🧰 Project Modes

During setup, you'll choose between two project modes:

- **Aspire** 
- **Docker**

---

## 🌱 Aspire Mode

### Setup Steps

<details>
<summary><strong>Command Line Setup</strong></summary>

1. Create a project folder  
2. Navigate to the folder  
3. Run Cookiecutter  
   ```bash
   cookiecutter gh:your-org/project.cookiecutter
   ```
4. Open the generated solution in Visual Studio  
5. Run the `HostingApp` project
</details>

<details>
<summary><strong>Using a Local Clone</strong></summary>

1. Clone the `project.cookiecutter` repository  
2. Create a project folder  
3. Run Cookiecutter from that path  
4. Open the solution in Visual Studio  
5. Run the `HostingApp` project
</details>

---

## 🐳 Docker Mode

### Setup Steps

<details>
<summary><strong>Command Line Setup</strong></summary>

1. Create a project folder  
2. Navigate to the folder  
3. Run Cookiecutter  
   ```bash
   cookiecutter gh:your-org/project.cookiecutter
   ```
4. Open the solution in Visual Studio  
5. Run `docker-compose` in Debug mode  
6. (If using OAuth) Add credentials to **Manage User Secrets**
</details>

<details>
<summary><strong>Using a Local Clone</strong></summary>

1. Clone the `project.cookiecutter` repository  
2. Create a project folder  
3. Navigate to the folder  
4. Run Cookiecutter  
5. Open the solution in Visual Studio  
6. Run `docker-compose` in Debug mode  
7. (If using OAuth) Add credentials to **Manage User Secrets**
</details>

---
## 🛠️ GitHub Setup

This project relies on two reusable workflows stored in the [`shared-workflows`](https://github.com/your-org/shared-workflows) repository:

- `validate_template_inputs.yml`
- `template_update.yml`

To ensure they function correctly, configure your GitHub project settings as follows:

### 1. Enable Reusable Workflows

Navigate to: **Settings** > **Actions** > **General**

- Under **Actions permissions**, select:
  > ✅ **Allow all actions and reusable workflows**

- Under **Workflow permissions**, select:
  > ✅ **Read repository contents and packages**

### 2. Define Required Repository Variable

Navigate to: **Settings** > **Secrets and variables** > **Actions** > **Variables** tab

- Add a variable named: `TemplateUpdateBranch`
- Set its value to the name of the branch you want updated by the template (e.g., `main`, `template`, etc.)

### 3. Scheduled Template Updates

By default, the workflow is scheduled to run every **Monday at 7:00 AM (UTC)**.  
It will:

- Look for the `TemplateUpdateBranch` repository variable.
- Fail if the variable is missing.
- Create a pull request if there are any changes detected in the upstream template.

### 4. Manual Update

You can also trigger the update manually via the **Actions** tab.  
When run manually, the workflow will prompt you to specify which branch should be updated.

> ⚠️ **Note:** If your repo is public and  hasn’t had any commits/issues/discussions in the past 60 days, GitHub silently disables scheduled workflows. 
> 
> For additional information on the scheduled action [see documentation](https://docs.github.com/en/actions/reference/workflows-and-actions/events-that-trigger-workflows#schedule)
>
---

## 📄 Additional Info

Each mode (`Aspire`, `Docker`) includes a dedicated `README.md` within its generated folder for environment-specific setup and configuration instructions.