﻿using Hyperbee.Migrations.Providers.Postgres;
using {{cookiecutter.assembly_name}}.Data.{{cookiecutter.database}};
using  {{cookiecutter.assembly_name}}.ServiceDefaults;

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


        builder.AddNpgsqlDbContext<MedstarContext>( "medstardb" ); // this allows for telemetry

        //Setup OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithTracing( tracing => tracing.AddSource( MainService.ActivitySourceName ) );

        // Add environment variables and user secrets to configuration
        builder.Configuration
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>( optional: true );

        //Connection string for medstarDb from aspire
        var connectionString = builder.Configuration["ConnectionStrings:medstarDb"];

        if (string.IsNullOrEmpty( connectionString ))
        {
            throw new ArgumentNullException( nameof( connectionString ), "Connection string for 'medstarDb' is not configured." );
        }

        //This line is needed to run migrations.  However, this doesn't allow for telemetry
        builder.Services.AddNpgsqlDataSource( connectionString );
        builder.Services.AddPostgresMigrations();
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