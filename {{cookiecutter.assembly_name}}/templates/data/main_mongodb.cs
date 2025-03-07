{% if cookiecutter.database == "PostgreSql" %}
   <PackageReference Include="AspNetCore.HealthChecks.NpgSql" Version="9.0.0" />
   <PackageReference Include="Npgsql" Version="9.0.2" />
{% elif cookiecutter.database == "MongoDb" %}
   <PackageReference Include="MongoDb.Driver" Version="3.2.0" />
{% endif %