#!/usr/bin/env python
"""
Post-gen hook for Cookiecutter

Flow:
1) Perform cleanup & write .cookiecutter.json (without template_sha initially)
2) Rename workflow files (*.j2 → no suffix)
3) ALWAYS initialize a Git repo & create the initial commit
4) THEN resolve the template commit SHA from the remote
   - If found: write it into .cookiecutter.json and commit that change
   - If missing: print a notice that the template SHA needs to be added
"""

from __future__ import annotations

import json
import os
import re
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Iterable, Optional

ROOT = Path.cwd()

# ────────────────────────────────────────────
# Shell helpers
# ────────────────────────────────────────────
def run_out(args: list[str], cwd: Path | None = None) -> tuple[int, str, str]:
    try:
        cp = subprocess.run(
            args,
            cwd=str(cwd) if cwd else None,
            check=False,
            capture_output=True,
            text=True,
        )
        return cp.returncode, (cp.stdout or "").strip(), (cp.stderr or "").strip()
    except Exception as e:
        return 1, "", str(e)

def git_available() -> bool:
    return run_out(["git", "--version"])[0] == 0

def is_git_repo(path: Path) -> bool:
    return run_out(["git", "-C", str(path), "rev-parse", "--is-inside-work-tree"])[0] == 0

def require_non_empty(name: str, value: str | None) -> None:
    if value is not None and str(value).strip() != "":
        return

    print(f"❌ Cookiecutter value '{name}' is required.", file=sys.stderr)
    raise SystemExit(1)

def should_skip_user_secrets() -> bool:
    def env_truthy(name: str) -> bool:
        return (os.getenv(name) or "").strip().lower() in ("1", "true", "yes", "y")

    return env_truthy("GITHUB_ACTIONS") or env_truthy("CI") 

def as_bool(v) -> bool:
    # Handles bools and stringified bools (True/False, true/false, etc.)
    if isinstance(v, bool):
        return v
    s = str(v).strip().lower()
    return s in ("true", "1", "y", "yes")


def run(cmd: list[str], cwd: Path | None = None) -> None:
    p = subprocess.run(cmd, cwd=str(cwd) if cwd else None, text=True, capture_output=True)
    if p.returncode != 0:
        print(f"❌ Command failed: {' '.join(cmd)}", file=sys.stderr)
        if p.stdout.strip():
            print(p.stdout, file=sys.stderr)
        if p.stderr.strip():
            print(p.stderr, file=sys.stderr)
        raise SystemExit(p.returncode)

# ────────────────────────────────────────────
# Filesystem helpers
# ────────────────────────────────────────────
def rm(item: Path | str) -> None:
    p = Path(item)
    try:
        if p.exists():
            if p.is_dir():
                shutil.rmtree(p, ignore_errors=True)
                print(f"🗑️  Removed dir:  {p}")
            else:
                try:
                    p.unlink()
                except FileNotFoundError:
                    pass
                print(f"🗑️  Removed file: {p}")
    except Exception as e:
        print(f"⚠️  Failed to remove {p}: {e}")

def rm_each(paths: Iterable[Path | str]) -> None:
    for p in paths:
        rm(p)

# ────────────────────────────────────────────
# 1) Answers
# ────────────────────────────────────────────
assembly_name = "{{ cookiecutter.assembly_name }}"
database_is_pg                     = "{{ cookiecutter.database }}" == "PostgreSql"
github_organization                = "{{ cookiecutter.github_organization }}"
include_azure_key_vault            = as_bool("{{ cookiecutter.include_azure_key_vault }}")
include_azure_application_insights = as_bool("{{ cookiecutter.include_azure_application_insights }}")
include_azure_storage              = as_bool("{{ cookiecutter.include_azure_storage }}")
include_audit                      = as_bool("{{ cookiecutter.include_audit }}")
include_oauth                      = as_bool("{{ cookiecutter.include_oauth }}")

oauth_app_name = "{{ cookiecutter.oauth_app_name }}"
oauth_audience = "{{ cookiecutter.oauth_audience }}"
oauth_domain = "{{ cookiecutter.oauth_domain }}"
oauth_api_audience = "{{ cookiecutter.oauth_api_audience }}"
oauth_api_client_secret = "{{ cookiecutter.oauth_api_client_secret }}"
oauth_api_client_id = "{{ cookiecutter.oauth_api_client_id }}"
oauth_swagger_client_id = "{{ cookiecutter.oauth_swagger_client_id }}"
oauth_swagger_client_secret = "{{ cookiecutter.oauth_swagger_client_secret }}"

