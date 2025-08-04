using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
{% if cookiecutter.database == "PostgreSql" %}
using {{ cookiecutter.assembly_name }}.Data.PostgreSql;
using Testcontainers.PostgreSql;
{% elif cookiecutter.database == "MongoDb" %}
using Testcontainers.MongoDb;
{% endif %}

namespace {{cookiecutter.assembly_name }}.Tests;

[TestClass]
public class InitializeTestContainer
{

}
