< Project Sdk = "Microsoft.NET.Sdk" >
 < PropertyGroup >
    < TargetFramework > net9.0 </ TargetFramework >
    < ImplicitUsings > enable </ ImplicitUsings >
  </ PropertyGroup >
  < ItemGroup >
    < AssemblyAttribute Include = "System.Runtime.CompilerServices.InternalsVisibleTo" >
      < _Parameter1 >$(AssemblyName).Tests </ _Parameter1 >
    </ AssemblyAttribute >
  </ ItemGroup >
  <>
    < PackageReference Include = "Asp.Versioning.Mvc" Version = "8.1.0" />
    {% if cookiecutter.include_audit == "yes" %}
    < PackageReference Include = "Audit.NET" Version = "30.0.2" />
      {% if cookiecutter.database == "PostgreSql" %}
      < PackageReference Include = "Audit.NET.PostgreSql" Version = "30.0.2 />
      {% elif cookiecutter.database == "MongoDb" %}
      < PackageReference Include = "Audit.MongoClient" Version = "30.0.2" />
      < PackageReference Include = "Audit.NET.MongoDB" Version = "30.0.2" />
      {% endif %}
{% endif %}
{% if cookiecutter.include_azure == "yes" %}
    < PackageReference Include = "Azure.Security.KeyVault.Keys" Version = "9.3.1" />
    < PackageReference Include = "Azure.Storage.Blobs" Version = 9.3.1" />
    < PackageReference Include = "Microsoft.ApplicationInsights" Version = "2.23.0" />
    < PackageReference Include = "Serilog.Sinks.ApplicationInsights" Version = "4.0.0" />
    < PackageReference Include = "Serilog.Sinks.AzureBlobStorage" Version = "4.0.5" />
    {% endif %}
    < PackageReference Include = "Serilog.AspNetCore.Enrichers.CorrelationId" Version = "1.0.0" />
    < PackageReference Include = "Serilog.Formatting.Compact" Version = "3.0.0" />
    < PackageReference Include = "Serilog.Settings.AppSettings" Version = "3.0.0" />
    < PackageReference Include = "Serilog.Settings.Configuration" Version = "9.0.0" />
    < PackageReference Include = "Serilog.Sinks.Console" Version = "6.0.0" />
    < PackageReference Include = "Serilog.Sinks.File" Version = "7.0.0" />
    < PackageReference Include = "Cronos" Version = "0.11.0" />
    < PackageReference Include = "FluentValidation" Version = "12.0.0" />
    < PackageReference Include = "Hyperbee.Extensions.DependencyInjection" Version = "2.0.3" />
    < PackageReference Include = "Hyperbee.Extensions.Lamar" Version = "2.0.3" />
    < PackageReference Include = "Hyperbee.Pipeline" Version = "2.0.2" />
    < PackageReference Include = "Lamar" Version = "15.0.0" />
    < PackageReference Include = "Lamar.Microsoft.DependencyInjection" Version = "15.0.0" />
    < PackageReference Include = "Microsoft.OpenApi" Version = "1.6.24" />
    < PackageReference Include = "Swashbuckle.AspNetCore.SwaggerGen" Version = "9.0.1" />
    {% if cookiecutter.include_oauth == "yes" %}
    < PackageReference Include = "Microsoft.AspNetCore.Authentication.JwtBearer" Version = "9.0.6" />
    {% endif %}
    < PackageReference Include = "System.Linq.Async" Version = "6.0.3" />
    < PackageReference Include = "Microsoft.Extensions.Http" Version = "9.0.6" />
    < PackageReference Include = "Microsoft.Extensions.Logging" Version = "9.0.6" />
    < PackageReference Include = "Microsoft.Extensions.Logging.Abstractions" Version = "9.0.6" />
    < PackageReference Include = "System.Text.Encodings.Web" Version = "9.0.6" />
  </ ItemGroup >
  < ItemGroup >
    < ProjectReference Include = "..\{{cookiecutter.assembly_name}}.Data.Abstractions\{{cookiecutter.assembly_name}}.Data.Abstractions.csproj" />
    < ProjectReference Include = "..\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.csproj" />
  </ ItemGroup >
  < ItemGroup >
    < PackageReference Update = "Microsoft.SourceLink.GitHub" Version = "8.0.0" >
    < PrivateAssets > all </ PrivateAssets >
    < IncludeAssets > runtime; build; native; contentfiles; analyzers; buildtransitive </ IncludeAssets >
    </ PackageReference >
    < PackageReference Include = "Microsoft.CodeAnalysis.CSharp.Scripting" Version = "4.14.0" />
  </ ItemGroup >
  </ Project >