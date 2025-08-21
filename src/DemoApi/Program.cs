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
var redis = ConnectionMultiplexer.Connect($"{redisConnectionString}");

builder.Services.AddDbContext<DemoDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// 🔽 Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
    db.Database.Migrate(); // 👈 This line applies migrations at startup
}

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
app.Run("http://0.0.0.0:5000");
