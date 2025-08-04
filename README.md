# project.cookiecutter

A Cookiecutter template for scaffolding a **modern .NET 9 Web API** solution with first-class support for **[Aspire](https://learn.microsoft.com/dotnet/aspire)**. It ships with:

- 🔐 OAuth 2.0 authentication  
- ☁️ Azure integrations (Key Vault, Storage, Service Bus, Application Insights, etc.)  
- 📋 Field-level encryption & auditing (PostgreSQL **or** MongoDB)  
- 🤖 GitHub Actions that keep your generated repo in sync with upstream template changes  

When the upstream template updates, a scheduled workflow will:

1. Create a new branch  
2. Re-generate your solution on that branch  
3. Open a pull request for review  

*(The workflow runs every **Monday at 07:00 UTC** by default and can also be triggered manually.)*

---

## 🧱 Solution Structure

| Project                      | Purpose                                                  |
| ---------------------------- | -------------------------------------------------------- |
| **Core**                     | Domain models and core business logic                    |
| **Infrastructure**           | Azure integrations, telemetry, encryption helpers        |
| **API**                      | ASP.NET Core Web API endpoints                           |
| **Abstractions**             | Shared DTOs, interfaces and contracts                    |
| **AppHost**  | Aspire host process                                      |
| **ServiceDefaults**          | Common Aspire setup (OpenTelemetry, health checks, etc.) |
| **Database**                 | EF Core `DbContext`, entity configuration     |
| **Migrations**               | EF Core migration scripts                                |
| **Tests**                    | Unit & integration test projects                         |


---

## 🛠 Prerequisites

| Tool                                                     | Purpose                  |
| -------------------------------------------------------- | ------------------------ |
| **Cookiecutter**                                         | Template generation      |
| &nbsp;&nbsp;`python3 -m pip install --user cookiecutter` |
| **.NET 9.x SDK**                                         | Build & run the solution |

### Optional (for OAuth / Azure)

| Service         | Required values                                        |
| --------------- | ------------------------------------------------------ |
| **OAuth**       | Application name, audience (per-env), domain (per-env) |
| **Azure**       | Tenant ID, Subscription ID, Region                     |
| **Key Vault**   | Vault name (per-env)                                   |
| **Storage**     | Account name, container name, connection string             |
| **Service Bus** | Namespace name                                         |

---

## ⚡️ Quick Start

```bash
cookiecutter gh:your-org/project.cookiecutter   # or local path
cd <MyApp>
dotnet build
dotnet run --project AppHost   # or HostingApp
```

*(In Visual Studio, simply set **AppHost/HostingApp** as the startup project and press ▶️ Run.)*

---

## 🗄 Database Support

### PostgreSQL (✨ recommended for auditing)

- Aspire provisions the database container automatically.  
- **Schema is not created automatically** – run the supplied `createSample.sql` (or EF migrations).

#### Field-level encryption & audit trail

`createSample.sql` installs the `pgcrypto` extension, defines helper functions for `pgp_sym_encrypt/decrypt`, creates an audit table and grants the required permissions.

If you prefer to manually configure `pgcrypto`:

1. Open your DB client  
2. Enable **pgcrypto** on the database.
3. Grant **public** execute on `pgp_sym_encrypt`

---

### MongoDB

- Aspire does **not** create databases or collections.  
- Provision them manually

#### Cosmos DB for MongoDB – Aspire limitation

As of **April 4 2025** Aspire cannot deploy Cosmos DB (MongoDB API). 

---
## 🛠️ GitHub Setup (Reusable Workflows)

This template depends on a workflow stored in **`shared-workflows`**:

| Workflow                       | Purpose                                                     |
| ------------------------------ | ----------------------------------------------------------- |
| `template_update.yml`          | Regenerates the solution when the upstream template changes |

### Repository settings

1. **Settings → Actions → General**  
   - **Actions permissions:** ✅ *Allow all actions and reusable workflows*  
   - **Workflow permissions:** ✅ *Read repository contents and packages*  

2. **Settings → Secrets and variables → Actions → Variables**  
   - `TemplateUpdateBranch = <branch-to-update>`

### Scheduled update behaviour

- Runs **every Monday 07:00 UTC**.  
- Fails if `TemplateUpdateBranch` is missing.  
- Opens a pull request whenever the upstream template SHA changes – even if the only diff is the SHA.

> ℹ️ Public repositories without activity for 60 days have their schedules paused automatically by GitHub.

---

## 🚀 Deployment Notes

| Scenario                | Notes                                                        |
| ----------------------- | ------------------------------------------------------------ |
| **Aspire + PostgreSQL** | No automatic schema; apply migrations or `createSample.sql`. |
| **Aspire + MongoDB**    | Use the Bicep template if you need Cosmos DB-compatible API. |

---

Happy coding — and happy template-syncing! 🚀
