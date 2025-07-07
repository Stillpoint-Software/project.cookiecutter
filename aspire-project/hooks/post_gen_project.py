import os 
from collections import OrderedDict
import shutil
import sys
import subprocess
import json

print(os.getcwd())  # prints src/{{ cookiecutter.assembly_name }}

def remove(filepath):
    if os.path.isfile(filepath):
        os.remove(filepath)
        print(f'Removed file: {filepath}')
    elif os.path.isdir(filepath):
        shutil.rmtree(filepath)
        print(f'Removed directory: {filepath}')
        
azure = '{{cookiecutter.include_azure}}'=='yes'
database = '{{cookiecutter.database}}' =='PostgreSql'
audit = '{{cookiecutter.include_audit}}'=='yes'
auth = '{{cookiecutter.include_oauth}}'=='yes'
deploy = '{{cookiecutter.aspire_deploy}}'=='yes'
github = '{{cookiecutter.github_deployment}}'=='yes'
project_path = '{{cookiecutter.project_path}}' 
template_path = '{{cookiecutter.template_path}}'

if not azure:
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}', 'Extensions\ApplicationInsightsExtension.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}', 'Extensions\AzureSecretsExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations','Extensions\AzureSecretsExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Vault'))

if database: # delete mongo files
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','Extensions\MongoExtensions.cs')) 
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'Services\MongoDbService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'BsonCollectionAttribute.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'Services\MongoDbService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}.Data.Abstractions', 'SecurityHelper.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'Resources\\1000-Initial\\administration\\users\\user.json'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.Abstractions','Services\IMongoDbService.cs'))

if not database: # delete postgres files
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'DbConnectionProvider.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'Resources\\1000-Initial\\CreateUsers.sql'))

if audit == False:
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Infrastructure\AuditSetup.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Infrastructure\ListAuditEvent.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Infrastructure\ListAuditModel.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Abstractions','Secure.cs'))

if auth == False:
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api', 'Identity\AuthService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api', 'Identity\CryptoRandom.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api', 'Extensions\AuthPolicyExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api', 'Infrastructure\SecurityRequirementsOperationFilter.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.Abstractions','Services\IAuthService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','Settings.cs'))

# Remove templates
remove(os.path.join('src/templates'))

# if azure and aspire and github setup deployment
if deploy and azure and project_path:
    # Call Python script with variables to setup for deployment process
    try:
        script_path =  os.path.join('deployment.py')  

        subprocess.run(["python", script_path, "{{cookiecutter.deployment_environment}}", "{{cookiecutter.assembly_name}}", str(github).lower(), "{{cookiecutter.database}}", project_path, template_path], check=True)
        print(f"Successfully executed '{script_path}' with variables")
    except subprocess.CalledProcessError as e:
        print(f"Error running '{script_path}': {e}")
        sys.exit(1)

def main():
    context = OrderedDict([
        ("assembly_name", "{{ cookiecutter.assembly_name }}"),
        ("root_namespace", "{{ cookiecutter.root_namespace }}"),
        ("api_app_name", "{{ cookiecutter.api_app_name }}"),
        ("api_web_url", "{{ cookiecutter.api_web_url }}"),
        ("database", "{{ cookiecutter.database }}"),
        ("database_name", "{{ cookiecutter.database_name }}"),
        ("include_audit", "{{ cookiecutter.include_audit }}"),
        ("include_oauth", "{{ cookiecutter.include_oauth }}"),
        ("include_azure", "{{ cookiecutter.include_azure }}"),
        ("aspire_deploy", "{{ cookiecutter.aspire_deploy }}")
    ])

    # 🔐 Conditionally add OAuth context
    if "{{ cookiecutter.include_oauth }}" == "yes":
        context.update({
            "oauth_app_name": "{{ cookiecutter.oauth_app_name }}",
            "oauth_audience": "{{ cookiecutter.oauth_audience }}",
            "oauth_api_audience_dev": "{{ cookiecutter.oauth_api_audience_dev }}",
            "oauth_api_audience_prod": "{{ cookiecutter.oauth_api_audience_prod }}",
            "oauth_domain_dev": "{{ cookiecutter.oauth_domain_dev }}",
            "oauth_domain_prod": "{{ cookiecutter.oauth_domain_prod }}"
        })

    # ☁️ Conditionally add Azure context
    if "{{ cookiecutter.include_azure }}" == "yes":
        context.update({
            "azure_tenant_id": "{{ cookiecutter.azure_tenant_id }}",
            "azure_subscription_id": "{{ cookiecutter.azure_subscription_id }}",
            "azure_location": "{{ cookiecutter.azure_location }}",
            "azure_key_vault_staging": "{{ cookiecutter.azure_key_vault_staging }}",
            "azure_key_vault_prod": "{{ cookiecutter.azure_key_vault_prod }}",
            "azure_storage_connection_staging": "{{ cookiecutter.azure_storage_connection_staging }}",
            "azure_container_dev": "{{ cookiecutter.azure_container_dev }}",
            "azure_container_staging": "{{ cookiecutter.azure_container_staging }}",
            "azure_container_prod": "{{ cookiecutter.azure_container_prod }}",
            "azure_storage_account_name_dev": "{{ cookiecutter.azure_storage_account_name_dev }}",
            "azure_storage_account_name_prod": "{{ cookiecutter.azure_storage_account_name_prod }}",
            "azure_container_registry_server_staging": "{{ cookiecutter.azure_container_registry_server_staging }}",
            "azure_container_registry_user_staging": "{{ cookiecutter.azure_container_registry_user_staging }}",
            "azure_container_registry_server_prod": "{{ cookiecutter.azure_container_registry_server_prod }}",
            "azure_container_registry_user_prod": "{{ cookiecutter.azure_container_registry_user_prod }}"
        })

    # 🚀 Conditionally add Aspire deployment values
    if "{{ cookiecutter.aspire_deploy }}" == "yes":
        context["deployment_environment"] = "{{ cookiecutter.deployment_environment }}"
        context["project_path"] = "{{ cookiecutter.project_path }}"
        context["github_deployment"] = "{{ cookiecutter.github_deployment }}"
        context["template_path"] = "{{ cookiecutter.template_path }}"

    # 📝 Write the final context
    try:
        with open(".cookiecutter.json", "w") as f:
            json.dump(context, f, indent=4)
        print("✅ .cookiecutter.json created successfully.")
    except Exception as e:
        print("❌ Error writing .cookiecutter.json:", e)

if __name__ == "__main__":
    main()