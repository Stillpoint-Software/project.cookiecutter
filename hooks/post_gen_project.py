import os
import shutil

print(os.getcwd())  # prints src/{{ cookiecutter.assembly_name }}

def remove(filepath):
    if os.path.isfile(filepath):
        os.remove(filepath)
    elif os.path.isdir(filepath):
        shutil.rmtree(filepath)


azure = '{{cookiecutter.include_azure}}' =='yes'

if not azure:
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}', 'Extensions\ApplicationInsightsExtension.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}', 'Extensions\AzureSecretsExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api', 'Identity\AuthService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Api', 'Identity\CryptoRandom.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.Abstractions','Services\IAuthService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations','Extensions\AzureSecretsExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}','Settings.cs'))

databasePostgres = '{{cookiecutter.database}}' =='Postgresql'

if databasePostgres:
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'Extensions\MongoExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'Services\MongoDbService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'BsonCollectionAttribute.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.Abstractions', 'Services\IMongoDbService.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'Extensions\BsonDocumentExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'Extensions\MongoDatabaseExtensions.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'Scripts\Script0001-Initial.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\DuplicateMigrationException.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\DuplicateMigrationException.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\Migration.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\MigrationActivator.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\MigrationDatabaseProvider.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\MigrationException.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\MigrationLocator.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\MigrationRecord.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\MigrationRepository.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'System\MigrationRunner.cs'))

if not databasePostgres:
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'DbConnectionProvider.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Data.{{cookiecutter.database}}', 'SampleContext.cs'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'Scripts\HardReset.sql'))
    remove(os.path.join('src/{{ cookiecutter.assembly_name }}.Migrations', 'Scripts\Script0001-Initial.sql'))
