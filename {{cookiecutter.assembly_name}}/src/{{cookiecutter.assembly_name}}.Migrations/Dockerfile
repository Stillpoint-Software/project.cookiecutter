#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/{{cookiecutter.assembly_name}}.Migrations/{{cookiecutter.assembly_name}}.Migrations.csproj", "src/{{cookiecutter.assembly_name}}.Migrations/"]
RUN dotnet restore "./src/{{cookiecutter.assembly_name}}.Migrations/{{cookiecutter.assembly_name}}.Migrations.csproj"
COPY . .
WORKDIR "/src/src/{{cookiecutter.assembly_name}}.Migrations"
RUN dotnet build "./{{cookiecutter.assembly_name}}.Migrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./{{cookiecutter.assembly_name}}.Migrations.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "{{cookiecutter.assembly_name}}.Migrations.dll"]