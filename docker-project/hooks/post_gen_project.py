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

ROOT = Path.cwd()                               # project root
SRC  = ROOT / "src" / "{{ cookiecutter.assembly_name }}"

# --------------------------------------------------------------------------- #
# Utilities
# --------------------------------------------------------------------------- #
def rm(item: Path | str) -> None:
    p = Path(item)
    if not p.exists():
        return
    shutil.rmtree(p) if p.is_dir() else p.unlink()
    kind = "dir " if p.is_dir() else "file"
    print(f"🗑️  Removed {kind}: {p.as_posix()}")

def rm_each(paths: Iterable[Path | str]) -> None:
    for p in paths:
        rm(p)

def docker_installed() -> bool:
    try:
        subprocess.run(["docker", "--version"], capture_output=True, check=True)
        return True
    except Exception:
        return False

# --------------------------------------------------------------------------- #
# Evaluate cookiecutter answers
# --------------------------------------------------------------------------- #
include_azure   = "{{ cookiecutter.include_azure }}"   == "yes"
database_is_pg  = "{{ cookiecutter.database }}"        == "PostgreSql"
include_audit   = "{{ cookiecutter.include_audit }}"   == "yes"
include_oauth   = "{{ cookiecutter.include_oauth }}"   == "yes"

# --------------------------------------------------------------------------- #
# 1️⃣  Conditional clean-up
# --------------------------------------------------------------------------- #
if not include_azure:
    rm_each([
        SRC / "Extensions/ApplicationInsightsExtension.cs",
        SRC / "Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Api/Vault",
    ])

if database_is_pg:
    mongo_base = ROOT / "src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}"
    rm_each([
        mongo_base / "Extensions/MongoExtensions.cs",
        mongo_base / "Services/MongoDbService.cs",
        mongo_base / "BsonCollectionAttribute.cs",
        mongo_base.parent / ".Data.Abstractions/Services/IMongoDbService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/administration/users/user.json",
    ])
else:
    pg_base = ROOT / "src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}"
    rm_each([
        pg_base / "DbConnectionProvider.cs",
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

# Strip template snippets
rm(ROOT / "templates")

# --------------------------------------------------------------------------- #
# 2️⃣  Optional Docker sanity check
# --------------------------------------------------------------------------- #
if not docker_installed():
    print("⚠️  Docker does not appear to be installed or on PATH; "
          "container tasks may fail.")

# --------------------------------------------------------------------------- #
# 3️⃣  Persist context if missing (⤵️ wrapped with "cookiecutter")
# --------------------------------------------------------------------------- #
cookie_file = ROOT / ".cookiecutter.json"
if not cookie_file.exists():
    context = {
        "is_docker": "yes",
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
        # OAuth block
        "oauth_app_name": "{{ cookiecutter.oauth_app_name }}",
        "oauth_audience": "{{ cookiecutter.oauth_audience }}",
        "oauth_api_audience_dev": "{{ cookiecutter.oauth_api_audience_dev }}",
        "oauth_api_audience_prod": "{{ cookiecutter.oauth_api_audience_prod }}",
        "oauth_domain_dev": "{{ cookiecutter.oauth_domain_dev }}",
        "oauth_domain_prod": "{{ cookiecutter.oauth_domain_prod }}",
    }

    # ✅ Wrap so replay files work
    cookie_file.write_text(json.dumps({"cookiecutter": context}, indent=4))
    print("✅ .cookiecutter.json written")

print("🎉 Docker post-gen hook completed successfully")
