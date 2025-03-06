<PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove="Controllers\**" />
    <Content Remove="Controllers\**" />
    <EmbeddedResource Remove="Controllers\**" />
    <None Remove="Controllers\**" />
  </ItemGroup>

  <ItemGroup>
    <Compile Remove="WeatherForecast.cs" />
  </ItemGroup>
  <ItemGroup>
    <EmbeddedResource Include="Resources\1000-Initial\CreateTables.sql" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSql" Version="9.1.0" />
    <PackageReference Include="Azure.Identity" Version="1.13.2" />
    <PackageReference Include="Hyperbee.Migrations" Version="2.0.1" />
    <PackageReference Include="Hyperbee.Migrations.Providers.Postgres" Version="2.0.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.2" />
    <PackageReference Include="Npgsql" Version="9.0.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\NewAspireAppAudit.Data.Postgres\NewAspireAppAudit.Data.Postgres.csproj" />
    <ProjectReference Include="..\NewAspireAppAudit.ServiceDefaults\NewAspireAppAudit.ServiceDefaults.csproj" />
  </ItemGroup>