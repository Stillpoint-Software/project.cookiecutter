{% if cookiecutter.include_audit == 'yes' %}
using Audit.Core;
{% endif %}
using FluentValidation.Results;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using {{ cookiecutter.assembly_name }}.Core.Commands;
using {{ cookiecutter.assembly_name }}.Core.Commands.Middleware;
using {{ cookiecutter.assembly_name }}.Data.Abstractions.Services;
using {{ cookiecutter.assembly_name }}.Data.Abstractions.Services.Models;
using {{ cookiecutter.assembly_name }}.Core.Extensions;


namespace {{cookiecutter.assembly_name }}.Api.Commands.SampleArea;

{% if cookiecutter.database == "PostgreSql" %}
{% include 'templates/api/get_sample_postgresql.cs' %}
{% elif cookiecutter.database == "MongoDb" %}
{% include 'templates/api/get_sample_mongodb.cs' %}
{% endif %}