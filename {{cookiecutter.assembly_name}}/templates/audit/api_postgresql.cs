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