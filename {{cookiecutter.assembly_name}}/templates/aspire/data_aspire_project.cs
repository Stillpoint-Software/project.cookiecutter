 <ItemGroup>
    
    {% if cookiecutter.database =="PostgreSql"%}
    <PackageReference Include="Aspire.Npgsql" Version="9.1.0" />
    <PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.Postgresql" Version="9.1.0" />
    {% elif cookiecutter.database =="MongoDB"%}
    //TO DO
    {% endif %}

    <PackageReference Include="System.Text.Json" Version="9.0.2" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\{{cookiecutter.assembly_name}}.Data.Abstractions\{{cookiecutter.assembly_name}}.Data.Abstractions.csproj" />
  </ItemGroup>
  <ItemGroup>
    <Folder Include="Models\" />
  </ItemGroup>
