"""
post_gen_project.py – runs after cookiecutter project generation.

Designed to work both:
• Interactively on dev machines (Windows/macOS/Linux)
• Non-interactive in CI (e.g., GitHub Actions ubuntu-latest runner)
"""

from __future__ import annotations

import json
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Iterable

# --------------------------------------------------------------------------- #
# Paths
# --------------------------------------------------------------------------- #
ROOT = Path.cwd()                                    # project root
SRC  = ROOT / "src" / "{{ cookiecutter.assembly_name }}"

# --------------------------------------------------------------------------- #
# Helpers
# --------------------------------------------------------------------------- #
def rm(path: Path | str) -> None:
    """Delete a file or directory if it exists (idempotent)."""
    p = Path(path)
    if not p.exists():
        return
    if p.is_dir():
        shutil.rmtree(p)
        print(f"🗑️  Removed dir  : {p.as_posix()}")
    else:
        p.unlink()
        print(f"🗑️  Removed file : {p.as_posix()}")

def rm_each(paths: Iterable[Path | str]) -> None:
    for p in paths:
        rm(p)

# --------------------------------------------------------------------------- #
# Evaluate cookiecutter answers (string literals after render)
# --------------------------------------------------------------------------- #
include_azure   = "{{ cookiecutter.include_azure }}"       == "yes"
database_is_pg  = "{{ cookiecutter.database }}"            == "PostgreSql"
include_audit   = "{{ cookiecutter.include_audit }}"       == "yes"
include_oauth   = "{{ cookiecutter.include_oauth }}"       == "yes"
aspire_deploy   = "{{ cookiecutter.aspire_deploy }}"       == "yes"
github_deploy   = "{{ cookiecutter.github_deployment }}"   == "yes"
project_path    = "{{ cookiecutter.project_path }}"
template_path   = "{{ cookiecutter.template_path }}"

# --------------------------------------------------------------------------- #
# 1️⃣  Clean-up unneeded files
# --------------------------------------------------------------------------- #
if not include_azure:
    rm_each([
        SRC / "Extensions" / "ApplicationInsightsExtension.cs",
        SRC / "Extensions" / "AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Vault",
        ROOT / ".github/workflows/ProdDeployment.yml",
        ROOT / ".github/workflows/ProdProvisioning.yml",
        ROOT / ".github/workflows/StagingDeployment.yml",
        ROOT / ".github/workflows/StagingProvisioning.yml",
        ROOT / ".github/workflows/DbMigrations_Production.yml",
        ROOT / ".github/workflows/DbMigrations_Staging.yml",
    ])

if database_is_pg:
    # Remove Mongo artefacts
    mongo_root = ROOT / "src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}"
    rm_each([
        mongo_root / "Extensions/MongoExtensions.cs",
        mongo_root / "Services/MongoDbService.cs",
        mongo_root / "BsonCollectionAttribute.cs",
        mongo_root.parent / ".Data.Abstractions/Services/IMongoDbService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/administration/users/user.json",
    ])
else:
    # Remove Postgres artefacts
    pg_root = ROOT / "src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}"
    rm_each([
        pg_root / "DbConnectionProvider.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/CreateUsers.sql",
    ])

if not include_audit:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Infrastructure/AuditSetup.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Infrastructure/ListAuditEvent.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Infrastructure/ListAuditModel.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Abstractions/Secure.cs",
    ])

if not include_oauth:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Identity/AuthService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Identity/CryptoRandom.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Extensions/AuthPolicyExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Infrastructure/SecurityRequirementsOperationFilter.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Data.Abstractions/Services/IAuthService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}/Settings.cs",
    ])

# Remove template snippets
rm(ROOT / "src/templates")

