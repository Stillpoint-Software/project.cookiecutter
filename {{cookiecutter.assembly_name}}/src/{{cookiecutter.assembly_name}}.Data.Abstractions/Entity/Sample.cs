{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
{% endif %}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
{% if cookiecutter.include_audit %}
using {{ cookiecutter.assembly_name}}.Core.Security;
{% endif %}


namespace {{cookiecutter.assembly_name }}.Data.Abstractions.Entity;

public record Sample
{
   {% if cookiecutter.database == "PostgreSql" %}
{% include 'templates/abstractions/postgresql_sample.cs' %}
{% elif cookiecutter.database == "MongoDb" %}
{% include 'templates/abstractions/mongodb_sample.cs' %}
{% endif %}
}