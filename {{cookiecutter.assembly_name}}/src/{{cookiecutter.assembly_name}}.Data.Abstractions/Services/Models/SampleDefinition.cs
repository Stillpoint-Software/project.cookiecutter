namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;

public record SampleDefinition(
    int SampleId,
    string Name,
    string Description
);
