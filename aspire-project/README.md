# Aspire project.cookiecutter

A cookiecutter template for generating a .NET Aspire-based Web API solution with built-in support for:

- OAuth authentication
- Azure services (Key Vault, Storage, etc.)
- Auditing (PostgreSQL or MongoDB support)

The template includes automation to keep your generated project in sync. When the upstream template updates, a GitHub Action automatically:
- Creates a new branch
- Applies the latest template changes
- Opens a pull request for review

---

## 🧱 Solution Structure

The solution is organized into the following projects:

| Project           | Purpose                                                       |
| ----------------- | ------------------------------------------------------------- |
| `Core`            | Domain models and core logic                                  |
| `Infrastructure`  | Integrations (Azure, telemetry, encryption)                   |
| `API`             | ASP.NET Core Web API endpoints                                |
| `Abstractions`    | Shared contracts and interfaces                               |
| `AppHost`         | Aspire host application                                       |
| `ServiceDefaults` | Default setup for Aspire (e.g., OpenTelemetry, health checks) |
| `Database`        | EF Core database setup and seed data                          |
| `Migrations`      | EF Core migration scripts                                     |
| `Tests`           | Unit and integration tests                                    |

---

## 🗄 Database Support

### 🔹 PostgreSQL (Recommended for Auditing)

- Aspire automatically provisions the PostgreSQL database.
- **Schema and tables are not auto-created** — you must run SQL manually or from a script with migrations.

#### 🔐 Auditing & Encryption Setup

To enable field-level encryption and auditing:
- Use the provided `createSample.sql` file.
- This includes:
  - `pgcrypto` extension for encryption
  - SQL functions for `pgp_sym_encrypt`/`decrypt`
  - An audit table with required permissions

**Note:**  
You can also add `pgcrypto` manually using your DB client:
- Navigate to the `database/extensions` folder
- Enable the `pgcrypto` extension in the **public** schema
- Grant **public** access to `pgp_sym_encrypt`

---

### 🔹 MongoDB

- Aspire **does not automatically create** MongoDB databases or collections.
- You must provision them manually or use the provided Bicep deployment templates.

---

## 🚀 Deployment Notes

### CosmosDB for MongoDB (⚠️ Aspire Limitation)

As of **April 4, 2025**, Aspire does **not support Azure Cosmos DB for MongoDB** deployments directly.

To work around this:
- An `infra/` folder is included in the `AppHost` project
- It contains **Bicep templates** for deploying Cosmos DB–compatible MongoDB
- Customize the bicep file as needed.

**Default MongoDB Bicep Config:**

- **API Type:** MongoDB (RU-based throughput)
- **Workload Type:** Development/Testing
- **Region:** East US 2
- **File:** `infra/mongo.bicep`

📖 [Microsoft Docs – Deploy MongoDB via Bicep](https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/manage-with-bicep)



