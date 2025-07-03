using System.Reflection;
using Audit.Core;
{% if cookiecutter.database == "PostgreSql" %}
using Audit.PostgreSql.Configuration;
{% elif cookiecutter.database == "MongoDb" %}
using Audit.MongoDB.Providers;
using {{ cookiecutter.assembly_name }}.Data.Abstractions;
{% endif %}

using {{ cookiecutter.assembly_name }}.Data.{{ cookiecutter.database }};
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using {{ cookiecutter.assembly_name }}.Data.Abstractions;

namespace {{cookiecutter.assembly_name }}.Api.Infrastructure;

public static class AuditSetup
{
    private static  SampleContext _dbContext;

    public static void ConfigureAudit(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("{{cookiecutter.databaseName}}");

        var optionsBuilder = new DbContextOptionsBuilder<SampleContext>();
        {% if cookiecutter.database == "PostgreSql" %}
        {% include 'templates/audit/api_postgresql.cs' %}
        {% endif %}
            {% if cookiecutter.database == "MongoDb" %}
            {% include 'templates/audit/api_mongodb.cs' %}
            {% endif %}

                Configuration.AddOnSavingAction(scope =>
                {
                    if (scope.Event is ListAuditEvent auditEvent)
                    {
                        var auditList = auditEvent.List
                            .Cast<object>()
                            .Select(item => new ListAuditModel
                            {

                       {% if cookiecutter.database == "PostgreSql" %}
                    Id = (int)item.GetType().GetProperty("Id")!.GetValue(item)!
        
                                {% elif cookiecutter.database == "MongoDb" %}
                    Id = (string)item.GetType().GetProperty("Id")!.GetValue(item)!
        
                                {% endif %}
                })
                            .ToList();

                auditEvent.List = auditList;
            }

            if (scope.Event.Target?.Type == null || scope.Event.Target?.New == null)
            {
                return;
            }

            SetSecuredProperties(scope.Event, _dbContext);
        } );
    }

    private static void SetSecuredProperties(AuditEvent auditEvent, SampleContext _dbContext)
    {
        var secureProperties = auditEvent.Target.New?.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<Secure>() != null);
        if (secureProperties != null)
        {
            foreach (var property in secureProperties)
            {
                {%if cookiecutter.database == 'PostgreSql' %}
                {% include 'templates/audit/api_security_postgresql.cs' %}
                {% endif %}
                    {%if cookiecutter.database == 'MongoDb' %}
                    {% include 'templates/audit/api_security_mongodb.cs' %}
                    {% endif %}
                    }
                }
            }
        }
