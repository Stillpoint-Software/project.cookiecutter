"""
docker pre_gen_project.py

• Interactive shell  → prompt for OAuth values.  
• CI / --no-input   → fill keys with "" automatically.
"""

from __future__ import annotations
import os
import sys
from cookiecutter import prompt

# --------------------------------------------------------------------------- #
# Detect --no-input (Cookiecutter sets COOKIECUTTER_NO_INPUT=1 for CI runs)
# --------------------------------------------------------------------------- #
NO_INPUT = os.getenv("COOKIECUTTER_NO_INPUT") == "1"

def maybe_ask(var: str, question: str) -> str:
    """Return the user’s answer, or an empty string in CI mode."""
    if NO_INPUT:
        return ""
    return prompt.read_user_variable(var, question)

# --------------------------------------------------------------------------- #
# Prompt (or stub) OAuth values
# --------------------------------------------------------------------------- #
if "{{ cookiecutter.include_oauth }}" == "yes":
    for var, q in {
        "oauth_app_name":          "Enter the OAuth application name for DEV (ex: https://{project_dev_domain}/api/v2/)",
        "oauth_audience":          "Enter the OAuth audience for DEV (ex: https://{project_dev_domain}/api/v2/)",
        "oauth_api_audience_dev":  "Enter the OAuth API audience for DEV",
        "oauth_api_audience_prod": "Enter the OAuth API audience for PROD",
        "oauth_domain_dev":        "Enter the OAuth domain for DEV (ex: dev-xxxxx.us.auth0.com)",
        "oauth_domain_prod":       "Enter the OAuth domain for PROD (ex: prod-xxxxx.us.auth0.com)",
    }.items():
        # Cookiecutter stores answers in the global 'cookiecutter' dict
        cookiecutter.variables[var] = maybe_ask(var, q)
else:
    # Ensure keys exist even when OAuth is disabled
    for var in (
        "oauth_app_name",
        "oauth_audience",
        "oauth_api_audience_dev",
        "oauth_api_audience_prod",
        "oauth_domain_dev",
        "oauth_domain_prod",
    ):
        cookiecutter.variables[var] = ""

sys.exit(0)
