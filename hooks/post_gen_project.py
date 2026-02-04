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
from typing import Any

# ════════════════════════════════════════════════════════════════════════════════
# CONFIGURATION
# ════════════════════════════════════════════════════════════════════════════════

ROOT = Path.cwd()
HOOKS_DIR = Path(__file__).parent
SECRETS_CONFIG_FILE = HOOKS_DIR / "secrets-config.json"

# ────────────────────────────────────────────────────────────────────────────────
# Cookiecutter context values
# ────────────────────────────────────────────────────────────────────────────────
CONTEXT = {
    "assembly_name": "{{ cookiecutter.assembly_name }}",
    "database": "{{ cookiecutter.database }}",
    "database_name": "{{ cookiecutter.database_name }}",
    "github_organization": "{{ cookiecutter.github_organization }}",
    "api_app_name": "{{ cookiecutter.api_app_name }}",
    "api_web_url": "{{ cookiecutter.api_web_url }}",
    "include_azure_key_vault": "{{ cookiecutter.include_azure_key_vault }}",
    "include_azure_application_insights": "{{ cookiecutter.include_azure_application_insights }}",
    "include_azure_storage": "{{ cookiecutter.include_azure_storage }}",
    "include_audit": "{{ cookiecutter.include_audit }}",
    "include_oauth": "{{ cookiecutter.include_oauth }}",
    "use_existing_azure_key_vault": "{{ cookiecutter.use_existing_azure_key_vault }}",
    "use_existing_azure_storage": "{{ cookiecutter.use_existing_azure_storage }}",
    "_template": "{{ cookiecutter._template }}",
    "_checkout": "{{ cookiecutter._checkout | default('') }}",
    # Secrets (only used when not skipping)
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
}

# ────────────────────────────────────────────────────────────────────────────────
# Validation rules: declarative schema for required fields
# ────────────────────────────────────────────────────────────────────────────────
VALIDATION_RULES = [
    # OAuth (secrets - skip during template update)
    {"field": "oauth_app_name", "required_when": ["include_oauth"], "is_secret": True},
    {"field": "oauth_audience", "required_when": ["include_oauth"], "is_secret": True},
    {"field": "oauth_domain", "required_when": ["include_oauth"], "is_secret": True},
    {"field": "oauth_api_audience", "required_when": ["include_oauth"], "is_secret": True},
    {"field": "oauth_api_client_id", "required_when": ["include_oauth"], "is_secret": True},
    {"field": "oauth_api_client_secret", "required_when": ["include_oauth"], "is_secret": True},
    {"field": "oauth_swagger_client_id", "required_when": ["include_oauth"], "is_secret": True},
    {"field": "oauth_swagger_client_secret", "required_when": ["include_oauth"], "is_secret": True},
    # Azure Application Insights (secrets - skip during template update)
    {"field": "azure_application_insights_name", "required_when": ["include_azure_application_insights"], "is_secret": True},
    {"field": "azure_application_insights_connection", "required_when": ["include_azure_application_insights"], "is_secret": True},
    # Azure Key Vault (secrets - skip during template update)
    {"field": "azure_key_vault_name", "required_when": ["include_azure_key_vault", "use_existing_azure_key_vault"], "is_secret": True},
    {"field": "azure_key_vault_connection", "required_when": ["include_azure_key_vault", "use_existing_azure_key_vault"], "is_secret": True},
    # Azure Storage (secrets - skip during template update)
    {"field": "azure_storage_account_name", "required_when": ["include_azure_storage", "use_existing_azure_storage"], "is_secret": True},
    {"field": "azure_storage_connection", "required_when": ["include_azure_storage", "use_existing_azure_storage"], "is_secret": True},
    # Azure Storage blob name (secret - skip during template update)
    {"field": "azure_storage_blob_name", "required_when": ["include_azure_storage"], "is_secret": True},
    # Azure common (secrets - skip during template update)
    {
        "field": "azure_tenant_id",
        "is_secret": True,
        "required_when_any": [
            ["include_azure_key_vault", "use_existing_azure_key_vault"],
            ["include_azure_application_insights"],
            ["include_azure_storage", "use_existing_azure_storage"],
        ],
    },
    {
        "field": "azure_subscription_id",
        "is_secret": True,
        "required_when_any": [
            ["include_azure_key_vault", "use_existing_azure_key_vault"],
            ["include_azure_application_insights"],
            ["include_azure_storage", "use_existing_azure_storage"],
        ],
    },
    {
        "field": "azure_location",
        "is_secret": True,
        "required_when_any": [
            ["include_azure_key_vault", "use_existing_azure_key_vault"],
            ["include_azure_application_insights"],
            ["include_azure_storage", "use_existing_azure_storage"],
        ],
    },
    {
        "field": "azure_resource_group",
        "is_secret": True,
        "required_when_any": [
            ["include_azure_key_vault", "use_existing_azure_key_vault"],
            ["include_azure_application_insights"],
            ["include_azure_storage", "use_existing_azure_storage"],
        ],
    },
]

