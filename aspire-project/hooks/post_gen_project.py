import os
import shutil
import sys
import subprocess

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

