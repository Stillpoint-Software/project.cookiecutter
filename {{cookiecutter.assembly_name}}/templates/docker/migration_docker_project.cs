  <ItemGroup>
    <Compile Remove="logs\**" />
    <Compile Remove="Properties\**" />
    <EmbeddedResource Remove="logs\**" />
    <EmbeddedResource Remove="Properties\**" />
    <None Remove="logs\**" />
    <None Remove="Properties\**" />
  </ItemGroup>
  <ItemGroup>
    <None Remove="appsettings.json" />
    <None Remove="appsettings.Production.json" />
    <None Remove="appsettings.Staging.json" />
{% if cookiecutter.database == "PostgreSql" %}
    <None Remove="Resources\1000-Initial\CreateUsers.sql" />
{% elif cookiecutter.database == "MongoDb" %}
    <None Remove="Resources\1000-Initial\administration\users\user.json" />
{% endif %}
  </ItemGroup>

  <ItemGroup>
    <Content Include="appsettings.json">
       <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="appsettings.Production.json">
       <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
       <ExcludeFromSingleFile>true</ExcludeFromSingleFile>
       <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
    <Content Include="appsettings.Staging.json">
       <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
       <ExcludeFromSingleFile>true</ExcludeFromSingleFile>
       <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
  </ItemGroup>
  <ItemGroup>
{% if cookiecutter.database == "PostgreSql" %}
    <EmbeddedResource Include="Resources\1000-Initial\CreateUsers.sql">
       <CopyToOutputDirectory>Never</CopyToOutputDirectory>
    </EmbeddedResource>
{% elif cookiecutter.database == "MongoDb" %}
    <EmbeddedResource Include="Resources\1000-Initial\administration\users\user.json" />
{% endif %}
  </ItemGroup>
  <ItemGroup>
{% if cookiecutter.include_azure == "yes" %}
    <PackageReference Include="Azure.Extensions.AspNetCore.Configuration.Secrets" Version="1.3.2" />
    <PackageReference Include="Azure.Identity" Version="1.13.1" />
    <PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.7.0" />
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.21.0" />
{% endif %}
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Primitives" Version="9.0.2" />
    <PackageReference Include="Hyperbee.Migrations" Version="2.0.1" />
{% if cookiecutter.database == "PostgreSql" %}
    <PackageReference Include="Npgsql" Version="9.0.3" />
    <PackageReference Include="Hyperbee.Migrations.Providers.Postgres" Version="2.0.1" />
{% elif cookiecutter.database == "MongoDb" %}
    <PackageReference Include="MongoDb.Driver" Version="3.0.0" />
    <PackageReference Include="Hyperbee.Migrations.Providers.MongoDB" Version="2.0.0" />
{% endif %}
    <PackageReference Include="System.Configuration.ConfigurationManager" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration.CommandLine" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.2" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.2" />
    <PackageReference Include="Serilog" Version="4.2.0" />
    <PackageReference Include="Serilog.Extensions.Hosting" Version="9.0.0" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="9.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageReference Include="Serilog.Formatting.Compact" Version="3.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}\{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.csproj" />
{% if cookiecutter.include_aspire == "yes" %}
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.ServiceDefaults\{{cookiecutter.assembly_name}}.ServiceDefaults.csproj" />
{% endif %}
  </ItemGroup>
  <ItemGroup>
    <PackageReference Update="Microsoft.SourceLink.GitHub" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>