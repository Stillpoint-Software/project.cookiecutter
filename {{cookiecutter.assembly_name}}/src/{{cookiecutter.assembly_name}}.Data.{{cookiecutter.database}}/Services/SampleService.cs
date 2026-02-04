using Microsoft.Extensions.Logging;
using {{cookiecutter.assembly_name }}.Core.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
{%- if cookiecutter.database == "PostgreSql" %}
using Microsoft.EntityFrameworkCore;
{%- endif %}
{%- if cookiecutter.database == "MongoDb" %}
using Microsoft.EntityFrameworkCore;
{%- if cookiecutter.include_audit %}
using MongoDB.Driver;
{%- endif %}
using MongoDB.Bson;
{%- endif %}

namespace {{cookiecutter.assembly_name }}.Data.{{cookiecutter.database }}.Services;

{%- if cookiecutter.database == "PostgreSql" %}
{% include "templates/data/sample_svc_postgresql.cs" %}
{%- endif %}
{%- if cookiecutter.database == "MongoDb" %}
{% include "templates/data/sample_svc_mongodb.cs" %}
{%- endif %}
