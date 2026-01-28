public static DatabaseContext Create(IMongoDatabase database) =>
     new(new DbContextOptionsBuilder<DatabaseContext>()
                                     .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                                     .Options);

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Sample>(entity =>
         {
             entity.ToCollection("sample");
             entity.HasKey(e => e.Id);

             entity.HasIndex(e => e.Name);
             entity.HasIndex(e => new { e.Name, e.Description });
         });
}

protected override void ConfigureConventions(ModelConfigurationBuilder configBuilder)
{
    //To use camel case field names in the serialized document
    var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
    ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
}