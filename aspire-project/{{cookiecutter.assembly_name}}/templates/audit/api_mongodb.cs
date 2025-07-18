Audit.Core.Configuration
        .Setup()
        .UseMongoDB( config => config
            .ConnectionString( connectionString )
            .Database("{{cookiecutter.database_name}}")
            .Collection( "audit_event" )
            .SerializeAsBson( true ) );