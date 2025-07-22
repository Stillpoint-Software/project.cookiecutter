using Microsoft.Extensions.Logging;
using {{ cookiecutter.assembly_name }}.Core.Services;
using {{ cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{ cookiecutter.assembly_name}}.Data.Abstractions.Services;
using {{ cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
using Microsoft.Extensions.Logging;
{% if cookiecutter.database == "PostgreSql" %}
using Microsoft.EntityFrameworkCore;
{% elif cookiecutter.database == "MongoDb" %}
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
{% endif %}

namespace {{cookiecutter.assembly_name }}.Data.{{ cookiecutter.database }}.Services;

{% if cookiecutter.database == "PostgreSql" %}
{% include "templates/data/sample_svc_postgresql.cs" %}
{% elif cookiecutter.database == "MongoDb" %}
{% include "templates/data/sample_svc_mongodb.cs" %}
{% endif %}
