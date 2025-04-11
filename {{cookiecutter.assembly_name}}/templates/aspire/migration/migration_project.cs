  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
<ItemGroup>
{% if cookiecutter.database == "PostgreSql" %}
    <None Remove="Resources\1000-Initial\CreateUsers.sql" />
{% elif cookiecutter.database == "MongoDb" %}
    <None Remove="Resources\1000-Initial\administration\users\user.json" />
{% endif %}
  </ItemGroup>
  <ItemGroup>

{% if cookiecutter.database =="PostgreSql" %}
    <PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.2.0" />
    <PackageReference Include="Hyperbee.Migrations.Providers.Postgres" Version="2.0.2" />
    <PackageReference Include="Npgsql" Version="9.0.3" />
{% elif cookiecutter.database == "MongoDb" %}
    <PackageReference Include="Hyperbee.Migrations.Providers.MongoDB" Version="2.0.2" />
    <PackageReference Include="Aspire.Hosting.MongoDB" Version="9.2.0" />
    <PackageReference Include="Aspire.MongoDB.Driver.v3" Version="9.2.0" />
{% endif %}

{% if cookiecutter.include_azure == 'yes'%}
    <PackageReference Include="Azure.Identity" Version="1.13.2" />
{% endif %}
    <PackageReference Include="Hyperbee.Migrations" Version="2.0.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.4">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.4" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.csproj" />
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.ServiceDefaults\{{cookiecutter.assembly_name}}.ServiceDefaults.csproj" />
  </ItemGroup>