# --------------------------------------------------------------------------- #
# 2️⃣  Optional deployment helper
# --------------------------------------------------------------------------- #
if aspire_deploy and include_azure and project_path:
    deploy_script = ROOT / "deployment.py"
    try:
        subprocess.run(
            [
                sys.executable,
                str(deploy_script),
                "{{ cookiecutter.deployment_environment }}",
                "{{ cookiecutter.assembly_name }}",
                str(github_deploy).lower(),
                "{{ cookiecutter.database }}",
                project_path,
                template_path,
            ],
            check=True,
        )
    except subprocess.CalledProcessError as exc:
        print(f"❌ Deployment helper failed: {exc}")
        sys.exit(exc.returncode)

# --------------------------------------------------------------------------- #
# 3️⃣  Persist final context (only if file doesn't already exist)
# --------------------------------------------------------------------------- #
cookie_file = ROOT / ".cookiecutter.json"
if not cookie_file.exists():
    context: dict[str, str] = {
        "include_aspire": "yes",
        "assembly_name": "{{ cookiecutter.assembly_name }}",
        "root_namespace": "{{ cookiecutter.root_namespace }}",
        "api_app_name": "{{ cookiecutter.api_app_name }}",
        "api_web_url": "{{ cookiecutter.api_web_url }}",
        "database": "{{ cookiecutter.database }}",
        "database_name": "{{ cookiecutter.database_name }}",
        "include_audit": "{{ cookiecutter.include_audit }}",
        "include_oauth": "{{ cookiecutter.include_oauth }}",
        "include_azure": "{{ cookiecutter.include_azure }}",
        "aspire_deploy": "{{ cookiecutter.aspire_deploy }}",
    }

    # 🔐 OAuth
    if include_oauth:
        context.update(
            oauth_app_name="{{ cookiecutter.oauth_app_name }}",
            oauth_audience="{{ cookiecutter.oauth_audience }}",
            oauth_api_audience_dev="{{ cookiecutter.oauth_api_audience_dev }}",
            oauth_api_audience_prod="{{ cookiecutter.oauth_api_audience_prod }}",
            oauth_domain_dev="{{ cookiecutter.oauth_domain_dev }}",
            oauth_domain_prod="{{ cookiecutter.oauth_domain_prod }}",
        )

    # ☁️ Azure
    if include_azure:
        context.update(
            azure_tenant_id="{{ cookiecutter.azure_tenant_id }}",
            azure_subscription_id="{{ cookiecutter.azure_subscription_id }}",
            azure_location="{{ cookiecutter.azure_location }}",
            azure_key_vault_staging="{{ cookiecutter.azure_key_vault_staging }}",
            azure_key_vault_prod="{{ cookiecutter.azure_key_vault_prod }}",
            azure_storage_connection_staging="{{ cookiecutter.azure_storage_connection_staging }}",
            azure_container_dev="{{ cookiecutter.azure_container_dev }}",
            azure_container_staging="{{ cookiecutter.azure_container_staging }}",
            azure_container_prod="{{ cookiecutter.azure_container_prod }}",
            azure_storage_account_name_dev="{{ cookiecutter.azure_storage_account_name_dev }}",
            azure_storage_account_name_prod="{{ cookiecutter.azure_storage_account_name_prod }}",
            azure_container_registry_server_staging="{{ cookiecutter.azure_container_registry_server_staging }}",
            azure_container_registry_user_staging="{{ cookiecutter.azure_container_registry_user_staging }}",
            azure_container_registry_server_prod="{{ cookiecutter.azure_container_registry_server_prod }}",
            azure_container_registry_user_prod="{{ cookiecutter.azure_container_registry_user_prod }}",
        )

    # 🚀 Aspire deployment
    if aspire_deploy:
        context.update(
            deployment_environment="{{ cookiecutter.deployment_environment }}",
            project_path=project_path,
            github_deployment=str(github_deploy).lower(),
            template_path=template_path,
        )

    # ✅  Wrap context so replay files work: {"cookiecutter": {...}}
    cookie_file.write_text(json.dumps({"cookiecutter": context}, indent=4))
    print("✅  .cookiecutter.json written")

print("🎉 Post-gen hook completed successfully")
