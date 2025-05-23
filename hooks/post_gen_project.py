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

def is_docker_installed() -> bool:
    try:
        subprocess.run(["docker", "--version"], capture_output=True, check=True)
        return True
    except Exception:
        return False

aspire ='{{cookiecutter.include_aspire}}' =='yes'

if not aspire:
    if not is_docker_installed():
        print('ERROR: Docker is not installed.')
        sys.exit(1)
   
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

if not aspire: # Remove Aspire files/folders 
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.AppHost'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.ServiceDefaults'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations','Startup.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api', 'Infrastructure\SerilogSetup.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Extensions\LoggerConfigurationExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Program.cs'))

if aspire: # Remove docker files/folders
    remove(os.path.join('src/{{cookiecutter.assembly_name}}'))
    remove(os.path.join('.dockerignore'))
    remove(os.path.join('Directory.Build.props'))
    remove(os.path.join('Directory.Build.targets'))
    remove(os.path.join('docker-compose.yml'))
    remove(os.path.join('docker-compose.override.yml'))
    remove(os.path.join('docker-compose.dcproj'))
    remove(os.path.join('src/{{cookiecutter.assembly_name}}', 'Dockerfile'))
    remove(os.path.join('src/{{cookiecutter.assembly_name}}.Migrations', 'Dockerfile'))
    remove(os.path.join('src/{{cookiecutter.assembly_name}}.Migrations', 'Extensions\BootstrapExtensions.cs'))
    remove(os.path.join('src/{{cookiecutter.assembly_name}}.Migrations', 'Extensions\AzureSecretsExtensions.cs'))
    remove(os.path.join('src/{{cookiecutter.assembly_name}}.Migrations', 'Extensions\LoggerConfigurationExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Settings.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api','Extentions\LoggerConfigurationExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','BsonCollectionAttribute.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','DbConnectionProvider.cs'))  
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','Services\MongoDbService.cs')) 
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','Extensions\MongoExtensions.cs')) 
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','Startup.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations','/Scripts'))
    remove(os.path.join('tests/{{cookiecutter.assembly_name}}.Tests', 'Dockerfile'))

if aspire and database: # Remove aspire files if postgesql
    remove(os.path.join('src/{{cookiecutter.assembly_name}}.Migrations', 'Extensions\StartupExtensions.cs'))
    remove(os.path.join('src/{{cookiecutter.assembly_name}}.Migrations', 'Migration'))

if aspire and not database: # Remove aspire files if mongo
    remove(os.path.join('src/{{cookiecutter.assembly_name}}.Migrations', 'Scripts'))

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
remove(os.path.join('templates'))

# if azure and aspire and github setup deployment
if deploy and azure and aspire and project_path:
    # Call Python script with variables to setup for deployment process
    try:
        script_path =  os.path.join('deployment.py')  

        subprocess.run(["python", script_path, "{{cookiecutter.deployment_environment}}", "{{cookiecutter.assembly_name}}", str(github).lower(), "{{cookiecutter.database}}", project_path, template_path], check=True)
        print(f"Successfully executed '{script_path}' with variables")
    except subprocess.CalledProcessError as e:
        print(f"Error running '{script_path}': {e}")
        sys.exit(1)

