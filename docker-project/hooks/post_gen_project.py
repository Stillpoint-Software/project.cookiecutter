#!/usr/bin/env python
"""
This hook runs automatically after Cookiecutter finishes generating the project.
It captures the exact Git commit SHA of the template you used, then writes it
into the generated project's .cookiecutter.json under "template_sha" so future
template updates can update the project correctly.

 ***Aborts generation*** when the template is not a Git repo (no .git).

Works both:
• Interactively on dev machines (Windows/macOS/Linux)
• Non-interactive in CI runners (e.g., GitHub Actions ubuntu-latest)
"""
from __future__ import annotations
import json, shutil, subprocess, sys
from pathlib import Path
from typing import Iterable

# ──────────────────────────────────────────── #
# Paths
# ──────────────────────────────────────────── #
ROOT = Path.cwd()                                   # generated project root
SRC  = ROOT / "src" / "{{ cookiecutter.assembly_name }}"

# ──────────────────────────────────────────── #
# Helpers
# ──────────────────────────────────────────── #
def rm(item: Path | str) -> None:
    p = Path(item)
    if not p.exists():
        return
    shutil.rmtree(p) if p.is_dir() else p.unlink()
    print(f"🗑️  Removed {'dir' if p.is_dir() else 'file'}: {p}")

def rm_each(paths: Iterable[Path | str]) -> None:
    for p in paths:
        rm(p)

_yes = lambda s: (s or "").strip().lower() == "yes"

# ──────────────────────────────────────────── #
# Answers
# ──────────────────────────────────────────── #
include_azure_key_vault    = _yes("{{ cookiecutter.include_azure_key_vault }}")
include_azure_application_insights = _yes("{{ cookiecutter.include_azure_application_insights }}")
include_azure_storage      = _yes("{{ cookiecutter.include_azure_storage }}")
include_azure_service_bus  = _yes("{{ cookiecutter.include_azure_service_bus }}")
database_is_pg       = "{{ cookiecutter.database }}" == "PostgreSql"
include_audit        = _yes("{{ cookiecutter.include_audit }}")
include_oauth        = _yes("{{ cookiecutter.include_oauth }}")
aspire_deploy        = _yes("{{ cookiecutter.aspire_deploy }}")
github_deploy        = _yes("{{ cookiecutter.github_deployment }}")
project_path         = "{{ cookiecutter.project_path }}"
template_path        = "{{ cookiecutter.template_path }}"
github_organization  = "{{ cookiecutter.github_organization }}"

# ──────────────────────────────────────────── #
# 1️⃣  Conditional clean-up (same logic as before)
# ──────────────────────────────────────────── #
# … leave your existing rm_each blocks here …

# Always drop template snippets directory
rm(ROOT / "templates")

# ──────────────────────────────────────────── #
# 2️⃣  Optional deployment helper (unchanged)
# ──────────────────────────────────────────── #
if aspire_deploy and include_azure_key_vault or include_azure_application_insights or include_azure_storage or include_azure_service_bus and project_path:
    try:
        subprocess.run(
            [
                sys.executable,
                str(ROOT / "deployment.py"),
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

# ──────────────────────────────────────────── #
# 2️⃣.5️⃣  Capture template commit SHA (mandatory)
# ──────────────────────────────────────────── #
hook_path     = Path(__file__).resolve()
tmp_template_root = hook_path.parent.parent   # /tmp clone (for Git URLs)
template_sha: str | None = None

def read_sha(repo_path: Path) -> str | None:
    """Return HEAD SHA if *repo_path* is a git repo, else None."""
    if not (repo_path / ".git").exists():
        return None
    return subprocess.check_output(
        ["git", "-C", str(repo_path), "rev-parse", "HEAD"]
    ).decode().strip()

# 1️⃣  Try temp directory first (remote URLs)
template_sha = read_sha(tmp_template_root)

# 2️⃣  Fallback: original template argument (handles local git repos)
if template_sha is None:
    template_arg = "{{ cookiecutter._template }}"
    if not template_arg.startswith(("http://", "https://", "git@", "ssh://", "gh:")):
        template_sha = read_sha(Path(template_arg).expanduser())

# 3️⃣  If still None, abort
if template_sha is None:
    print("❌ Template is not a git repository – cancelling project creation.")
    sys.exit(1)

print(f"✅ Template commit SHA: {template_sha}")

# ──────────────────────────────────────────── #
# 3️⃣  Persist minimal replay context
# ──────────────────────────────────────────── #
COOKIE_FILE = ROOT / ".cookiecutter.json"
if not COOKIE_FILE.exists():
    ctx: dict[str, str] = {
        # core answers
        "assembly_name": "{{ cookiecutter.assembly_name }}",
        "root_namespace": "{{ cookiecutter.root_namespace }}",
        "database": "{{ cookiecutter.database }}",
        "database_name": "{{ cookiecutter.database_name }}",
        "github_organization": github_organization,

        # toggles
        "is_docker": "yes",
        "include_audit": "{{ cookiecutter.include_audit }}",
        "include_oauth": "{{ cookiecutter.include_oauth }}",
        "include_azure_key_vault": "{{ cookiecutter.include_azure_key_vault }}",
        "include_azure_application_insights": "{{ cookiecutter.include_azure_application_insights }}",
        "include_azure_storage": "{{ cookiecutter.include_azure_storage }}",
        "include_azure_service_bus": "{{ cookiecutter.include_azure_service_bus }}",
        "aspire_deploy": "{{ cookiecutter.aspire_deploy }}",

        # mandatory SHA
        "template_sha": template_sha,
    }

    ctx = {k: v for k, v in ctx.items() if v not in ("", None)}
    COOKIE_FILE.write_text(json.dumps({"cookiecutter": ctx}, indent=4))
    print("✅  .cookiecutter.json written (with cookiecutter key)")

print("🎉 Docker post-gen hook completed successfully")