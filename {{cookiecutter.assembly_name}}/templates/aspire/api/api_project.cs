<Project Sdk="Microsoft.NET.Sdk.Web">
 <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  {% if cookiecutter.include_azure =="yes" %}
    <ItemGroup>
    <Compile Remove="Vault\AzureKeyService.cs" />
  </ItemGroup>
  {% endif %}
  <ItemGroup>
    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
      <_Parameter1>$(AssemblyName).Tests</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Asp.Versioning.Mvc" Version="8.1.0" />
    {%if cookiecutter.include_audit == "yes"%}
    <PackageReference Include="Audit.NET" Version="27.5.2" />
    {% endif %}
     {% if cookiecutter.database =="PostgreSql" %}
    <PackageReference Include="Audit.NET.PostgreSql" Version="27.5.2" />
    {% elif cookiecutter.database == "MongoDb" %}
    <PackageReference Include="Audit.NET.MongoDB" Version="27.5.2" />
    {% endif %} 
    <PackageReference Include="Aspire.Azure.Security.KeyVault" Version="9.2.0" />
    <PackageReference Include="Aspire.Azure.Storage.Blobs" Version="9.2.0" />
    <PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.7.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.4" />
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.21.2" />
    {% if cookiecutter.database =="PostgreSql" %}
    <PackageReference Include="Npgsql" Version="9.0.3" />
    {% elif cookiecutter.database =="MongoDb" %}
    <PackageReference Include="Aspire.MongoDB.Driver.v3" Version="9.2.0" />
    {% endif %}
    <PackageReference Include="Serilog" Version="4.2.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
    <PackageReference Include="Serilog.Enrichers.ClientInfo" Version="2.1.2" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="9.0.0" />
    <PackageReference Include="Serilog.Sinks.Async" Version="2.1.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="8.1.1" />
    <PackageReference Include="Cronos" Version="0.10.0" />
    <PackageReference Include="FluentValidation" Version="11.11.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="Hyperbee.Extensions.DependencyInjection" Version="2.0.3" />
    <PackageReference Include="Hyperbee.Extensions.Lamar" Version="2.0.3" />
    <PackageReference Include="Hyperbee.Pipeline" Version="2.0.2" />
    <PackageReference Include="Lamar" Version="14.0.1" />
    <PackageReference Include="Lamar.Microsoft.DependencyInjection" Version="14.0.1" />
    {% if cookiecutter.include_oauth == "yes" %}
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.4" />
    {% endif %}
    <PackageReference Include="System.Linq.Async" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.4" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.4" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.4" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.Abstractions\{{cookiecutter.assembly_name}}.Data.Abstractions.csproj" />
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.csproj" />
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.ServiceDefaults\{{cookiecutter.assembly_name}}.ServiceDefaults.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Update="Microsoft.SourceLink.GitHub" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Scripting" Version="4.13.0" />
  </ItemGroup>
  </Project>