azure_resource_group = "{{ cookiecutter.azure_resource_group }}"
azure_tenant_id = "{{ cookiecutter.azure_tenant_id }}"
azure_subscription_id = "{{ cookiecutter.azure_subscription_id }}"
azure_location = "{{ cookiecutter.azure_location }}"
azure_application_insights_name = "{{ cookiecutter.azure_application_insights_name }}"
azure_application_insights_connection = "{{ cookiecutter.azure_application_insights_connection }}"
use_existing_azure_key_vault = as_bool("{{ cookiecutter.use_existing_azure_key_vault }}")
azure_key_vault_name  = "{{ cookiecutter.azure_key_vault_name }}"
azure_key_vault_connection = "{{ cookiecutter.azure_key_vault_connection }}"
use_existing_azure_storage = as_bool("{{ cookiecutter.use_existing_azure_storage }}")
azure_storage_account_name = "{{ cookiecutter.azure_storage_account_name }}"
azure_storage_connection = "{{ cookiecutter.azure_storage_connection }}"
azure_storage_blob_name = "{{ cookiecutter.azure_storage_blob_name }}"

# ────────────────────────────────────────────
# Check for required variables
# ────────────────────────────────────────────
if include_oauth and not should_skip_user_secrets():
    # Make these required when OAuth is enabled
    require_non_empty("oauth_app_name", oauth_app_name)
    require_non_empty("oauth_audience", oauth_audience)
    require_non_empty("oauth_domain", oauth_domain)

    require_non_empty("oauth_api_audience", oauth_api_audience)
    require_non_empty("oauth_api_client_id", oauth_api_client_id)
    require_non_empty("oauth_api_client_secret", oauth_api_client_secret)

    require_non_empty("oauth_swagger_client_id", oauth_swagger_client_id)
    require_non_empty("oauth_swagger_client_secret", oauth_swagger_client_secret)
    
if include_azure_application_insights and not should_skip_user_secrets():
    require_non_empty("azure_application_insights_name", azure_application_insights_name)
    require_non_empty("azure_application_insights_connection", azure_application_insights_connection)

if include_azure_key_vault and use_existing_azure_key_vault and not should_skip_user_secrets():
    require_non_empty("azure_key_vault_name", azure_key_vault_name)
    require_non_empty("azure_key_vault_connection", azure_key_vault_connection)    
    
# need blob name always if storage is included
if include_azure_storage and use_existing_azure_storage and not should_skip_user_secrets():
    require_non_empty("azure_storage_account_name", azure_storage_account_name)
    require_non_empty("azure_storage_connection", azure_storage_connection) 
    
if include_azure_storage:
    require_non_empty("azure_storage_blob_name", azure_storage_blob_name)    

if ((include_azure_key_vault and use_existing_azure_key_vault) or include_azure_application_insights or (include_azure_storage and use_existing_azure_storage)) and not should_skip_user_secrets():
    require_non_empty("azure_tenant_id", azure_tenant_id)
    require_non_empty("azure_subscription_id", azure_subscription_id)
    require_non_empty("azure_location", azure_location)
    require_non_empty("azure_resource_group", azure_resource_group)
    
# ──────────────────────────────────────────────────
# AppHost user-secrets (skip in CI/template update)
# ──────────────────────────────────────────────────
# Get the apphost project path
apphost_csproj = Path.cwd() / f"src/{assembly_name}.AppHost" / f"{assembly_name}.AppHost.csproj"

print(f"ℹ️  AppHost project path: {apphost_csproj}")

if not apphost_csproj.exists():
     print(f"⚠️  AppHost project not found: {apphost_csproj}")
     
