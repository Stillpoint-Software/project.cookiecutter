﻿
namespace {{cookiecutter.assembly_name}}.Api;

public class ApiSettings
{
    public string AppName { get; set; }
    public string WebUrl { get; set; }
}
{% if cookiecutter.include_azure == "yes" %}
    {% include '/templates/docker/main/main_settings.cs' %}
{% endif %}