# ────────────────────────────────────────────────────────────────────────────────
# Cleanup rules: files to remove based on feature flags
# ────────────────────────────────────────────────────────────────────────────────
CLEANUP_RULES = {
    "database_is_pg": {
        "remove_if_true": [
            "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/{{ cookiecutter.database_name }}",
        ],
        "remove_if_false": [
            "src/{{ cookiecutter.assembly_name }}.Migrations/Resources/1000-Initial/CreateSchema.sql",
            "src/{{ cookiecutter.assembly_name }}.Core/Configuration/DatabaseConfigurationPostgres.cs",
        ],
    },
    "include_azure_key_vault": {
        "remove_if_false": [
            "src/{{ cookiecutter.assembly_name }}.Core/Extensions/AzureSecretsExtensions.cs",
            "src/{{ cookiecutter.assembly_name }}.Core/Configuration/AzureKeyVaultConfiguration.cs",
            "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Production.json",
            "src/{{ cookiecutter.assembly_name }}.Migrations/appsettings.Staging.json",
            "src/{{ cookiecutter.assembly_name }}.Migrations/Extensions/AzureSecretsExtensions.cs",
            "src/{{ cookiecutter.assembly_name }}.Core/Vault",
            "src/{{ cookiecutter.assembly_name }}.AppHost/Extensions/KeyVaultExtensions.cs",
        ],
    },
    "include_azure_application_insights": {
        "remove_if_false": [
            "src/{{ cookiecutter.assembly_name }}.Core/Extensions/ApplicationInsightsExtensions.cs",
            "src/{{ cookiecutter.assembly_name }}.AppHost/Extensions/ApplicationInsightExtensions.cs",
        ],
    },
    "include_audit": {
        "remove_if_false": [
            "src/{{ cookiecutter.assembly_name }}.Core/Security/SecureAttribute.cs",
            "src/{{ cookiecutter.assembly_name }}.Core/Security/SecurityHelper.cs",
            "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/AuditSetup.cs",
            "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/ListAuditEvent.cs",
            "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/ListAuditModel.cs",
        ],
    },
    "include_oauth": {
        "remove_if_false": [
            "src/{{ cookiecutter.assembly_name }}.Core/Configuration/CryptoRandom.cs",
            "src/{{ cookiecutter.assembly_name }}.Core/Extensions/AuthPolicyExtensions.cs",
            "src/{{ cookiecutter.assembly_name }}.Core/Identity/AuthService.cs",
            "src/{{ cookiecutter.assembly_name }}.Core/Identity/CryptoRandom.cs",
            "src/{{ cookiecutter.assembly_name }}.Infrastructure/Configuration/AuthService.cs",
            "src/{{ cookiecutter.assembly_name }}.Data/Abstractions/IAuthService.cs",
        ],
    },
    "include_azure_storage": {
        "remove_if_false": [
            "src/{{ cookiecutter.assembly_name }}.AppHost/Extensions/StorageExtensions.cs",
        ],
    },
}

