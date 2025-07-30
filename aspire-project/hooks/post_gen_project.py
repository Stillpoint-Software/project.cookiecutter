#!/usr/bin/env python
"""
This hook runs automatically after Cookiecutter finishes generating the project.
It captures the exact Git commit SHA of the template you used, then writes it
into the generated project's .cookiecutter.json under "template_sha" so future
template updates can update the project correctly.

 ***Aborts generation*** when the template is not a Git repo (unless running in CI).

Works both:
• Interactively on dev machines (Windows/macOS/Linux)
• Non-interactive in CI runners (e.g., GitHub Actions ubuntu-latest)
"""
from __future__ import annotations
import json, shutil, subprocess, sys, os
from pathlib import Path
from typing import Iterable

# ──────────────────────────────────────────── #
# Paths
# ──────────────────────────────────────────── #
ROOT = Path.cwd()
SRC = ROOT / "src" / "{{ cookiecutter.assembly_name }}"

# ──────────────────────────────────────────── #
# Helpers
# ──────────────────────────────────────────── #
def rm(item: Path | str) -> None:
    p = Path(item)
    if p.exists():
        shutil.rmtree(p) if p.is_dir() else p.unlink()
        print(f"🗑️  Removed {'dir' if p.is_dir() else 'file'}: {p}")

def rm_each(paths: Iterable[Path | str]) -> None:
    for p in paths:
        rm(p)

_yes = lambda s: (s or "").strip().lower() == "yes"

def find_git_root(path: Path) -> Path | None:
    for parent in [path] + list(path.parents):
        if (parent / ".git").exists():
            return parent
    return None

def read_sha(repo_root: Path) -> str | None:
    try:
        return subprocess.check_output(["git", "-C", str(repo_root), "rev-parse", "HEAD"], text=True).strip()
    except subprocess.CalledProcessError:
        return None

# ──────────────────────────────────────────── #
# 1️⃣ Answers
# ──────────────────────────────────────────── #
include_azure_key_vault            = _yes("{{ cookiecutter.include_azure_key_vault }}")
include_azure_application_insights = _yes("{{ cookiecutter.include_azure_application_insights }}")
include_azure_storage              = _yes("{{ cookiecutter.include_azure_storage }}")
include_azure_service_bus          = _yes("{{ cookiecutter.include_azure_service_bus }}")
database_is_pg                     = "{{ cookiecutter.database }}" == "PostgreSql"
include_audit                      = _yes("{{ cookiecutter.include_audit }}")
include_oauth                      = _yes("{{ cookiecutter.include_oauth }}")
project_path                       = "{{ cookiecutter.project_path }}"
template_path                      = "{{ cookiecutter.template_path }}"
deployment_environment             = "{{ cookiecutter.deployment_environment }}"
github_organization                = "{{ cookiecutter.github_organization }}"

# ──────────────────────────────────────────── #
# 2️⃣ Conditional cleanup
# ──────────────────────────────────────────── #
# Database
if not database_is_pg:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/CreateSchema.sql",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Configuration/DatabaseConfigurationPostgres.cs",
    ])
else:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/{{ cookiecutter.database_name }}",
    ])

# Azure
if not include_azure_key_vault:
    rm_each([
        SRC / ".Core/Vault",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Configuration/AzureKeyVaultConfiguration.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Production.json",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Staging.json",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Identity/CryptoRandom.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Vault"
    ])

if not include_azure_application_insights:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/ApplicationInsightsExtensions.cs",
    ])

if not include_azure_service_bus:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Commands/Middleware/CommandMiddlewareTelemetryExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Commands/Middleware/TelemetrySourceProvider.cs"
    ])

if not include_audit:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Security/SecureAttribute.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Security/SecurityHelper.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/AuditSetup.cs",
    ])

if not include_oauth:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Identity/AuthService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Configuration/CryptoRandom.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Infrastructure/Extensions/SecurityRequirementsOperationFilter.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/AuthPolicyExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Data/Abstractions/IAuthService.cs",
    ])

rm(ROOT / "templates")  # always drop template snippets

# ──────────────────────────────────────────── #
# 3️⃣ Optional deployment helper (MongoDb only)
# ──────────────────────────────────────────── #
# database_is_mongo = "{{ cookiecutter.database }}" == "MongoDb"

# if database_is_mongo :
#     try:
#         subprocess.run(
#             [
#                 sys.executable,
#                 str(ROOT / "deployment.py"),
#                  str(deployment_environment).lower(),
#                 "{{ cookiecutter.assembly_name }}",
#                 "{{ cookiecutter.database }}",
#                 project_path,
#                 template_path,
#             ],
#             check=True,
#         )
#     except subprocess.CalledProcessError as exc:
#         print(f"❌ Deployment helper failed: {exc}")
#         sys.exit(exc.returncode)

