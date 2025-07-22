public static DatabaseContext Create(IMongoDatabase database) =>
     new(new DbContextOptionsBuilder<DatabaseContext>()
                                     .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                                     .Options);

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    var sampleIndex = new CreateIndexModel<Sample>(Builders<Sample>.IndexKeys
                           .Ascending(x => x.Name)
                           .Ascending(x => x.Description));

    modelBuilder.Entity<Sample>().ToCollection("sample");
}

protected override void ConfigureConventions(ModelConfigurationBuilder configBuilder)
{
    //To use camel case field names in the serialized document
    var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
    ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
}