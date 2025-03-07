using Audit.Core;
using Audit.PostgreSql.Configuration;
using Microsoft.EntityFrameworkCore;
using {{cookiecutter.assembly_name}}.Data.Abstractions;
using {{cookiecutter.assembly_name}}.Data.Postgres;
using System.Reflection;

namespace {{cookiecutter.assembly_name}}.Api.Infrastructure;

public static class AuditSetup
{
    private static {{cookiecutter.assembly_name}}Context _dbContext;

    public static void ConfigureAudit( WebApplicationBuilder builder )
    {
        var connectionString = builder.Configuration.GetConnectionString( "projectdb" );

        var optionsBuilder = new DbContextOptionsBuilder<MedstarContext>();
        optionsBuilder.UseNpgsql( connectionString );
        _dbContext = new {{cookiecutter.assembly_name}}Context( optionsBuilder.Options );

        {% if cookiecutter.database =="PostgreSql" %}
            {% include '/templates/audit/api_postgresql.cs' %}
        {% endif %}
        {% if cookiecutter.database =="MongoDb" %}
            {% include '/templates/audit/api_mongodb.cs' %}
        {% endif %}

        Configuration.AddOnSavingAction( scope =>
        {
            if (scope.Event is ListAuditEvent auditEvent)
            {
                var auditList = auditEvent.List
                    .Cast<object>()
                    .Select( item => new ListAuditModel
                    {
                        Id = (int) item.GetType().GetProperty( "Id" )!.GetValue( item )!
                    } )
                    .ToList();

                auditEvent.List = auditList;
            }

            if (scope.Event.Target?.Type == null || scope.Event.Target?.New == null)
            {
                return;
            }

            SetSecuredProperties( scope.Event, _dbContext );
        } );
    }

    private static void SetSecuredProperties( AuditEvent auditEvent, MedstarContext _dbContext )
    {
        var secureProperties = auditEvent.Target.New?.GetType()
            .GetProperties( BindingFlags.Public | BindingFlags.Instance )
            .Where( p => p.GetCustomAttribute<Secure>() != null );
        if (secureProperties != null)
        {
            foreach (var property in secureProperties)
            {
                {%if cookiecutter.database =='PostgreSql' %}
                    {% include '/templates/audit/api_security_postgresql.cs' %}
                {% endif %}
                {%if cookiecutter.database =='MongoDb' %}
                    {% include '/templates/audit/api_security_mongodb.cs' %}
                {% endif %}
            }
        }
    }
}
