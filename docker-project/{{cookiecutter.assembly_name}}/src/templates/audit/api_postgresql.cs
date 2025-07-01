  optionsBuilder.UseNpgsql( connectionString );
  _dbContext = new SampleContext( optionsBuilder.Options ); 
 
 Configuration
        .Setup()
        .UsePostgreSql( config => config
            .ConnectionString( connectionString )
            .Schema( "sample"  )
            .TableName( "audit_event" )
            .IdColumnName( "event_id" )
            .LastUpdatedColumnName( "last_updated" )
            .DataColumn( "data", DataType.JSONB, ev => ev.ToJson() )
            .CustomColumn( "event_type", ev => ev.EventType ) );