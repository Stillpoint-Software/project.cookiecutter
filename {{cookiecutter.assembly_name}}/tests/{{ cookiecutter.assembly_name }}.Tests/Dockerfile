#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["*.sln", "."]
COPY ["tests/{{cookiecutter.assembly_name}}.Tests/{{cookiecutter.assembly_name}}.Tests.csproj", "tests/{{cookiecutter.assembly_name}}.Tests/"]
COPY ["src/{{cookiecutter.assembly_name}}.Api/{{cookiecutter.assembly_name}}.Api.csproj", "src/{{cookiecutter.assembly_name}}.Api/"]
COPY ["src/{{cookiecutter.assembly_name}}.Data.Abstractions/{{cookiecutter.assembly_name}}.Data.Abstractions.csproj", "src/{{cookiecutter.assembly_name}}.Data.Abstractions/"]
COPY ["src/{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}/{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}.csproj", "src/{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}/"]
RUN dotnet restore "/src/tests/{{cookiecutter.assembly_name}}.Tests/{{cookiecutter.assembly_name}}.Tests.csproj"
WORKDIR "/src/tests/{{cookiecutter.assembly_name}}.Tests"

ENTRYPOINT ["dotnet", "test", "--logger:trx"]