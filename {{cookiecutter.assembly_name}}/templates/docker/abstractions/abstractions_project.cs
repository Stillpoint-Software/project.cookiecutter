  <ItemGroup>
    <PackageReference Include="Hyperbee.Json" Version="3.0.2" />
    <PackageReference Include="Hyperbee.Pipeline" Version="2.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.4" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.4" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.4" />
    {% if cookiecutter.database == "MongoDb" %}
    <PackageReference Include="MongoDB.Bson" Version="3.3.0" />
    <PackageReference Include="MongoDB.Driver" Version="3.3.0" />
    {% endif %}
  </ItemGroup>
  <ItemGroup>
    <PackageReference Update="Microsoft.SourceLink.GitHub" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>