# ──────────────────────────────────────────── #
# 4️⃣ Capture template SHA – mandatory, but skip in CI
# ──────────────────────────────────────────── #
hook_path = Path(__file__).resolve()
template_sha: str | None = None

git_root = find_git_root(hook_path)
if git_root:
    template_sha = read_sha(git_root)

# fallback via _template replay path
if template_sha is None:
    template_arg = Path(r"{{ cookiecutter._template }}").expanduser()
    git_root = find_git_root(template_arg)
    if git_root:
        template_sha = read_sha(git_root)

if template_sha is None:
    if not os.environ.get("CI"):
        print("❌ Template is not a git repository – cancelling project creation.")
        sys.exit(1)
    else:
        print("⚠️  No git repo detected in CI; continuing without template SHA.")
else:
    print(f"✅ Template commit SHA: {template_sha}")
    print(f"🔎 post_gen_project.py location: {hook_path}")

# ──────────────────────────────────────────── #
# 5️⃣ Persist minimal replay context
# ──────────────────────────────────────────── #
COOKIE_FILE = ROOT / ".cookiecutter.json"
if not COOKIE_FILE.exists():
    ctx: dict[str, str] = {
        "is_aspire": "yes",
        "assembly_name": "{{ cookiecutter.assembly_name }}",
        "root_namespace": "{{ cookiecutter.root_namespace }}",
        "api_app_name": "{{ cookiecutter.api_app_name }}",
        "api_web_url": "{{ cookiecutter.api_web_url }}",
        "database": "{{ cookiecutter.database }}",
        "database_name": "{{ cookiecutter.database_name }}",
        "github_organization": github_organization,
        "deployment_environment": deployment_environment,
        "project_path": project_path,
        "template_path": template_path,
        "include_audit": "{{ cookiecutter.include_audit | default('') }}",
        "include_oauth": "{{ cookiecutter.include_oauth | default('') }}",
        "include_azure_key_vault": "{{ cookiecutter.include_azure_key_vault | default('') }}",
        "include_azure_application_insights": "{{ cookiecutter.include_azure_application_insights | default('') }}",
        "include_azure_storage": "{{ cookiecutter.include_azure_storage | default('') }}",
        "include_azure_service_bus": "{{ cookiecutter.include_azure_service_bus | default('') }}",
        "oauth_app_name": "{{ cookiecutter.oauth_app_name | default('') }}",
        "oauth_audience": "{{ cookiecutter.oauth_audience | default('') }}",
        "oauth_api_audience_dev": "{{ cookiecutter.oauth_api_audience_dev | default('') }}",
        "oauth_api_audience_prod": "{{ cookiecutter.oauth_api_audience_prod | default('') }}",
        "oauth_domain_dev": "{{ cookiecutter.oauth_domain_dev | default('') }}",
        "oauth_domain_prod":"{{ cookiecutter.oauth_domain_prod | default('') }}",
        "key_vault_name": "{{ cookiecutter.key_vault_name | default('') }}",
        "key_vault_tenant_id": "{{ cookiecutter.key_vault_tenant_id | default('') }}",
        "key_vault_client_id": "{{ cookiecutter.key_vault_client_id | default('') }}",
        "key_vault_secret_name": "{{ cookiecutter.key_vault_secret_name | default('') }}",
        "app_insights_connection_string": "{{ cookiecutter.app_insights_connection_string | default('') }}",
        "storage_account_name": "{{ cookiecutter.storage_account_name | default('') }}",
        "storage_container_name": "{{ cookiecutter.storage_container_name | default('') }}",
        "service_bus_namespace": "{{ cookiecutter.service_bus_namespace | default('') }}",
        "service_bus_topic": "{{ cookiecutter.service_bus_topic | default('') }}",
        "service_bus_subscription": "{{ cookiecutter.service_bus_subscription | default('') }}",
        "template_sha": template_sha,
    }
    ctx = {k: v for k, v in ctx.items() if v not in ("", None)}
    COOKIE_FILE.write_text(json.dumps({"cookiecutter": ctx}, indent=4))
    print("✅  .cookiecutter.json written (with cookiecutter key)")

workflow = ROOT / ".github" / "workflows" / "Project_Template_Update.yml.j2"
if workflow.exists():
    workflow.rename(workflow.with_suffix(""))  # strips .j2
    print("✅ Renamed .yml.j2 → .yml")
    
print("🎉 Aspire post-gen hook completed successfully")