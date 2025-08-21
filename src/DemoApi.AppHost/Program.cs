var builder = DistributedApplication.CreateBuilder(args);

// var db = builder.AddPostgres("postgres")
//     .WithDataVolume() // persists data between runs
//     .AddDatabase("apidb"); // creates a database called "apidb"

var postgres = builder.AddPostgres("postgres");

var databaseName = "apidb";

var db = postgres.AddDatabase(databaseName);

// Register API project and give it the DB connection string
var api = builder.AddProject<Projects.DemoApi>("demoapi")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();