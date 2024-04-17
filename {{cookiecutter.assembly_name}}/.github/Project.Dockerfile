# DOCKERFILE used in Github workflows for deployment
# must be located in the root solution folder

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/{{cookiecutter.assembly_name}}/{{cookiecutter.assembly_name}}.csproj", "src/{{cookiecutter.assembly_name}}/"]
COPY ["src/{{cookiecutter.assembly_name}}.Api/{{cookiecutter.assembly_name}}.Api.csproj", "src/{{cookiecutter.assembly_name}}.Api/"]
COPY ["src/{{cookiecutter.assembly_name}}.Data.Abstractions/{{cookiecutter.assembly_name}}.Data.Abstractions.csproj", "src/{{cookiecutter.assembly_name}}.Data.Abstractions/"]
COPY ["src/{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}/GlyphEditor.Data.{{cookiecutter.database}}.csproj", "src/{{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}}/"]
COPY ["tests/{{cookiecutter.assembly_name}}.Tests/{{cookiecutter.assembly_name}}.Tests.csproj", "tests/{{cookiecutter.assembly_name}}.Tests/"]
COPY ["tests/{{cookiecutter.assembly_name}}.Tests/Resources/", "tests/{{cookiecutter.assembly_name}}.Tests/Resources/"]

RUN dotnet nuget add source "https://www.myget.org/F/lpoint/auth/01429c3b-aacd-4dba-8fba-74cc294ed537/api/v3/index.json" -n "MyGet"
COPY . .

# run the unit tests
RUN dotnet restore "/src/tests/{{cookiecutter.assembly_name}}.Tests/{{cookiecutter.assembly_name}}.Tests.csproj"

COPY . .
RUN dotnet build  "/src/tests/{{cookiecutter.assembly_name}}.Tests/{{cookiecutter.assembly_name}}.Tests.csproj" -c Release -o /app/build
RUN dotnet test "/src/tests/{{cookiecutter.assembly_name}}.Tests/{{cookiecutter.assembly_name}}.Tests.csproj" --logger "trx;LogFileName=testresults.trx"

RUN dotnet restore "src/{{cookiecutter.assembly_name}}/{{cookiecutter.assembly_name}}.csproj" 

# build the application
WORKDIR "/src/src/{{cookiecutter.assembly_name}}"
RUN dotnet build "{{cookiecutter.assembly_name}}.csproj" -c Release -o /app/build 

FROM build AS publish
RUN dotnet publish "{{cookiecutter.assembly_name}}.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "{{cookiecutter.assembly_name}}.dll"]