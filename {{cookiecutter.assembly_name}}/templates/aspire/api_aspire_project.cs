  <ItemGroup>
    <PackageReference Include="Asp.Versioning.Mvc" Version="8.1.0" />
    <PackageReference Include="Aspire.Azure.Security.KeyVault" Version="9.1.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.3" />
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.21.2" />
    {% if cookiecutter.database =="PostgreSql" %}
    <PackageReference Include="Npgsql" Version="9.0.3" />
    {% endif %}
    <PackageReference Include="Serilog" Version="4.2.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
    <PackageReference Include="Serilog.Enrichers.ClientInfo" Version="2.1.2" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="9.0.0" />
    <PackageReference Include="Serilog.Sinks.Async" Version="2.1.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.3.1" />
    <PackageReference Include="Cronos" Version="0.9.0" />
    <PackageReference Include="FluentValidation" Version="11.11.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="Hyperbee.Extensions.DependencyInjection" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Extensions.Lamar" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Pipeline" Version="2.0.1" />
    <PackageReference Include="Lamar" Version="14.0.1" />
    <PackageReference Include="Lamar.Microsoft.DependencyInjection" Version="14.0.1" />
    {% if cookiecutter.include_oauth == "yes" %}
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.2" />
    {% endif %}
    <PackageReference Include="System.Linq.Async" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.3" />
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