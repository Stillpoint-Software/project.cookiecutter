#!/usr/bin/env python
"""
Post-gen hook for Cookiecutter

Flow:
1) Perform cleanup & write .cookiecutter.json (without template_sha initially)
2) Set user secrets from secrets-config.json (SKIP in CI)
3) Rename workflow files (*.j2 → no suffix)
4) Initialize a Git repo & create the initial commit (SKIP in CI)
5) Resolve the template commit SHA from the remote (SKIP in CI)
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
from typing import Iterable

ROOT = Path.cwd()
HOOKS_DIR = Path(__file__).parent
SECRETS_CONFIG_FILE = HOOKS_DIR / "secrets-config.json"

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


def run(cmd: list[str], cwd: Path | None = None) -> None:
    p = subprocess.run(cmd, cwd=str(cwd) if cwd else None, text=True, capture_output=True)
    if p.returncode != 0:
        print(f"❌ Command failed: {' '.join(cmd)}", file=sys.stderr)
        if p.stdout.strip():
            print(p.stdout, file=sys.stderr)
        if p.stderr.strip():
            print(p.stderr, file=sys.stderr)
        raise SystemExit(p.returncode)


def git_available() -> bool:
    return run_out(["git", "--version"])[0] == 0


def is_git_repo(path: Path) -> bool:
    return run_out(["git", "-C", str(path), "rev-parse", "--is-inside-work-tree"])[0] == 0


def env_truthy(name: str) -> bool:
    return (os.getenv(name) or "").strip().lower() in ("1", "true", "yes", "y")


def should_skip_user_secrets() -> bool:
    """Check if running in CI environment (GitHub Actions, etc.)"""
    return env_truthy("GITHUB_ACTIONS") or env_truthy("CI")


def as_bool(v) -> bool:
    """Handles bools and stringified bools (True/False, true/false, etc.)"""
    if isinstance(v, bool):
        return v
    s = str(v).strip().lower()
    return s in ("true", "1", "y", "yes")


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
# 1) Cookiecutter answers 
# ────────────────────────────────────────────
# All cookiecutter variables - Jinja2 renders these at generation time
COOKIECUTTER_CONTEXT = {
    # Core
    "assembly_name": "{{ cookiecutter.assembly_name }}",
    "root_namespace": "{{ cookiecutter.assembly_name | lower | replace(' ', '_') | replace('-', '_') }}",
    "api_app_name": "{{ cookiecutter.api_app_name }}",
    "api_web_url": "{{ cookiecutter.api_web_url }}",
    "database": "{{ cookiecutter.database }}",
    "database_name": "{{ cookiecutter.database_name | lower }}",
    "github_organization": "{{ cookiecutter.github_organization }}",
    # Feature flags
    "include_audit": "{{ cookiecutter.include_audit }}",
    "include_oauth": "{{ cookiecutter.include_oauth }}",
    "include_azure_key_vault": "{{ cookiecutter.include_azure_key_vault }}",
    "include_azure_application_insights": "{{ cookiecutter.include_azure_application_insights }}",
    "include_azure_storage": "{{ cookiecutter.include_azure_storage }}",
    "use_existing_azure_key_vault": "{{ cookiecutter.use_existing_azure_key_vault }}",
    "use_existing_azure_storage": "{{ cookiecutter.use_existing_azure_storage }}",
    # User secrets (only used locally, stored in .cookiecutter.json as empty)
    "oauth_app_name": "{{ cookiecutter.oauth_app_name }}",
    "oauth_audience": "{{ cookiecutter.oauth_audience }}",
    "oauth_domain": "{{ cookiecutter.oauth_domain }}",
    "oauth_api_audience": "{{ cookiecutter.oauth_api_audience }}",
    "oauth_api_client_secret": "{{ cookiecutter.oauth_api_client_secret }}",
    "oauth_api_client_id": "{{ cookiecutter.oauth_api_client_id }}",
    "oauth_swagger_client_id": "{{ cookiecutter.oauth_swagger_client_id }}",
    "oauth_swagger_client_secret": "{{ cookiecutter.oauth_swagger_client_secret }}",
    "azure_resource_group": "{{ cookiecutter.azure_resource_group }}",
    "azure_tenant_id": "{{ cookiecutter.azure_tenant_id }}",
    "azure_subscription_id": "{{ cookiecutter.azure_subscription_id }}",
    "azure_location": "{{ cookiecutter.azure_location }}",
    "azure_application_insights_name": "{{ cookiecutter.azure_application_insights_name }}",
    "azure_application_insights_connection": "{{ cookiecutter.azure_application_insights_connection }}",
    "azure_key_vault_name": "{{ cookiecutter.azure_key_vault_name }}",
    "azure_key_vault_connection": "{{ cookiecutter.azure_key_vault_connection }}",
    "azure_storage_account_name": "{{ cookiecutter.azure_storage_account_name }}",
    "azure_storage_connection": "{{ cookiecutter.azure_storage_connection }}",
    "azure_storage_blob_name": "{{ cookiecutter.azure_storage_blob_name }}",
    # Template source
    "_template": "{{ cookiecutter._template }}",
    "_checkout": "{{ cookiecutter._checkout | default('') }}",
}

# Convenience accessors
assembly_name = COOKIECUTTER_CONTEXT["assembly_name"]
database_is_pg = COOKIECUTTER_CONTEXT["database"] == "PostgreSql"
include_azure_key_vault = as_bool(COOKIECUTTER_CONTEXT["include_azure_key_vault"])
include_azure_application_insights = as_bool(COOKIECUTTER_CONTEXT["include_azure_application_insights"])
include_azure_storage = as_bool(COOKIECUTTER_CONTEXT["include_azure_storage"])
include_audit = as_bool(COOKIECUTTER_CONTEXT["include_audit"])
include_oauth = as_bool(COOKIECUTTER_CONTEXT["include_oauth"])
use_existing_azure_key_vault = as_bool(COOKIECUTTER_CONTEXT["use_existing_azure_key_vault"])
use_existing_azure_storage = as_bool(COOKIECUTTER_CONTEXT["use_existing_azure_storage"])


def get_context_value(key: str) -> str:
    """Get a value from the cookiecutter context."""
    return COOKIECUTTER_CONTEXT.get(key, "")


def get_context_bool(key: str) -> bool:
    """Get a boolean value from the cookiecutter context."""
    return as_bool(COOKIECUTTER_CONTEXT.get(key, "false"))


# ────────────────────────────────────────────
# 2) User secrets from secrets-config.json
# ────────────────────────────────────────────
def evaluate_condition(condition) -> bool:
    """
    Evaluate a condition from secrets-config.json.
    
    - Single string: "include_oauth" → check if that flag is true
    - List (AND): ["include_azure_key_vault", "use_existing_azure_key_vault"] → all must be true
    """
    if isinstance(condition, str):
        return get_context_bool(condition)
    elif isinstance(condition, list):
        # AND condition - all must be true
        return all(get_context_bool(c) for c in condition)
    return False


def evaluate_condition_any(conditions: list) -> bool:
    """
    Evaluate a condition_any from secrets-config.json.
    
    List of conditions where ANY can be true (OR logic).
    Each item in the list is itself a condition (AND logic within).
    """
    return any(evaluate_condition(c) for c in conditions)


def load_secrets_config() -> dict:
    """Load the secrets configuration from JSON file."""
    if not SECRETS_CONFIG_FILE.exists():
        print(f"⚠️  Secrets config not found: {SECRETS_CONFIG_FILE}")
        return {}
    
    try:
        return json.loads(SECRETS_CONFIG_FILE.read_text())
    except Exception as e:
        print(f"⚠️  Failed to load secrets config: {e}")
        return {}


def set_user_secrets(apphost_csproj: Path) -> None:
    """Set user secrets based on secrets-config.json."""
    config = load_secrets_config()
    if not config:
        return
    
    print("🔐 Setting user secrets...")
    
    for section_name, section in config.items():
        # Check if this section's condition is met
        should_apply = False
        
        if "condition" in section:
            should_apply = evaluate_condition(section["condition"])
        elif "condition_any" in section:
            should_apply = evaluate_condition_any(section["condition_any"])
        else:
            # No condition means always apply
            should_apply = True
        
        if not should_apply:
            continue
        
        print(f"  📦 {section_name}")
        
        secrets = section.get("secrets", {})
        for secret_key, context_key in secrets.items():
            value = get_context_value(context_key)
            if value and value.strip():
                run([
                    "dotnet", "user-secrets", "set", secret_key, value,
                    "--project", str(apphost_csproj)
                ])
            else:
                print(f"    ⚠️  Skipping {secret_key} (empty value)")


def require_secrets_non_empty() -> None:
    """Validate that required secrets have values based on enabled features."""
    config = load_secrets_config()
    if not config:
        return
    
    errors = []
    
    for section_name, section in config.items():
        # Check if this section's condition is met
        should_validate = False
        
        if "condition" in section:
            should_validate = evaluate_condition(section["condition"])
        elif "condition_any" in section:
            should_validate = evaluate_condition_any(section["condition_any"])
        
        if not should_validate:
            continue
        
        secrets = section.get("secrets", {})
        for secret_key, context_key in secrets.items():
            value = get_context_value(context_key)
            if not value or not value.strip():
                errors.append(f"{context_key} (required for {section_name})")
    
    if errors:
        print("❌ Missing required values:", file=sys.stderr)
        for err in errors:
            print(f"   - {err}", file=sys.stderr)
        raise SystemExit(1)


# ────────────────────────────────────────────
# 3) AppHost user-secrets (skip in CI)
# ────────────────────────────────────────────
apphost_csproj = ROOT / f"src/{assembly_name}.AppHost" / f"{assembly_name}.AppHost.csproj"

print(f"ℹ️  AppHost project path: {apphost_csproj}")

if not apphost_csproj.exists():
    print(f"⚠️  AppHost project not found: {apphost_csproj}")

if should_skip_user_secrets():
    print("ℹ️  Running in CI environment - skipping user secrets")
else:
    # Validate required secrets
    require_secrets_non_empty()
    
    if apphost_csproj.exists():
        print("⏳ Please wait... initializing user-secrets")
        run(["dotnet", "user-secrets", "init", "--project", str(apphost_csproj)])
        set_user_secrets(apphost_csproj)


# ────────────────────────────────────────────
# 4) Conditional cleanup
# ────────────────────────────────────────────
# Database
if not database_is_pg:
    rm_each([
        ROOT / f"src/{assembly_name}.Migrations/Resources/1000-Initial/CreateSchema.sql",
        ROOT / f"src/{assembly_name}.Core/Configuration/DatabaseConfigurationPostgres.cs",
    ])
else:
    rm_each([
        ROOT / f"src/{assembly_name}.Migrations/Resources/1000-Initial/{COOKIECUTTER_CONTEXT['database_name']}",
    ])

# Azure Key Vault
if not include_azure_key_vault:
    rm_each([
        ROOT / f"src/{assembly_name}.Core/Extensions/AzureSecretsExtensions.cs",
        ROOT / f"src/{assembly_name}.Core/Configuration/AzureKeyVaultConfiguration.cs",
        ROOT / f"src/{assembly_name}.Migrations/appsettings.Production.json",
        ROOT / f"src/{assembly_name}.Migrations/appsettings.Staging.json",
        ROOT / f"src/{assembly_name}.Migrations/Extensions/AzureSecretsExtensions.cs",
        ROOT / f"src/{assembly_name}.Core/Vault",
        ROOT / f"src/{assembly_name}.AppHost/Extensions/KeyVaultExtensions.cs",
    ])

# Azure Application Insights
if not include_azure_application_insights:
    rm_each([
        ROOT / f"src/{assembly_name}.Core/Extensions/ApplicationInsightsExtensions.cs",
        ROOT / f"src/{assembly_name}.AppHost/Extensions/ApplicationInsightExtensions.cs",
    ])

# Audit
if not include_audit:
    rm_each([
        ROOT / f"src/{assembly_name}.Core/Security/SecureAttribute.cs",
        ROOT / f"src/{assembly_name}.Core/Security/SecurityHelper.cs",
        ROOT / f"src/{assembly_name}.Infrastructure/Configuration/AuditSetup.cs",
        ROOT / f"src/{assembly_name}.Infrastructure/Configuration/ListAuditEvent.cs",
        ROOT / f"src/{assembly_name}.Infrastructure/Configuration/ListAuditModel.cs",
    ])

# OAuth
if not include_oauth:
    rm_each([
        ROOT / f"src/{assembly_name}.Core/Configuration/CryptoRandom.cs",
        ROOT / f"src/{assembly_name}.Core/Extensions/AuthPolicyExtensions.cs",
        ROOT / f"src/{assembly_name}.Core/Identity/AuthService.cs",
        ROOT / f"src/{assembly_name}.Core/Identity/CryptoRandom.cs",
        ROOT / f"src/{assembly_name}.Infrastructure/Configuration/AuthService.cs",
        ROOT / f"src/{assembly_name}.Data/Abstractions/IAuthService.cs"
    ])

# Azure Storage
if not include_azure_storage:
    rm_each([
        ROOT / f"src/{assembly_name}.AppHost/Extensions/StorageExtensions.cs",
    ])


# ────────────────────────────────────────────
# 5) Persist minimal replay context (without template_sha)
# ────────────────────────────────────────────
COOKIE_FILE = ROOT / ".cookiecutter.json"

# Prune deprecated keys if the template provides them
try:
    import importlib.util
    prune_script = HOOKS_DIR / "prune_cookiecutter_json.py"
    if prune_script.exists():
        spec = importlib.util.spec_from_file_location("prune_cookiecutter_json", prune_script)
        prune_module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(prune_module)
        
        dep_file = HOOKS_DIR / "deprecated.json"
        if dep_file.exists() and COOKIE_FILE.exists():
            keys = prune_module.load_deprecation_keys(dep_file)
            removed = prune_module.prune_cookiecutter_file(COOKIE_FILE, keys)
            if removed:
                print(f"🧹 Pruned deprecated keys from .cookiecutter.json: {', '.join(removed)}")
except Exception as e:
    print(f"⚠️  Deprecations prune skipped: {e}")

if not COOKIE_FILE.exists():
    BLANK_FOR_REPLAY_KEYS = {
        "oauth_app_name",
        "oauth_audience",
        "oauth_domain",
        "oauth_api_audience",
        "oauth_api_client_id",
        "oauth_swagger_client_id",
        "azure_resource_group",
        "azure_tenant_id",
        "azure_subscription_id",
        "azure_location",
        "azure_application_insights_name",
        "azure_key_vault_name",
        "azure_storage_account_name",
        "azure_storage_blob_name",
    }

    ctx = {
        # Core
        "assembly_name": COOKIECUTTER_CONTEXT["assembly_name"],
        "root_namespace": COOKIECUTTER_CONTEXT["root_namespace"],
        "api_app_name": COOKIECUTTER_CONTEXT["api_app_name"],
        "api_web_url": COOKIECUTTER_CONTEXT["api_web_url"],
        "database": COOKIECUTTER_CONTEXT["database"],
        "database_name": COOKIECUTTER_CONTEXT["database_name"],
        "github_organization": COOKIECUTTER_CONTEXT["github_organization"],

        # Feature flags (REAL bools)
        "include_audit": get_context_bool("include_audit"),
        "include_oauth": get_context_bool("include_oauth"),
        "include_azure_key_vault": get_context_bool("include_azure_key_vault"),
        "include_azure_application_insights": get_context_bool("include_azure_application_insights"),
        "include_azure_storage": get_context_bool("include_azure_storage"),
        "use_existing_azure_key_vault": get_context_bool("use_existing_azure_key_vault"),
        "use_existing_azure_storage": get_context_bool("use_existing_azure_storage"),
    }

    # Add “blank but present” keys for replay (intentionally blank)
    for k in BLANK_FOR_REPLAY_KEYS:
        ctx[k] = ""

    template_source = COOKIECUTTER_CONTEXT["_template"]
    if template_source and str(template_source).startswith(("gh:", "git@", "http://", "https://")):
        ctx["template_source"] = template_source

    template_ref = COOKIECUTTER_CONTEXT["_checkout"]
    if template_ref and str(template_ref).strip() and str(template_ref).lower() not in ("none", ""):
        ctx["template_ref"] = template_ref

    # Drop None and empty strings EXCEPT for keys we intentionally keep blank
    def keep_item(k, v) -> bool:
        if v is None:
            return False
        if isinstance(v, str) and v.strip() == "":
            return k in BLANK_FOR_REPLAY_KEYS
        return True

    ctx = {k: v for k, v in ctx.items() if keep_item(k, v)}

    COOKIE_FILE.write_text(json.dumps({"cookiecutter": ctx}, indent=4))
    print("✅  .cookiecutter.json written (without template_sha)")


# ────────────────────────────────────────────
# 6) Rename workflow files (*.j2 → no suffix)
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
# 7) Initialize Git repo (SKIP in CI)
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

    # Set a local identity if none exists
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


if not should_skip_user_secrets():
    ensure_project_git(ROOT)
else:
    print("ℹ️  Skipping git initialization (CI environment)")


# ────────────────────────────────────────────
# 8) Resolve template SHA (SKIP in CI)
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


def resolve_template_sha(template_ref: str, checkout: str | None) -> str | None:
    if not git_available():
        return None

    if checkout and _SHA_RE.fullmatch(checkout):
        return checkout.lower()

    # Check if template_ref is a local git repo
    local_path = Path(template_ref)
    if local_path.exists() and is_git_repo(local_path):
        code, sha, _ = run_out(["git", "rev-parse", "HEAD"], cwd=local_path)
        if code == 0 and sha and _SHA_RE.fullmatch(sha):
            return sha.lower()

    # Fall back to remote URL logic
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


if not should_skip_user_secrets():
    _template_ref = COOKIECUTTER_CONTEXT["_template"]
    _checkout_ref = COOKIECUTTER_CONTEXT["_checkout"].strip() or None
    template_sha = resolve_template_sha(_template_ref, _checkout_ref)

    if template_sha:
        print(f"✅ Template commit SHA: {template_sha}")
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
else:
    print("ℹ️  Skipping template SHA resolution (CI environment)")

print("🎉 Post-gen hook completed")