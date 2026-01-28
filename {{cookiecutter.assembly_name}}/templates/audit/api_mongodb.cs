Audit.Core.Configuration
        .Setup()
        .UseMongoDB( config => config
            .ConnectionString( connectionString )
            .Database("{{cookiecutter.database_name | lower}}")
            .Collection( "audit_event" ));