if apphost_csproj.exists() and not should_skip_user_secrets():
    print(f"⚠️  Please wait. . . initializing user-secrets")
    
    run(["dotnet", "user-secrets", "init", "--project", str(apphost_csproj)])      
          
    if include_oauth:
        run([
            "dotnet", "user-secrets", "set", "OAuth:AppName", oauth_app_name,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "OAuth:Audience", oauth_audience,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "OAuth:Domain", oauth_domain,
            "--project", str(apphost_csproj)
        ])
        run([
                "dotnet", "user-secrets", "set", "OAuth:Api:Audience", oauth_api_audience,
                "--project", str(apphost_csproj)
            ])
        run([
            "dotnet", "user-secrets", "set", "OAuth:Api:ClientSecret", oauth_api_client_secret,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "OAuth:Api:ClientId", oauth_api_client_id,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "OAuth:Swagger:ClientId", oauth_swagger_client_id,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "OAuth:Swagger:ClientSecret", oauth_swagger_client_secret,
            "--project", str(apphost_csproj)
        ])    
    if include_azure_application_insights:
        run([
            "dotnet", "user-secrets", "set", "Parameters:AppInsightsName", azure_application_insights_name,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "Parameters:AppInsightsConnection", azure_application_insights_connection,
            "--project", str(apphost_csproj)
        ])
    if include_azure_key_vault and use_existing_azure_key_vault:
        run([
            "dotnet", "user-secrets", "set", "Parameters:KeyVaultName", azure_key_vault_name,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "Parameters:KeyVaultConnection", azure_key_vault_connection,
            "--project", str(apphost_csproj)
        ])
    if include_azure_storage and use_existing_azure_storage:
        run([
            "dotnet", "user-secrets", "set", "Parameters:StorageAccountName", azure_storage_account_name,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "Parameters:StorageConnection", azure_storage_connection,
            "--project", str(apphost_csproj)
        ])
    if include_azure_storage:
        run([
        "dotnet", "user-secrets", "set", "Parameters:BlobName", azure_storage_blob_name,
        "--project", str(apphost_csproj)
    ])                    
    if (include_azure_key_vault and use_existing_azure_key_vault) or include_azure_application_insights or (include_azure_storage and use_existing_azure_storage):
        run([
            "dotnet", "user-secrets", "set", "Azure:TenantId", azure_tenant_id,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "Azure:SubscriptionId", azure_subscription_id,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "Azure:Location", azure_location,
            "--project", str(apphost_csproj)
        ])
        run([
            "dotnet", "user-secrets", "set", "Parameters:ResourceGroup", azure_resource_group,
            "--project", str(apphost_csproj)
        ])

# ────────────────────────────────────────────────────────
# Conditional cleanup (create project artifacts first)
# ────────────────────────────────────────────────────────
# Database
if not database_is_pg:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/CreateSchema.sql",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Configuration/DatabaseConfigurationPostgres.cs",
    ])
else:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/{{ cookiecutter.database_name  }}",
    ])

# Azure
if not include_azure_key_vault:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Configuration/AzureKeyVaultConfiguration.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Production.json",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Staging.json",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Vault",
        ROOT / "src/{{ cookiecutter.assembly_name }}.AppHost/Extensions/KeyVaultExtensions.cs",
    ])

if not include_azure_application_insights:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/ApplicationInsightsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.AppHost/Extensions/ApplicationInsightExtensions.cs",
    ])

if not include_audit:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Security/SecureAttribute.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Security/SecurityHelper.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/AuditSetup.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/ListAuditEvent.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/ListAuditModel.cs",
    ])

if not include_oauth:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Configuration/CryptoRandom.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/AuthPolicyExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Identity/AuthService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Identity/CryptoRandom.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/AuthService.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Data/Abstractions/IAuthService.cs"
    ])

if not include_azure_storage:    
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.AppHost/Extensions/StorageExtensions.cs",
    ])

# ────────────────────────────────────────────
# Persist minimal replay context (without template_sha initially)
# ────────────────────────────────────────────
COOKIE_FILE = ROOT / ".cookiecutter.json"

if not COOKIE_FILE.exists():
    template_source = {{ cookiecutter._template | tojson }}
    template_checkout = {{ cookiecutter._checkout | default('', true) | tojson }}    
       
    ctx: dict[str, str | None] = {
        "assembly_name": "{{ cookiecutter.assembly_name }}",
        "root_namespace": "{{ cookiecutter.assembly_name | lower | replace(' ', '_') | replace('-', '_') }}",
        "api_app_name": "{{ cookiecutter.api_app_name }}",
        "api_web_url": "{{ cookiecutter.api_web_url }}",
        "database": "{{ cookiecutter.assembly_name | lower | replace(' ', '_') | replace('-', '_') }}",
        "database_name": "{{ cookiecutter.database_name | lower}}",
        "github_organization": github_organization,
        "include_audit": "{{ cookiecutter.include_audit | default(false) }}",
        "include_oauth": "{{ cookiecutter.include_oauth | default(false) }}",
        "include_azure_key_vault": "{{ cookiecutter.include_azure_key_vault | default(false) }}",
        "include_azure_application_insights": "{{ cookiecutter.include_azure_application_insights | default(false) }}",
        "include_azure_storage": "{{ cookiecutter.include_azure_storage | default(false)  }}",
        # Template source
        "template_source": template_source,
        "template_ref": template_checkout
    }
    
    ctx = {k: v for k, v in ctx.items() if v not in ("", None)}
    COOKIE_FILE.write_text(json.dumps({"cookiecutter": ctx}, indent=4))
    print("✅  .cookiecutter.json written (without template_sha)")

