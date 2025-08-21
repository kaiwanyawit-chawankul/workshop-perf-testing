using DemoApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Get connection string from Aspire
var connectionString = builder.Configuration.GetConnectionString("apidb");

// Append pool size if not already present
if (!connectionString.Contains("Maximum Pool Size"))
{
    connectionString += ";Maximum Pool Size=10";
}

Console.WriteLine($"Using connection string: {connectionString}");

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
IOBoundedEndpoints.Map(app, connectionString);
LoadTestEndpoints.Map(app);
app.Run("http://0.0.0.0:5000");
