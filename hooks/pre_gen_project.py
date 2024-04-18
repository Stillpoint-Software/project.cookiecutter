import sys
import cookiecutter.prompt

if "{{ cookiecutter.include_oauth }}" =="yes":
    cookiecutter.prompt.read_user_variable("oauth_app_name","Enter the OAuth audience for development, (https://{project_dev_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_audience","Enter the OAuth audience for development, (https://{project_dev_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_api_audience_dev","Enter the OAuth audience for development, (https://{project_dev_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_api_audience_prod","Enter the OAuth audience for production, (https://{project_production_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_domain_dev","email_contact_here: Format @email")
    cookiecutter.prompt.read_user_variable("oauth_domain_prod","email_contact_here: Format @email")
else:
    """{{ cookiecutter.update(
        {
            "oauth_app_name": "",
            "oauth_audience": "",
            "oauth_api_audience_dev": "",
            "oauth_api_audience_prod": "",
            "oauth_domain_dev": "",
            "oauth_domain_prod": "",

        }
    )}}"""

if "{{ cookiecutter.include_azure }}" =="yes":
    cookiecutter.prompt.read_user_variable("azure_tenant_id", "Enter the Azure tenantId")
    cookiecutter.prompt.read_user_variable("azure_subscription_id", "Enter the Azure subscriptionId")
    cookiecutter.prompt.read_user_variable("azure_location", "Enter the Azure region, (eastus)")
    cookiecutter.prompt.read_user_variable("azure_key_vault_staging", "Enter the name of the Azure key vault for staging, ({projectName}-Staging)")
    cookiecutter.prompt.read_user_variable("azure_key_vault_prod", "Enter the name of the Azure key vault for production, ({projectName}-Production")
    cookiecutter.prompt.read_user_variable("azure_storage_connection_staging", "Enter the connection string to Azure storage")
    cookiecutter.prompt.read_user_variable("azure_container_dev", "Enter the storage container name for development, (development)")
    cookiecutter.prompt.read_user_variable("azure_container_staging", "Enter the storage container name for staging, (staging)")
    cookiecutter.prompt.read_user_variable("azure_container_prod", "Enter the storage container name for production, (production)")
    cookiecutter.prompt.read_user_variable("azure_storage_account_name_dev", "Enter the storage account name for development, ({projectName}}assetsstaging)")
    cookiecutter.prompt.read_user_variable("azure_storage_account_name_prod", "Enter the storage account name for production, ({projectName}}assetsprod)")
    cookiecutter.prompt.read_user_variable("azure_container_registry_server_staging", "Enter the Azure container registry name for staging, (cr3hmn6weg7opbk.azurecr.io)")
    cookiecutter.prompt.read_user_variable("azure_container_registry_user_staging", "Enter the Azure container user for staging, (cr3hmn6weg7opbk)")
    cookiecutter.prompt.read_user_variable("azure_container_registry_server_prod", "Enter the Azure container registry name for production, (crcp24evzhtokxg.azurecr.io)")     
    cookiecutter.prompt.read_user_variable("azure_container_registry_user_prod", "Enter the Azure container user for production, (crcp24evzhtokxg)")
else:
    """{{ cookiecutter.update(
        {
            "azure_tenant_id": "",
            "azure_subscription_id": "",
            "azure_location": "",
            "azure_key_vault_staging": "",
            "azure_key_vault_prod": "",
            "azure_storage_connection_staging": "",
            "azure_container_dev": "",
            "azure_container_staging": "",
            "azure_container_prod": "",
            "azure_storage_account_name_dev": "",
            "azure_storage_account_name_prod": "",
            "azure_container_registry_server_staging": "",
            "azure_container_registry_user_staging": "",
            "azure_container_registry_server_prod": "",
            "azure_container_registry_user_prod": "",
        }
    )}}"""

sys.exit(0)