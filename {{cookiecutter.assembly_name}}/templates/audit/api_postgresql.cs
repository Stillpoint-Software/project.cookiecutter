  optionsBuilder.UseNpgsql( connectionString );
  _dbContext = new DatabaseContext( optionsBuilder.Options );

Audit.Core.Configuration
        .Setup()
        .UsePostgreSql( config => config
            .ConnectionString( connectionString )
            .Schema("{{cookiecutter.database_name}}")
            .TableName( "audit_event" )
            .IdColumnName( "event_id" )
            .LastUpdatedColumnName( "last_updated" )
            .DataColumn( "data", DataType.JSONB, ev => ev.ToJson() )
            .CustomColumn( "event_type", ev => ev.EventType ) );