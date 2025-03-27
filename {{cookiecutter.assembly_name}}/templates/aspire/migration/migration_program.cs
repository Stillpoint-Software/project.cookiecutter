using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};
using  {{cookiecutter.assembly_name}}.ServiceDefaults;
{% if cookiecutter.database =="PostgreSql" %}
using Hyperbee.Migrations.Providers.Postgres;
{% elif cookiecutter.database =="MongoDb" %}
using Hyperbee.Migrations.Providers.MongoDB;
using cookieAspire.Migrations.Extensions;
using Microsoft.Extensions.DependencyInjection;
{% endif %}


namespace  {{cookiecutter.assembly_name}}.Migrations;
public class Program
{
    public static void Main( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();


        // Manually invoke Startup's ConfigureServices
        var startupInstance = new Startup( builder.Configuration );
        startupInstance.ConfigureServices( builder.Services );

        {% if cookiecutter.database== "PostgreSql"%}
        builder.AddNpgsqlDbContext<SampleContext>( "projectdb" ); // this allows for telemetry
        {% elif cookiecutter.database== "MongoDb"%}
         //mongodb here
        builder.AddMongoDBClient(  "{{cookiecutter.database_name}}" );
        {% endif %}

        //Setup OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithTracing( tracing => tracing.AddSource( MainService.ActivitySourceName ) );

        // Add environment variables and user secrets to configuration
        builder.Configuration
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>( optional: true );

        //Connection string for projectdb from aspire
        var connectionString = builder.Configuration["ConnectionStrings:{{cookiecutter.database_name}}"];

        if (string.IsNullOrEmpty( connectionString ))
        {
            throw new ArgumentNullException( nameof( connectionString ), "Connection string for '{{cookiecutter.database_name}}' is not configured." );
        }

        //This line is needed to run migrations.  However, this doesn't allow for telemetry
        {% if cookiecutter.database =="PostgreSql" %}
        builder.Services.AddNpgsqlDataSource( connectionString );
        builder.Services.AddPostgresMigrations();
        {% elif cookiecutter.database =="MongoDb" %}
        builder.Services.AddMongoDbMigrations( builder.Configuration );
        {% endif %}
        builder.Services.AddHostedService<MainService>();
        builder.Services.AddDataProtection();

        // Build the application
        var app = builder.Build();

        // Call Startup's Configure method to configure the middleware pipeline
        startupInstance.Configure( app, app.Environment );

        // Run the application
        app.Run();
    }
}