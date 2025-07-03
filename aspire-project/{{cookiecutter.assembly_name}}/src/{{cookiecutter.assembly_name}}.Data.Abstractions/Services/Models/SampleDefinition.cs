namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;

public record SampleDefinition(
    {% if cookiecutter.database == "PostgreSql" %}
    int? SampleId,
    {% elif cookiecutter.database == "MongoDb" %}
    string Id,
    {% endif %}
    string Name,
    string Description
);
