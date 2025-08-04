"""
pre_gen_project.py

Runs *before* project generation.  
• Interactive sessions (no --no-input): ask the user the questions you defined.  
• Non-interactive sessions (e.g. GitHub Actions with --no-input=1): silently
  set every optional value to "" so Cookiecutter continues without a TTY.
"""

from __future__ import annotations

import os
import sys

from cookiecutter import prompt
from cookiecutter.main import cookiecutter

# --------------------------------------------------------------------------- #
# Detect --no-input
# --------------------------------------------------------------------------- #
# • In CLI usage Cookiecutter sets env var COOKIECUTTER_NO_INPUT=1
#   when you pass --no-input.
# • We ALSO detect GitHub Actions (CI=true) just in case, but the env var
#   is the reliable signal.
NO_INPUT = os.getenv("COOKIECUTTER_NO_INPUT") == "1"

# Helper: prompt when interactive, else return ""
def maybe_ask(var: str, question: str) -> str:
    if NO_INPUT:
        return ""  # CI mode: skip prompt
    return prompt.read_user_variable(var, question)  # raises on Ctrl-C

# --------------------------------------------------------------------------- #
# OAuth questions
# --------------------------------------------------------------------------- #
if "{{ cookiecutter.include_oauth }}" == "yes":
    oauth_vars = {
        "oauth_app_name":           "Enter the OAuth application name for DEV (ex: https://{project_dev_domain}/api/v2/)",
        "oauth_audience":           "Enter the OAuth audience for DEV (ex: https://{project_dev_domain}/api/v2/)",
        "oauth_api_audience_dev":   "Enter the OAuth API audience for DEV",
        "oauth_api_audience_prod":  "Enter the OAuth API audience for PROD",
        "oauth_domain_dev":         "Enter the OAuth domain for DEV (ex: dev-xxxxx.us.auth0.com)",
        "oauth_domain_prod":        "Enter the OAuth domain for PROD (ex: prod-xxxxx.us.auth0.com)",
    }

    # Ask (or stub) every var
    for var, question in oauth_vars.items():
        cookiecutter.variables[var] = maybe_ask(var, question)

# --------------------------------------------------------------------------- #
# Azure questions
# --------------------------------------------------------------------------- #
if "{{ cookiecutter.include_azure_application_insights }}" == "yes":
    azure_application_insights_vars = {
       "azure_application_insights_staging":               "Application Insights name for STAGING",
       "azure_application_insights_prod":                  "Application Insights name for PROD",
    }

    for var, question in azure_application_insights_vars.items():
        cookiecutter.variables[var] = maybe_ask(var, question)
        
    
if "{{ cookiecutter.include_azure_key_vault }}" == "yes":
    azure_key_vault_vars = {
       "azure_key_vault_staging":               "Key Vault name for STAGING",
       "azure_key_vault_prod":                  "Key Vault name for PROD",
    }
    
    for var, question in azure_key_vault_vars.items():
        cookiecutter.variables[var] = maybe_ask(var, question)

if "{{ cookiecutter.include_azure_storage }}" == "yes":
    azure_storage_vars = {
            "azure_storage_connection_staging":      "Storage connection string (staging)",
            "azure_container_dev":                   "Blob container for DEV",
            "azure_container_staging":               "Blob container for STAGING",
            "azure_container_prod":                  "Blob container for PROD",
            "azure_storage_account_name_dev":        "Storage account name (DEV)",
            "azure_storage_account_name_prod":       "Storage account name (PROD)"
        }
        
    for var, question in azure_storage_vars.items():
        cookiecutter.variables[var] = maybe_ask(var, question)
        
if "{{ cookiecutter.include_azure_key_vault }}" == "yes" or "{{ cookiecutter.include_azure_storage }}" == "yes" or "{{ cookiecutter.include_azure_application_insights }}" == "yes" or "{{ cookiecutter.include_azure_service_bus }}" == "yes":
    azure_config_vars = {
        "azure_tenant_id":                       "Azure tenant ID",
        "azure_subscription_id":                 "Azure subscription ID",
        "azure_location":                        "Azure region (ex: eastus)"
    }
    
    for var, question in azure_config_vars.items():
        cookiecutter.variables[var] = maybe_ask(var, question)

# --------------------------------------------------------------------------- #
# If we skipped prompts, cookiecutter.variables already has empty strings
# for the keys (satisfies later hooks/templates). Nothing else to do.
# --------------------------------------------------------------------------- #
sys.exit(0)
