var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres");

var databaseName = "apidb";

var db = postgres.AddDatabase(databaseName);

// Register API project and give it the DB connection string
var api = builder
    .AddProject<Projects.DemoApi>("demoapi")
    .WithReference(cache)
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
