var dbPassword = builder.AddParameter("DbPassword", "postgres", true);
var dbUser = builder.AddParameter("DbUser", "postgres", true);

var dbServer = builder.AddPostgres("postgres", userName: dbUser, password: dbPassword)
    .PublishAsConnectionString()
    .WithDataVolume()
    .WithPgAdmin(x => x.WithImageTag("9.5"));
