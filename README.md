# project.cookiecutter

A Cookiecutter template for scaffolding a **.NET 10 Web API** solution with first-class support for **[Aspire Version 13.1](https://learn.microsoft.com/dotnet/aspire)**. It ships with:

- 🔐 OAuth 2.0 authentication  
- ☁️ Azure integrations (Key Vault, Storage, Application Insights)  
- 📋 Field-level encryption & auditing (PostgreSQL **or** MongoDB)  
- 🤖 GitHub Actions that keep your generated repo in sync with upstream template changes  

When the upstream template updates, a scheduled workflow will:

1. Create a new branch  
2. Re-generate your solution on that branch based on your configuration from the .cookiecutter.json file.  
3. Open a pull request for review  

*(The workflow runs every **Monday at 07:00 UTC** by default and can also be triggered manually.)*

---

## 🧱 Solution Structure

| Project             | Purpose                                                  |
| ------------------- | -------------------------------------------------------- |
| **Core**            | Domain models and core business logic                    |
| **Infrastructure**  | Azure integrations, telemetry, encryption helpers        |
| **API**             | ASP.NET Core Web API endpoints                           |
| **Abstractions**    | Shared DTOs, interfaces and contracts                    |
| **AppHost**         | Aspire host process                                      |
| **ServiceDefaults** | Common Aspire setup (OpenTelemetry, health checks, etc.) |
| **Database**        | EF Core `DbContext`, entity configuration                |
| **Migrations**      | EF Core migration scripts                                |
| **Tests**           | Unit & integration test projects                         |


---

## 🛠 Prerequisites

| Tool                                                     | Purpose                  |
| -------------------------------------------------------- | ------------------------ |
| **Cookiecutter**                                         | Template generation      |
| &nbsp;&nbsp;`python3 -m pip install --user cookiecutter` |
| **.NET 10.x SDK**                                        | Build & run the solution |

### Optional (for OAuth / Azure)

If you plan to use existing Azure Key Vault or Storage resources, you’ll need to set them up ahead of time and enter their connection details during project generation. For local development, you can use emulators instead.

| Service       | Required values                                                                         |
| ------------- | --------------------------------------------------------------------------------------- |
| **OAuth**     | Application name, audience , domain, Api Client Secret & Id, Swagger Client Secret & Id |
| **Azure**     | Tenant ID, Subscription ID, Region, Resource Group Name                                 |
| **Key Vault** | Vault name, Connection String                                                           |
| **Storage**   | Storage Account name, Blob name, Connection string                                      |

When using existing Azure resources or OAuth settings, the provided connection details are written to the Aspire host project’s User Secrets store. These values are created once and are not automatically overwritten or updated.

---

### ⚡️ Quick Start

Create a project directory and navigate into it, then use one of the following options to generate your new solution:
   ```
   mkdir MyApp
   cd MyApp
   ```

#### Option A - run directly from Github

```
cookiecutter gh:your-org/project.cookiecutter --checkout main  
cd <MyApp>
dotnet build
dotnet run --project AppHost   
```

#### Option B - clone locally
Clone the template repo locally and point Cookiecutter at the folder:
```
cookiecutter /path/to/project.cookiecutter
cd <MyApp>
dotnet build
dotnet run --project AppHost   
```   

*(In Visual Studio, simply set **AppHost** as the startup project and Run (F5).)*

---

## 🗄 Database Support

The database project includes sample scripts for both PostgreSQL and MongoDB. You can choose either or both.  

### PostgreSQL (✨ recommended for auditing)

The migrations project includes a sample script `createSample.sql` that creates a sample table.

#### Field-level encryption & audit trail

`createSample.sql` installs the `pgcrypto` extension, defines helper functions for `pgp_sym_encrypt/decrypt`, creates an audit table and grants the required permissions.

If you prefer to manually configure `pgcrypto`:

1. Open your DB client  
2. Enable **pgcrypto** on the database.
3. Grant **public** execute on `pgp_sym_encrypt`

---

### MongoDB

The migrations project includes a sample script `createSampleMongo.js` that creates a sample collection.

---
## 🛠️ GitHub Setup (Reusable Workflows)

This template depends on a workflow stored in **`shared-workflows`**:

| Workflow                      | Purpose                                                                                                    |
| ----------------------------- | ---------------------------------------------------------------------------------------------------------- |
| `project_template_update.yml` | Regenerates the solution when the upstream template changes.  Will create a pull request with the changes. |
| `create_release.yml`          | Creating the release to publish to Nuget.                                                                  |
| `create_test_report.yml`      | Turns uploaded .trx into a GitHub Checks report.                                                           |
| `run_tests.yml`               | Builds and tests the solution. Uploads .trx as artifacts.                                                  |
| `pack_and_publish.yml`        | Packs and Publishes artifacts to Nuget.                                                                    |
| `format.yml`                  | Automatically formats C# with                                                                              |
| `issue_branch.yml`            | Creates a branch when issues.                                                                              |


### Repository settings

1. **Settings → Actions → General**  
   - **Actions permissions:** ✅ *Allow all actions and reusable workflows*  
   - **Workflow permissions:** ✅ *Read repository contents and packages*  

2. **Settings → Secrets and variables → Actions → Variables**  
   - `TEMPLATE_UPDATE_BRANCH = <branch-to-update>`
   - `SOLUTION_NAME = <solutionname.slnx>`

### Scheduled update behaviour

- Runs **every Monday 07:00 GITHUB UTC**.  
- Uses the **.cookiecutter.json** file in the project root to update.
- Fails if `TEMPLATE_UPDATE_BRANCH` is missing from the GitHub Variables.
- Opens a pull request whenever the upstream template SHA changes – even if the only difference is the SHA.
- You can also run the update manually.

> ℹ️ Public repositories without activity for 60 days have their schedules paused automatically by GitHub.

---

## 🔒 Required Post-Creation Setup (Branch Protection)

Generated repos follow a **trunk-based** workflow: all changes PR into `main`, and the `Create Release` workflow bumps `version.json` directly on `main` before creating a draft release. For that `version.json` push to land on a protected branch, the repo's branch protection must allow the `stillpoint-version-bot` GitHub App to bypass PR requirements.

Apply this once after pushing the new repo to GitHub (requires admin/owner on the repo):

```bash
gh api --method PUT repos/Stillpoint-Software/<new-repo>/branches/main/protection \
  -H "Content-Type: application/json" \
  --input - <<'EOF'
{
  "required_status_checks": null,
  "enforce_admins": false,
  "required_pull_request_reviews": {
    "dismiss_stale_reviews": false,
    "require_code_owner_reviews": true,
    "require_last_push_approval": false,
    "required_approving_review_count": 1,
    "bypass_pull_request_allowances": {
      "users": [],
      "teams": ["Admin"],
      "apps": ["stillpoint-version-bot"]
    }
  },
  "restrictions": null,
  "allow_force_pushes": false,
  "allow_deletions": false
}
EOF
```

Without this, `Create Release` will fail with `GH006: Protected branch update failed ... Changes must be made through a pull request.`

The `stillpoint-version-bot` app is installed org-wide; org secrets `RELEASE_APP_ID` and `RELEASE_APP_PRIVATE_KEY` supply its credentials to the shared `set_version.yml` workflow.

## Release Process

Trunk-based, three steps:

1. Squash-merge feature PRs into `main`.
2. Run the **Create Release** workflow on `main` (choose `bump` + increment, or `auto`).
3. Review the draft release and click **Publish release**. Publishing triggers `Pack and Publish` → NuGet.

Branch policy for `auto` mode: `main`/`vX.Y` → stable, `hotfix/*` → `-alpha` patch, feature branches → `-alpha` minor.

---

Happy coding — and happy template-syncing! 🚀
