using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Services.Models;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
{% endif %}

namespace {{cookiecutter.assembly_name}}.Data.Abstractions.Services;
public interface ISampleService
{
   {% if cookiecutter.database == "PostgreSql" %}
   {% include 'templates/abstractions/postgresql_service.cs' %}
   {% endif %}
 
   {% if cookiecutter.database == "MongoDb" %}
   {% include 'templates/abstractions/mongodb_service.cs' %}
   {% endif %}
}