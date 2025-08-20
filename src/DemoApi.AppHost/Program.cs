var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.DemoApi>("demoapi");
builder.Build().Run();
