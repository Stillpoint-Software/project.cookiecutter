﻿  <ItemGroup>
    {% if cookiecutter.include_azure == "yes" %}
    <PackageReference Include="Azure.ResourceManager" Version="1.13.0" />
    {% endif %}
    <PackageReference Include="Hyperbee.Extensions.DependencyInjection" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Extensions.Lamar" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Pipeline" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Resources" Version="2.0.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.2" />
    {% if cookiecutter.database == "PostgreSql" %}
    <PackageReference Include="AspNetCore.HealthChecks.NpgSql" Version="9.0.0" />
    <PackageReference Include="Npgsql" Version="9.0.2" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.Postgresql" Version="9.0.4" />
    {% elif cookiecutter.database == "MongoDb" %}
    <PackageReference Include="MongoDb.Driver" Version="3.2.0" />
    <PackageReference Include="AspNetCore.HealthChecks.MongoDb" Version="9.0.0" />
    {% endif %}
    <PackageReference Include="Portable.BouncyCastle" Version="1.9.0" />
    <PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.2" />
  </ItemGroup>
  <ItemGroup>
    <Folder Include="Models\" />
  </ItemGroup> 
  <ItemGroup>
  <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.Abstractions\{{cookiecutter.assembly_name}}.Data.Abstractions.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Update="Microsoft.SourceLink.GitHub" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup> 