# ════════════════════════════════════════════════════════════════════════════════
# HELPER FUNCTIONS
# ════════════════════════════════════════════════════════════════════════════════


def as_bool(value: Any) -> bool:
    """Convert various truthy representations to bool."""
    if isinstance(value, bool):
        return value
    return str(value).strip().lower() in ("true", "1", "y", "yes")


def get_flag(name: str) -> bool:
    """Get a boolean flag from the context."""
    return as_bool(CONTEXT.get(name, False))


def get_value(name: str) -> str:
    """Get a string value from the context."""
    return CONTEXT.get(name, "")


def is_empty(value: str | None) -> bool:
    """Check if a value is empty or None."""
    return value is None or str(value).strip() == ""


def env_truthy(name: str) -> bool:
    """Check if an environment variable is truthy."""
    return (os.getenv(name) or "").strip().lower() in ("1", "true", "yes", "y", "on")


def should_skip_user_secrets() -> bool:
    """Determine if user secrets setup should be skipped."""
    return (
        env_truthy("CC_TEMPLATE_UPDATE")
        or env_truthy("COOKIECUTTER_NO_INPUT")
        or env_truthy("GITHUB_ACTIONS")
        or env_truthy("CI")
    )


# ────────────────────────────────────────────────────────────────────────────────
# Shell/subprocess helpers
# ────────────────────────────────────────────────────────────────────────────────


def run_out(args: list[str], cwd: Path | None = None) -> tuple[int, str, str]:
    """Run a command and return (return_code, stdout, stderr)."""
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
    """Run a command, raising SystemExit on failure."""
    p = subprocess.run(cmd, cwd=str(cwd) if cwd else None, text=True, capture_output=True)
    if p.returncode != 0:
        print(f"❌ Command failed: {' '.join(cmd)}", file=sys.stderr)
        if p.stdout.strip():
            print(p.stdout, file=sys.stderr)
        if p.stderr.strip():
            print(p.stderr, file=sys.stderr)
        raise SystemExit(p.returncode)


def git_available() -> bool:
    """Check if git is available on PATH."""
    return run_out(["git", "--version"])[0] == 0


def is_git_repo(path: Path) -> bool:
    """Check if a path is inside a git repository."""
    return run_out(["git", "-C", str(path), "rev-parse", "--is-inside-work-tree"])[0] == 0


# ────────────────────────────────────────────────────────────────────────────────
# Filesystem helpers
# ────────────────────────────────────────────────────────────────────────────────


def rm(item: Path | str) -> None:
    """Remove a file or directory."""
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


# ════════════════════════════════════════════════════════════════════════════════
# VALIDATION
# ════════════════════════════════════════════════════════════════════════════════


def check_conditions(conditions: list[str]) -> bool:
    """Check if ALL conditions in a list are true."""
    return all(get_flag(c) for c in conditions)


def check_any_conditions(condition_groups: list[list[str]]) -> bool:
    """Check if ANY group of conditions is fully satisfied."""
    return any(check_conditions(group) for group in condition_groups)


def validate_required_fields() -> None:
    """Validate required fields based on declarative rules."""
    skip_secrets = should_skip_user_secrets()
    
    for rule in VALIDATION_RULES:
        # Skip secret fields during template updates
        if skip_secrets and rule.get("is_secret", False):
            continue
            
        field = rule["field"]
        
        # Determine if field is required
        is_required = False
        if "required_when" in rule:
            is_required = check_conditions(rule["required_when"])
        elif "required_when_any" in rule:
            is_required = check_any_conditions(rule["required_when_any"])

        if is_required and is_empty(get_value(field)):
            print(f"❌ Cookiecutter value '{field}' is required.", file=sys.stderr)
            raise SystemExit(1)


# ════════════════════════════════════════════════════════════════════════════════
# CLEANUP
# ════════════════════════════════════════════════════════════════════════════════


