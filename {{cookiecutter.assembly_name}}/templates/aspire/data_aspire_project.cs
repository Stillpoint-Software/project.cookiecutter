<ItemGroup>
    {% if cookiecutter.database == "PostgreSql" %}
    <PackageReference Include="Aspire.Npgsql" Version="9.1.0" />
    <PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.1.0" />
    {% endif %}
    <PackageReference Include="System.Text.Json" Version="9.0.3" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.Abstractions\{{cookiecutter.assembly_name}}.Data.Abstractions.csproj" />
  </ItemGroup>