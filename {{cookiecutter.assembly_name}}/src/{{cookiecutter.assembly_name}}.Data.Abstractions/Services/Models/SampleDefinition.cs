namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;

public record SampleDefinition(
    {%- if cookiecutter.database == "Postgresql" -%}
    int SampleId,
    {%- elif cookiecutter.database == "Mongo" -%}
    string SampleId,
    {% endif %}
    string Name,
    string Description
);
