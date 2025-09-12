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

def _yes(s: Optional[str]) -> bool:
    return (s or "").strip().lower() == "yes"

# ────────────────────────────────────────────
# 1) Answers
# ────────────────────────────────────────────
include_azure_key_vault            = _yes("{{ cookiecutter.include_azure_key_vault }}")
include_azure_application_insights = _yes("{{ cookiecutter.include_azure_application_insights }}")
include_azure_storage              = _yes("{{ cookiecutter.include_azure_storage }}")
include_azure_service_bus          = _yes("{{ cookiecutter.include_azure_service_bus }}")
database_is_pg                     = "{{ cookiecutter.database }}" == "PostgreSql"
include_audit                      = _yes("{{ cookiecutter.include_audit }}")
include_oauth                      = _yes("{{ cookiecutter.include_oauth }}")
deployment_environment             = "{{ cookiecutter.deployment_environment }}"
github_organization                = "{{ cookiecutter.github_organization }}"

# ────────────────────────────────────────────
# 2) Conditional cleanup (create project artifacts first)
# ────────────────────────────────────────────
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
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Configuration/AzureKeyVaultConfiguration.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Production.json",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Staging.json",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Migrations/Extensions/AzureSecretsExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Identity/CryptoRandom.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Vault",
    ])

if not include_azure_application_insights:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Extensions/ApplicationInsightsExtensions.cs",
    ])

if not include_azure_service_bus:
    rm_each([
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Commands/Middleware/CommandMiddlewareTelemetryExtensions.cs",
        ROOT / "src/{{ cookiecutter.assembly_name }}.Core/Commands/Middleware/TelemetrySourceProvider.cs",
    ])

if not (include_azure_storage and include_azure_key_vault and include_azure_application_insights and include_azure_service_bus):
    rm_each([
        ROOT / ".github/workflows/DbMigrations_Production.yml.j2",
        ROOT / ".github/workflows/DbMigrations_Staging.yml.j2",
        ROOT / ".github/workflows/ProdDeployment.yml",
        ROOT / ".github/workflows/ProdProvisioning.yml",
        ROOT / ".github/workflows/StagingDeployment.yml",
        ROOT / ".github/workflows/StagingProvisioning.yml",
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

# ────────────────────────────────────────────
# 3) Persist minimal replay context (without template_sha initially)
# ────────────────────────────────────────────
COOKIE_FILE = ROOT / ".cookiecutter.json"
if not COOKIE_FILE.exists():
    ctx: dict[str, str | None] = {
        "assembly_name": "{{ cookiecutter.assembly_name }}",
        "root_namespace": "{{ cookiecutter.root_namespace }}",
        "api_app_name": "{{ cookiecutter.api_app_name }}",
        "api_web_url": "{{ cookiecutter.api_web_url }}",
        "database": "{{ cookiecutter.database }}",
        "database_name": "{{ cookiecutter.database_name }}",
        "github_organization": github_organization,
        "deployment_environment": deployment_environment,
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
        # record where we came from; SHA added later
        "template_source": "{{ cookiecutter._template }}",
        "template_ref": "{{ cookiecutter._checkout | default('') }}",
    }
    ctx = {k: v for k, v in ctx.items() if v not in ("", None, "")}
    COOKIE_FILE.write_text(json.dumps({"cookiecutter": ctx}, indent=4))
    print("✅  .cookiecutter.json written (without template_sha)")

# ────────────────────────────────────────────
# 4) Rename workflow files (*.j2 → no suffix)
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
# 5) Initialize Git repo (always) & initial commit
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

    # Set a local identity if none exists so the commit doesn’t fail
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
# 6) AFTER the project is created: resolve template SHA
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

_template_ref = "{{ cookiecutter._template }}"
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
