import os
import shutil
import subprocess
from collections import OrderedDict
import json

print(os.getcwd())  # prints src/{{ cookiecutter.assembly_name }}

def remove(filepath):
    if os.path.isfile(filepath):
        os.remove(filepath)
        print(f'Removed file: {filepath}')
    elif os.path.isdir(filepath):
        shutil.rmtree(filepath)
        print(f'Removed directory: {filepath}')

def is_docker_installed() -> bool:
    try:
        subprocess.run(["docker", "--version"], capture_output=True, check=True)
        return True
    except Exception:
        return False

   
azure = '{{cookiecutter.include_azure}}'=='yes'
database = '{{cookiecutter.database}}' =='PostgreSql'
audit = '{{cookiecutter.include_audit}}'=='yes'
auth = '{{cookiecutter.include_oauth}}'=='yes'

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

    # 📝 Write the final context
    try:
        with open(".cookiecutter.json", "w") as f:
            json.dump(context, f, indent=4)
        print("✅ .cookiecutter.json created successfully.")
    except Exception as e:
        print("❌ Error writing .cookiecutter.json:", e)

if __name__ == "__main__":
    main()