using {{cookiecutter.assembly_name}}.Data.Abstractions;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Microsoft.Extensions.Logging;
{% if cookiecutter.database == "PostgreSql" %}
using Microsoft.EntityFrameworkCore;
{% elif cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using MongoDB.Driver.Linq;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.Services;

{% if cookiecutter.database == "PostgreSql" %}
    {% include 'templates/audit/data_sample_svc_postgesql.cs' %}
{% elif cookiecutter.database == "MongoDb" %}
    {% include 'templates/audit/data_sample_svc_mondodb.cs' %}
{% endif %}
