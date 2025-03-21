using {{ cookiecutter.assembly_name}}.Extensions;
using Lamar.Microsoft.DependencyInjection;
using Serilog;

namespace {{ cookiecutter.assembly_name}};

{% if cookiecutter.include_azure == "yes" %}
{% include '/templates/docker/main/main_program_azure.cs' %}
{% else %}
{% include '/templates/docker/main/main_program.cs' %}
{% endif %}