def perform_cleanup() -> None:
    """Remove files based on cleanup rules."""
    # Compute derived flags
    flags = {
        "database_is_pg": get_value("database") == "PostgreSql",
        "include_azure_key_vault": get_flag("include_azure_key_vault"),
        "include_azure_application_insights": get_flag("include_azure_application_insights"),
        "include_audit": get_flag("include_audit"),
        "include_oauth": get_flag("include_oauth"),
        "include_azure_storage": get_flag("include_azure_storage"),
    }

    for flag_name, rules in CLEANUP_RULES.items():
        flag_value = flags.get(flag_name, False)

        if flag_value and "remove_if_true" in rules:
            for path in rules["remove_if_true"]:
                rm(ROOT / path)

        if not flag_value and "remove_if_false" in rules:
            for path in rules["remove_if_false"]:
                rm(ROOT / path)


# ════════════════════════════════════════════════════════════════════════════════
# USER SECRETS
# ════════════════════════════════════════════════════════════════════════════════


def load_secrets_config() -> dict[str, Any]:
    """Load secrets configuration from external JSON file."""
    if not SECRETS_CONFIG_FILE.exists():
        print(f"⚠️  Secrets config not found: {SECRETS_CONFIG_FILE}")
        return {}
    
    try:
        return json.loads(SECRETS_CONFIG_FILE.read_text())
    except Exception as e:
        print(f"⚠️  Failed to load secrets config: {e}")
        return {}


def should_apply_secret_group(group_config: dict[str, Any]) -> bool:
    """Determine if a secret group should be applied based on its conditions."""
    if "condition" in group_config:
        cond = group_config["condition"]
        if isinstance(cond, list):
            return check_conditions(cond)
        return get_flag(cond)
    
    if "condition_any" in group_config:
        return check_any_conditions(group_config["condition_any"])
    
    return True


def set_user_secret(project_path: Path, secret_name: str, secret_value: str) -> None:
    """Set a single user secret."""
    run(["dotnet", "user-secrets", "set", secret_name, secret_value, "--project", str(project_path)])


def setup_user_secrets() -> None:
    """Initialize and configure user secrets for the AppHost project."""
    if should_skip_user_secrets():
        return

    assembly_name = get_value("assembly_name")
    apphost_csproj = ROOT / f"src/{assembly_name}.AppHost" / f"{assembly_name}.AppHost.csproj"

    print(f"ℹ️  AppHost project path: {apphost_csproj}")

    if not apphost_csproj.exists():
        print(f"⚠️  AppHost project not found: {apphost_csproj}")
        return

    print("⚠️  Please wait... initializing user-secrets")
    run(["dotnet", "user-secrets", "init", "--project", str(apphost_csproj)])

    secrets_config = load_secrets_config()
    
    for group_name, group_config in secrets_config.items():
        if not should_apply_secret_group(group_config):
            continue

        secrets = group_config.get("secrets", {})
        for secret_name, context_key in secrets.items():
            value = get_value(context_key)
            if not is_empty(value):
                set_user_secret(apphost_csproj, secret_name, value)


# ════════════════════════════════════════════════════════════════════════════════
# COOKIECUTTER JSON
# ════════════════════════════════════════════════════════════════════════════════


def prune_deprecated_keys() -> None:
    """Prune deprecated keys from .cookiecutter.json if deprecations file exists."""
    cookie_file = ROOT / ".cookiecutter.json"
    dep_file = ROOT / "templates" / "deprecations.json"

    if not dep_file.exists() or not cookie_file.exists():
        return

    try:
        from prune_cookiecutter_json import load_deprecation_keys, prune_cookiecutter_file

        keys = load_deprecation_keys(dep_file)
        removed = prune_cookiecutter_file(cookie_file, keys)
        if removed:
            print(f"🧹 Pruned deprecated keys from .cookiecutter.json: {', '.join(removed)}")
    except Exception as e:
        print(f"⚠️  Deprecations prune skipped: {e}")


