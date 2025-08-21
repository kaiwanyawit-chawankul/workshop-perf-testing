using System.Diagnostics;
using DemoApi;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRedisClient(connectionName: "cache");

// Get connection string from Aspire
var connectionString = builder.Configuration.GetConnectionString("apidb");
var redisConnectionString = builder.Configuration.GetConnectionString("cache");

// Append pool size if not already present
if (!connectionString.Contains("Maximum Pool Size"))
{
    connectionString += ";Maximum Pool Size=10";
}

Console.WriteLine($"Using connection string: {connectionString}");

// Add Redis
IConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync($"{redisConnectionString}");

builder.Services.AddSingleton(redis);
builder.Services.AddStackExchangeRedisCache(options =>
    options.ConnectionMultiplexerFactory = () => Task.FromResult(redis)
);

builder.Services.AddDbContext<DemoDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// 🔽 Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
    db.Database.Migrate(); // 👈 This line applies migrations at startup
}

// this is a demo API for testing purposes
app.MapGet(
    "api/demo/pid",
    () =>
    {
        var pid = Process.GetCurrentProcess().Id;
        return Results.Ok(new { pid });
    }
);

app.MapGet("api/demo/messages", async (DemoDbContext db) => await db.Messages.ToListAsync());

app.MapPost(
    "api/demo/messages",
    async (DemoDbContext db, Message msg) =>
    {
        db.Messages.Add(msg);
        await db.SaveChangesAsync();
        return Results.Created($"api/demo/messages/{msg.Id}", msg);
    }
);
IOBoundedEndpoints.Map(app, connectionString, redis);
LoadTestEndpoints.Map(app);
MemoryLeakEndpoints.Map(app);
app.Run("http://0.0.0.0:5000");
