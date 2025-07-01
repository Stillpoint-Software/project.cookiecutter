#define CONTAINER_DIAGNOSTICS

using System.Globalization;
{% if cookiecutter.include_oauth == "yes" %}
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
{% endif %}
using Microsoft.IdentityModel.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using { { cookiecutter.assembly_name} }.Api.Validators;
using { { cookiecutter.assembly_name} }.Extensions;
using { { cookiecutter.assembly_name} }.Middleware;
using Hyperbee.Extensions.Lamar;
using Hyperbee.Pipeline;
using Lamar;
{% if cookiecutter.include_azure == "yes" %}
using Microsoft.ApplicationInsights.Extensibility.Implementation;
{% endif %}
using Microsoft.AspNetCore.Http.Json;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
{% if cookiecutter.database == "MongoDb" %}
using MongoDB.Driver;
{% endif %}

namespace {{cookiecutter.assembly_name }};

{% if cookiecutter.include_azure == "yes" %}
{% include '../templates/main/main_startup_azure.cs' %}
{% else %}
{% include '../templates/main/main_startup.cs' %}
{% endif %}