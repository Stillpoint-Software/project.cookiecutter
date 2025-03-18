{% if cookiecutter.include_audit =='yes'%}
using Audit.Core;
{% endif %}
using FluentValidation.Results;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using {{cookiecutter.assembly_name}}.Api.Commands.Infrastructure;
using {{cookiecutter.assembly_name}}.Api.Commands.Middleware;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Microsoft.Extensions.Logging;
using {{cookiecutter.assembly_name}}.Api.Infrastructure;

namespace {{cookiecutter.assembly_name}}.Api.Commands.SampleArea;

{% if cookiecutter.database =="PostgreSql" %}
{% include "/templates/docker/api_get_sample_postgresql.cs" %}
{% elif cookiecutter.database =="MongoDb" %}
{% include "/templates/docker/api_get_sample_mongodb.cs" %}
{% endif %}