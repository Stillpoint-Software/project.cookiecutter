  Configuration
        .Setup()
        .UseMongoDB( config => config
            .ConnectionString( connectionString )
            .Database( "mongoDb" )
            .Collection( "audit_event" ) 
            .SerializeAsBson( true ) );