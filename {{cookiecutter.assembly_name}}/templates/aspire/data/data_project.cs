< ItemGroup >
    {% if cookiecutter.database == "PostgreSql" %}
    < PackageReference Include = "Aspire.Npgsql" Version = "9.3.1" />
    < PackageReference Include = "Aspire.Npgsql.EntityFrameworkCore.PostgreSQL" Version = "9.3.1" />
    {% elif cookiecutter.database == "MongoDb" %}
    < PackageReference Include = "MongoDB.EntityFrameworkCore" Version = "9.0.0" />
    {% endif %}
    < PackageReference Include = "System.Text.Json" Version = "9.0.6" />
  </ ItemGroup >
  < ItemGroup >
    < ProjectReference Include = "..\{{cookiecutter.assembly_name}}.Data.Abstractions\{{cookiecutter.assembly_name}}.Data.Abstractions.csproj" />
  </ ItemGroup >