# ────────────────────────────────────────────
# Rename workflow files (*.j2 → no suffix)
# ────────────────────────────────────────────
workflows_dir = ROOT / ".github" / "workflows" 
if workflows_dir.exists():
    for jf in workflows_dir.rglob("*.j2"):
        target = jf.with_suffix("")  # drop only .j2
        if target.exists():
            print(f"⚠️  Skipped {jf.relative_to(ROOT)} (already exists: {target.name})")
            continue
        try:
            jf.rename(target)
            print(f"✅ Renamed {jf.relative_to(ROOT)} → {target.relative_to(ROOT)}")
        except Exception as e:
            print(f"⚠️  Failed to rename {jf}: {e}")

# ────────────────────────────────────────────
#  Initialize Git repo (always) & initial commit
# ────────────────────────────────────────────
def ensure_project_git(project_dir: Path, branch: str = "main") -> None:
    if not git_available():
        print("⚠️  Git not found on PATH; skipping repo init.")
        return
    if is_git_repo(project_dir):
        print("ℹ️  Git repo already present; skipping init.")
        return

    print("🔧 Initializing git repository…")
    code, _, _ = run_out(["git", "init", "-b", branch], cwd=project_dir)
    if code != 0:
        run_out(["git", "init"], cwd=project_dir)
        run_out(["git", "branch", "-M", branch], cwd=project_dir)

    # Set a local identity if none exists so the commit doesn't fail
    _, name, _ = run_out(["git", "config", "--get", "user.name"], cwd=project_dir)
    _, email, _ = run_out(["git", "config", "--get", "user.email"], cwd=project_dir)
    if not name:
        run_out(["git", "config", "user.name", "Scaffolder"], cwd=project_dir)
    if not email:
        run_out(["git", "config", "user.email", "scaffolder@example.com"], cwd=project_dir)

    run_out(["git", "add", "-A"], cwd=project_dir)
    code, _, err = run_out(["git", "commit", "-m", "chore(scaffold): initial commit from cookiecutter"], cwd=project_dir)
    if code == 0:
        print("✅  Git repo initialized and initial commit created.")
    else:
        print(f"⚠️  Git commit failed (continuing): {err}")

ensure_project_git(ROOT)

# ────────────────────────────────────────────
# AFTER the project is created: resolve template SHA
# ────────────────────────────────────────────
_SHA_RE = re.compile(r"^[0-9a-f]{7,40}$", re.I)

def to_git_url(template_ref: str) -> str | None:
    s = (template_ref or "").strip()
    if not s:
        return None
    if s.startswith("gh:"):
        owner_repo = s[3:].strip("/")
        return f"https://github.com/{owner_repo}.git"
    if s.startswith(("git@", "http://", "https://", "git+http://", "git+https://")):
        return s.replace("git+https://", "https://").replace("git+http://", "http://")
    return None

def resolve_template_sha_from_remote(template_ref: str, checkout: str | None) -> str | None:
    if not git_available():
        return None

    if checkout and _SHA_RE.fullmatch(checkout):
        return checkout.lower()

    git_url = to_git_url(template_ref)
    if not git_url:
        return None

    targets: list[str] = []
    if checkout:
        targets += [checkout, f"refs/heads/{checkout}", f"refs/tags/{checkout}"]
    else:
        targets.append("HEAD")

    for tgt in targets:
        code, out, _ = run_out(["git", "ls-remote", git_url, tgt])
        if code == 0 and out:
            first = out.splitlines()[0].split()
            if first:
                sha = first[0]
                if _SHA_RE.fullmatch(sha):
                    return sha.lower()
    return None


template_source = {{ cookiecutter._template | tojson }}

_template_ref = template_source
_checkout_ref = "{{ cookiecutter._checkout | default('') }}".strip() or None
template_sha = resolve_template_sha_from_remote(_template_ref, _checkout_ref)

if template_sha:
    print(f"✅ Template commit SHA: {template_sha}")
    # Update .cookiecutter.json with the SHA and commit that change
    try:
        data = json.loads(COOKIE_FILE.read_text())
        data.setdefault("cookiecutter", {})["template_sha"] = template_sha
        COOKIE_FILE.write_text(json.dumps(data, indent=4))
        print("📝  Updated .cookiecutter.json with template_sha")

        if git_available() and is_git_repo(ROOT):
            run_out(["git", "add", str(COOKIE_FILE)], cwd=ROOT)
            short = template_sha[:12]
            run_out(["git", "commit", "-m", f"chore(scaffold): record template SHA {short}"], cwd=ROOT)
            print("✅  Committed template_sha to git.")
    except Exception as e:
        print(f"⚠️  Could not update/commit template_sha: {e}")
else:
    print("⚠️  Template SHA not detected. Please add it to .cookiecutter.json as `template_sha`,")
    print("    or re-run Cookiecutter with `--checkout <branch|tag|commit>` for determinism.")

print("🎉 Post-gen hook completed")
