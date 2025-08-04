var dbUsername = builder.AddParameter("DbUser", "mongodb", true);
var dbPassword = builder.AddParameter("DbPassword", "mongodb", secret: true);

var dbServer = builder.AddMongoDB("mongo", userName: dbUsername, password: dbPassword)
                .WithMongoExpress()
                .PublishAsConnectionString()
                .WithDataVolume();

if (builder.Environment.IsDevelopment())
{
    dbServer.WithLifetime(ContainerLifetime.Persistent);
}