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