using System.Data;
using {{cookiecutter.assembly_name}}.Data.Abstractions.Entity;
using Microsoft.EntityFrameworkCore;
{%- if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.Bson.Serialization.Conventions;
{%- endif %}

namespace {{cookiecutter.assembly_name }}.Data.{{cookiecutter.database }}
;

public class DatabaseContext : DbContext
{
    {%- if cookiecutter.include_audit and cookiecutter.database =="PostgreSql" %}
    private readonly string encryptionKey = "mysecretkey"; // use azure key vault for this
    {%- endif %}

public DbSet<Sample> Samples { get; set; }

public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
{
}

{%- if cookiecutter.database == "PostgreSql" %}
{% include 'templates/audit/data_postgresql_encryption.cs' %}
{% elif cookiecutter.database == "MongoDb" %}
{% include 'templates/data/mongodb_context.cs' %}
{%- endif %}
}
