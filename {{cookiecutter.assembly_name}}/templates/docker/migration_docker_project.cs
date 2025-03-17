  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <UserSecretsId>f0c20361-65af-4d55-8907-0e4f3d6e22c9</UserSecretsId>
    <Configurations>Debug;Release;Staging;Debug_api;Release_api;Debug_migration;Release_migration</Configurations>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
    <DockerfileContext>..\..</DockerfileContext>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Staging|AnyCPU'">
    <Optimize>True</Optimize>
  </PropertyGroup>

{% if cookiecutter.database == "PostgreSql" %}
  <ItemGroup>
    <None Remove="Resources\1000-Initial\CreateUsers.sql" />
  </ItemGroup>
  <ItemGroup>
    <EmbeddedResource Include="Resources\1000-Initial\CreateUsers.sql" />
  </ItemGroup>
{% elif cookiecutter.database == "MongoDb" %}
 <ItemGroup>
    <None Remove="Resources\1000-Initial\administration\users\user.json" />
  </ItemGroup>
{% endif %}
    <ItemGroup>
{% if cookiecutter.include_azure == "yes" %}
    <PackageReference Include="Azure.Extensions.AspNetCore.Configuration.Secrets" Version="1.4.0" />
    <PackageReference Include="Azure.Identity" Version="1.13.2" />
    <PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.7.0" />
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.21.2" />
{% endif %}
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Primitives" Version="9.0.3" />
    <PackageReference Include="Hyperbee.Migrations" Version="2.0.1" />
{% if cookiecutter.database == "PostgreSql" %}
    <PackageReference Include="Npgsql" Version="9.0.3" />
    <PackageReference Include="Hyperbee.Migrations.Providers.Postgres" Version="2.0.1" />
{% elif cookiecutter.database == "MongoDb" %}
    <PackageReference Include="MongoDb.Driver" Version="3.0.0" />
    <PackageReference Include="Hyperbee.Migrations.Providers.MongoDB" Version="2.0.0" />
{% endif %}
    <PackageReference Include="System.Configuration.ConfigurationManager" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Configuration.CommandLine" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.3" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.3" />
    <PackageReference Include="Serilog" Version="4.2.0" />
    <PackageReference Include="Serilog.Extensions.Hosting" Version="9.0.0" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="9.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageReference Include="Serilog.Formatting.Compact" Version="3.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Update="Microsoft.SourceLink.GitHub" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>