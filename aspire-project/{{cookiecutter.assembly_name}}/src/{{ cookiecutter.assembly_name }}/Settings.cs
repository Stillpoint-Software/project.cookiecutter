
namespace {{ cookiecutter.assembly_name }};

public class ApiSettings
{
    public string AppName { get; set; }
    public string WebUrl { get; set; }
}

{% if cookiecutter.include_azure == "yes" %}
{% include 'templates/main/main_settings.cs' %}
{% endif %}