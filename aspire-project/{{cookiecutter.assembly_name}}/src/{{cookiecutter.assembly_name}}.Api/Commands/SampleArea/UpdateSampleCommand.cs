{% if cookiecutter.include_audit == 'yes' %}
using Audit.Core;
using {{ cookiecutter.assembly_name}}.Data.{{ cookiecutter.database}};
{% endif %}
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using MongoDB.Bson;
{% endif %}
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Commands;
using Hyperbee.Pipeline.Context;
using {{ cookiecutter.assembly_name }}.Core.Commands;
using {{ cookiecutter.assembly_name }}.Core.Commands.Middleware;
using {{ cookiecutter.assembly_name }}.Core.Extensions;
using {{ cookiecutter.assembly_name }}.Data.Abstractions.Services;
using {{ cookiecutter.assembly_name }}.Data.Abstractions.Services.Models;
using Microsoft.Extensions.Logging;
using {{ cookiecutter.assembly_name }}.Data.Abstractions.Entity;

namespace {{cookiecutter.assembly_name }}.Api.Commands.SampleArea;

{% if cookiecutter.database == "PostgreSql" %}
{% include "templates/api/api_update_sample_postgresql.cs" %}
{% elif cookiecutter.database == "MongoDb" %}
{% include "templates/api/api_update_sample_mongodb.cs" %}
{% endif %}