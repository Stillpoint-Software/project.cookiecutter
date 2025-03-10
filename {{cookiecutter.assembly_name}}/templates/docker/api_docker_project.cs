  <ItemGroup>
    <PackageReference Include="Asp.Versioning.Mvc" Version="8.1.0" />
    {% if cookiecutter.include_azure == "yes" %}
    <PackageReference Include="Azure.Security.KeyVault.Keys" Version="4.7.0" />
    <PackageReference Include="Azure.Storage.Blobs" Version="12.23.0" />
    <PackageReference Include="Serilog.Sinks.AzureBlobStorage" Version="4.0.5" />
    {% endif %}
    <PackageReference Include="Cronos" Version="0.9.0" />
    <PackageReference Include="FluentValidation" Version="11.11.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="Hyperbee.Extensions.DependencyInjection" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Extensions.Lamar" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Pipeline" Version="2.0.1" />
    <PackageReference Include="Lamar" Version="14.0.1" />
    <PackageReference Include="Lamar.Microsoft.DependencyInjection" Version="14.0.1" />
    <PackageReference Include="Microsoft.OpenApi" Version="1.6.23" />
    <PackageReference Include="Swashbuckle.AspNetCore.SwaggerGen" Version="7.3.1" />
    {% if cookiecutter.include_oauth == "yes" %}
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.2" />
    {% endif %}
    <PackageReference Include="System.Linq.Async" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.2" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.Abstractions\{{cookiecutter.assembly_name}}.Data.Abstractions.csproj" />
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Update="Microsoft.SourceLink.GitHub" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Scripting" Version="4.13.0" />
  </ItemGroup>