def write_cookiecutter_json() -> None:
    """Write .cookiecutter.json with project context (without template_sha initially)."""
    cookie_file = ROOT / ".cookiecutter.json"

    if cookie_file.exists():
        return

    assembly_name = get_value("assembly_name")
    root_ns = assembly_name.lower().replace(" ", "_").replace("-", "_")

    ctx = {
        "assembly_name": assembly_name,
        "root_namespace": root_ns,
        "api_app_name": get_value("api_app_name"),
        "api_web_url": get_value("api_web_url"),
        "database": root_ns,
        "database_name": get_value("database_name").lower(),
        "github_organization": get_value("github_organization"),
        "include_audit": get_value("include_audit"),
        "include_oauth": get_value("include_oauth"),
        "include_azure_key_vault": get_value("include_azure_key_vault"),
        "include_azure_application_insights": get_value("include_azure_application_insights"),
        "include_azure_storage": get_value("include_azure_storage"),
        "use_existing_azure_key_vault": get_value("use_existing_azure_key_vault"),
        "use_existing_azure_storage": get_value("use_existing_azure_storage"),
        "template_source": get_value("_template"),
        "template_ref": get_value("_checkout"),
    }

    # Remove empty values
    ctx = {k: v for k, v in ctx.items() if v not in ("", None)}

    cookie_file.write_text(json.dumps({"cookiecutter": ctx}, indent=4))
    print("✅  .cookiecutter.json written (without template_sha)")


# ════════════════════════════════════════════════════════════════════════════════
# WORKFLOW FILES
# ════════════════════════════════════════════════════════════════════════════════


def rename_workflow_files() -> None:
    """Rename workflow files from *.j2 to remove the suffix."""
    workflows_dir = ROOT / ".github" / "workflows"

    if not workflows_dir.exists():
        return

    for jf in workflows_dir.rglob("*.j2"):
        target = jf.with_suffix("")
        if target.exists():
            print(f"⚠️  Skipped {jf.relative_to(ROOT)} (already exists: {target.name})")
            continue
        try:
            jf.rename(target)
            print(f"✅ Renamed {jf.relative_to(ROOT)} → {target.relative_to(ROOT)}")
        except Exception as e:
            print(f"⚠️  Failed to rename {jf}: {e}")


# ════════════════════════════════════════════════════════════════════════════════
# GIT INITIALIZATION
# ════════════════════════════════════════════════════════════════════════════════


def ensure_project_git(project_dir: Path, branch: str = "main") -> None:
    """Initialize git repository and create initial commit."""
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

    # Set local identity if none exists
    _, name, _ = run_out(["git", "config", "--get", "user.name"], cwd=project_dir)
    _, email, _ = run_out(["git", "config", "--get", "user.email"], cwd=project_dir)
    if not name:
        run_out(["git", "config", "user.name", "Scaffolder"], cwd=project_dir)
    if not email:
        run_out(["git", "config", "user.email", "scaffolder@example.com"], cwd=project_dir)

    run_out(["git", "add", "-A"], cwd=project_dir)
    code, _, err = run_out(
        ["git", "commit", "-m", "chore(scaffold): initial commit from cookiecutter"],
        cwd=project_dir,
    )
    if code == 0:
        print("✅  Git repo initialized and initial commit created.")
    else:
        print(f"⚠️  Git commit failed (continuing): {err}")


# ════════════════════════════════════════════════════════════════════════════════
# TEMPLATE SHA RESOLUTION
# ════════════════════════════════════════════════════════════════════════════════

_SHA_RE = re.compile(r"^[0-9a-f]{7,40}$", re.I)


def is_local_path(template_ref: str) -> bool:
    """Check if template reference is a local path."""
    s = (template_ref or "").strip()
    if not s:
        return False
    # Remote indicators
    if s.startswith(("gh:", "git@", "http://", "https://", "git+http://", "git+https://")):
        return False
    # Looks like a local path
    return True


