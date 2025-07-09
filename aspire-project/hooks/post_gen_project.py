"""
post_gen_project.py – runs after cookiecutter project generation.

Works both:
• Interactively on dev machines (Windows/macOS/Linux)
• Non-interactive in CI runners (e.g., GitHub Actions ubuntu-latest)
"""

from __future__ import annotations

import json
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Iterable

# ──────────────────────────────────────────── #
# Paths
# ──────────────────────────────────────────── #
ROOT = Path.cwd()                                   # project root
SRC  = ROOT / "src" / "{{ cookiecutter.assembly_name }}"

# ──────────────────────────────────────────── #
# Helpers
# ──────────────────────────────────────────── #
def rm(path: Path | str) -> None:
    """Idempotently delete a file or directory."""
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

def _yes(value: str | None) -> bool:
    """Return True when *value* represents an affirmative answer."""
    return (value or "").strip().lower() == "yes"

# ──────────────────────────────────────────── #
# Evaluate cookiecutter answers (all rendered as strings)
# ──────────────────────────────────────────── #
include_azure   = _yes("{{ cookiecutter.get('include_azure', '') }}")
database_is_pg  = "{{ cookiecutter.get('database', '') }}" == "PostgreSql"
include_audit   = _yes("{{ cookiecutter.get('include_audit', '') }}")
include_oauth   = _yes("{{ cookiecutter.get('include_oauth', '') }}")
aspire_deploy   = _yes("{{ cookiecutter.get('aspire_deploy', '') }}")
github_deploy   = _yes("{{ cookiecutter.get('github_deployment', '') }}")
project_path    = "{{ cookiecutter.get('project_path', '') }}"
template_path   = "{{ cookiecutter.get('template_path', '') }}"

# ──────────────────────────────────────────── #
# 1️⃣  Clean-up files we don’t need in this flavour
# ──────────────────────────────────────────── #
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
    mongo_root = ROOT / "src/{{ cookiecutter.assembly_name }}.Data.MongoDb"
    rm_each([
        mongo_root / "Extensions/MongoExtensions.cs",
        mongo_root / "Services/MongoDbService.cs",
        mongo_root / "BsonCollectionAttribute.cs",
        mongo_root.parent / ".Data.Abstractions/Services/IMongoDbService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/administration/users/user.json",
    ])
else:
    # Remove PostgreSql artefacts
    pg_root = ROOT / "src/{{ cookiecutter.assembly_name }}.Data.PostgreSql"
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
        ROOT / "src/{{ cookiecutter.assembly_name }}.Data.MongoDb/Settings.cs",
    ])

# Always remove template snippets
rm(ROOT / "src/templates")

# ──────────────────────────────────────────── #
# 2️⃣  Optional deployment helper
# ──────────────────────────────────────────── #
if aspire_deploy and include_azure and project_path:
    deploy_script = ROOT / "deployment.py"
    try:
        subprocess.run(
            [
                sys.executable,
                str(deploy_script),
                "{{ cookiecutter.get('deployment_environment', '') }}",
                "{{ cookiecutter.assembly_name }}",
                str(github_deploy).lower(),
                "{{ cookiecutter.get('database', '') }}",
                project_path,
                template_path,
            ],
            check=True,
        )
    except subprocess.CalledProcessError as exc:
        print(f"❌ Deployment helper failed: {exc}")
        sys.exit(exc.returncode)

# ──────────────────────────────────────────── #
# 3️⃣  Persist the *minimal* context for future replays
# ──────────────────────────────────────────── #
COOKIE_FILE = ROOT / ".cookiecutter.json"
if not COOKIE_FILE.exists():
    ctx: dict[str, str] = {
        # ── core answers ──
        "assembly_name": "{{ cookiecutter.assembly_name }}",
        "root_namespace": "{{ cookiecutter.root_namespace }}",
        "database": "{{ cookiecutter.get('database', '') }}",
        "database_name": "{{ cookiecutter.database_name }}",

        # ── feature toggles ──
        "include_audit": "{{ cookiecutter.get('include_audit', '') }}",
        "include_oauth": "{{ cookiecutter.get('include_oauth', '') }}",
        "include_azure": "{{ cookiecutter.get('include_azure', '') }}",
        "aspire_deploy": "{{ cookiecutter.get('aspire_deploy', '') }}",
    }

    # 🔐 OAuth extras
    if include_oauth:
        ctx.update(
            oauth_app_name="{{ cookiecutter.get('oauth_app_name', '') }}",
            oauth_audience="{{ cookiecutter.get('oauth_audience', '') }}",
            oauth_api_audience_dev="{{ cookiecutter.get('oauth_api_audience_dev', '') }}",
            oauth_api_audience_prod="{{ cookiecutter.get('oauth_api_audience_prod', '') }}",
            oauth_domain_dev="{{ cookiecutter.get('oauth_domain_dev', '') }}",
            oauth_domain_prod="{{ cookiecutter.get('oauth_domain_prod', '') }}",
        )

    # ☁️ Azure extras
    if include_azure:
        ctx.update(
            azure_tenant_id="{{ cookiecutter.get('azure_tenant_id', '') }}",
            azure_subscription_id="{{ cookiecutter.get('azure_subscription_id', '') }}",
            azure_location="{{ cookiecutter.get('azure_location', '') }}",
            azure_key_vault_staging="{{ cookiecutter.get('azure_key_vault_staging', '') }}",
            azure_key_vault_prod="{{ cookiecutter.get('azure_key_vault_prod', '') }}",
            azure_storage_connection_staging="{{ cookiecutter.get('azure_storage_connection_staging', '') }}",
            azure_container_dev="{{ cookiecutter.get('azure_container_dev', '') }}",
            azure_container_staging="{{ cookiecutter.get('azure_container_staging', '') }}",
            azure_container_prod="{{ cookiecutter.get('azure_container_prod', '') }}",
            azure_storage_account_name_dev="{{ cookiecutter.get('azure_storage_account_name_dev', '') }}",
            azure_storage_account_name_prod="{{ cookiecutter.get('azure_storage_account_name_prod', '') }}",
            azure_container_registry_server_staging="{{ cookiecutter.get('azure_container_registry_server_staging', '') }}",
            azure_container_registry_user_staging="{{ cookiecutter.get('azure_container_registry_user_staging', '') }}",
            azure_container_registry_server_prod="{{ cookiecutter.get('azure_container_registry_server_prod', '') }}",
            azure_container_registry_user_prod="{{ cookiecutter.get('azure_container_registry_user_prod', '') }}",
        )

    # 🚀 Aspire deploy extras
    if aspire_deploy:
        ctx.update(
            deployment_environment="{{ cookiecutter.get('deployment_environment', '') }}",
            github_deployment=str(github_deploy).lower(),
            project_path=project_path,
            template_path=template_path,
        )

    # ── final sweep: remove empties ──
    ctx = {k: v for k, v in ctx.items() if v not in ("", None)}

    COOKIE_FILE.write_text(json.dumps({"cookiecutter": ctx}, indent=4))
    print("✅  .cookiecutter.json written")

print("🎉 Post-gen hook completed successfully")
