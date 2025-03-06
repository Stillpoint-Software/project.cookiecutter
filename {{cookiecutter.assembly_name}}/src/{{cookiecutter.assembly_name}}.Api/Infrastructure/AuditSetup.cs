using Audit.Core;
using Audit.PostgreSql.Configuration;
using Microsoft.EntityFrameworkCore;
using {{cookiecutter.assembly_name}}.Data.Abstractions;
using {{cookiecutter.assembly_name}}.Data.Postgres;
using System.Reflection;

namespace {{cookiecutter.assembly_name}}.Api.Infrastucture;

public static class AuditSetup
{
    private static {{cookiecutter.assembly_name}}Context _dbContext;

    public static void ConfigureAudit( WebApplicationBuilder builder )
    {
        var connectionString = builder.Configuration.GetConnectionString( "projectdb" );

        var optionsBuilder = new DbContextOptionsBuilder<MedstarContext>();
        optionsBuilder.UseNpgsql( connectionString );
        _dbContext = new {{cookiecutter.assembly_name}}Context( optionsBuilder.Options );

        {% if cookiecutter.database =="PostgreSql"%}
        Configuration
        .Setup()
        .UsePostgreSql( config => config
            .ConnectionString( connectionString )
            .Schema( {{cookiecutter.assembly_name}} )
            .TableName( "audit_event" )
            .IdColumnName( "event_id" )
            .LastUpdatedColumnName( "last_updated" )
            .DataColumn( "data", DataType.JSONB, ev => ev.ToJson() )
            .CustomColumn( "event_type", ev => ev.EventType ) );
        {% elif cookiecutter.database =="MongoDb"%}
          Configuration
        .Setup()
        .UseMongoDB( config => config
            .ConnectionString( connectionString )
            .Database( "mongoDb" )
            .Collection( "audit_event" ) );
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
                if (auditEvent.Target.Old != null)
                {
                    var propValue = property.GetValue( auditEvent.Target.Old );
                    if (propValue != null)
                    {
                        {% if cookiecutter.database =="PostgreSql"%}
                        var oldData = Convert.ToBase64String( _dbContext.EncryptData( propValue.ToString() ?? string.Empty ) );
                        property.SetValue( auditEvent.Target.Old, oldData );
                        {% elif cookiecutter.database =="MongoDb"%}
                        property.SetValue( auditEvent.Target.Old, SecurityHelper.EncryptValue( propValue.ToString() ?? string.Empty ) );
                        {% endif %
                    }
                }
                var newValue = property.GetValue( auditEvent.Target.New );
                if (newValue != null)
                {
                     {% if cookiecutter.database =="PostgreSql"%}
                    var newData = Convert.ToBase64String( _dbContext.EncryptData( newValue.ToString() ?? string.Empty ) );
                    property.SetValue( auditEvent.Target.New, newData );
                    {% elif cookiecutter.database =="MongoDb"%}
                    property.SetValue( auditEvent.Target.New, SecurityHelper.DecryptValue( newValue.ToString() ?? string.Empty ) );
                    {% endif %
                }
            }
        }
    }
}