def to_git_url(template_ref: str) -> str | None:
    """Convert template reference to git URL."""
    s = (template_ref or "").strip()
    if not s:
        return None
    if s.startswith("gh:"):
        owner_repo = s[3:].strip("/")
        return f"https://github.com/{owner_repo}.git"
    if s.startswith(("git@", "http://", "https://", "git+http://", "git+https://")):
        return s.replace("git+https://", "https://").replace("git+http://", "http://")
    return None


def resolve_sha_from_local(template_path: str, checkout: str | None) -> str | None:
    """Resolve template commit SHA from a local git repository."""
    if not git_available():
        return None

    path = Path(template_path)
    if not path.exists():
        return None

    # Check if it's a git repo
    if not is_git_repo(path):
        return None

    # If checkout is already a SHA, return it
    if checkout and _SHA_RE.fullmatch(checkout):
        return checkout.lower()

    # Get the SHA for the specified ref or HEAD
    ref = checkout if checkout and checkout.lower() != "none" else "HEAD"
    code, sha, _ = run_out(["git", "rev-parse", ref], cwd=path)
    
    if code == 0 and sha and _SHA_RE.fullmatch(sha):
        return sha.lower()

    return None


def resolve_sha_from_remote(template_ref: str, checkout: str | None) -> str | None:
    """Resolve template commit SHA from remote repository."""
    if not git_available():
        return None

    if checkout and _SHA_RE.fullmatch(checkout):
        return checkout.lower()

    git_url = to_git_url(template_ref)
    if not git_url:
        return None

    targets: list[str] = []
    if checkout and checkout.lower() != "none":
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


def resolve_template_sha(template_ref: str, checkout: str | None) -> str | None:
    """Resolve template commit SHA from local path or remote repository."""
    # Normalize "None" string to None
    if checkout and checkout.lower() == "none":
        checkout = None

    if is_local_path(template_ref):
        return resolve_sha_from_local(template_ref, checkout)
    else:
        return resolve_sha_from_remote(template_ref, checkout)


def update_cookiecutter_json_with_sha() -> None:
    """Resolve template SHA and update .cookiecutter.json."""
    template_ref = get_value("_template")
    checkout_ref = get_value("_checkout") or None
    template_sha = resolve_template_sha(template_ref, checkout_ref)

    cookie_file = ROOT / ".cookiecutter.json"

    if template_sha:
        print(f"✅ Template commit SHA: {template_sha}")
        try:
            data = json.loads(cookie_file.read_text())
            data.setdefault("cookiecutter", {})["template_sha"] = template_sha
            cookie_file.write_text(json.dumps(data, indent=4))
            print("📝  Updated .cookiecutter.json with template_sha")

            if git_available() and is_git_repo(ROOT):
                run_out(["git", "add", str(cookie_file)], cwd=ROOT)
                short = template_sha[:12]
                run_out(
                    ["git", "commit", "-m", f"chore(scaffold): record template SHA {short}"],
                    cwd=ROOT,
                )
                print("✅  Committed template_sha to git.")
        except Exception as e:
            print(f"⚠️  Could not update/commit template_sha: {e}")
    else:
        print("⚠️  Template SHA not detected. Please add it to .cookiecutter.json as `template_sha`,")
        print("    or re-run Cookiecutter with `--checkout <branch|tag|commit>` for determinism.")


# ════════════════════════════════════════════════════════════════════════════════
# MAIN
# ════════════════════════════════════════════════════════════════════════════════


def main() -> None:
    """Main entry point for post-gen hook."""
    # 1. Validate required fields
    validate_required_fields()

    # 2. Setup user secrets (if not skipped)
    setup_user_secrets()

    # 3. Perform conditional cleanup
    perform_cleanup()

    # 4. Prune deprecated keys and write .cookiecutter.json
    prune_deprecated_keys()
    write_cookiecutter_json()

    # 5. Rename workflow files
    rename_workflow_files()

    # 6. Initialize git repository
    ensure_project_git(ROOT)

    # 7. Resolve and commit template SHA
    update_cookiecutter_json_with_sha()

    print("🎉 Post-gen hook completed")


if __name__ == "__main__":
    main()