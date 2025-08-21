using DemoApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();




// Get connection string from Aspire
var connectionString = builder.Configuration.GetConnectionString("apidb");
Console.WriteLine($"Using connection string: {connectionString}");

builder.Services.AddDbContext<DemoDbContext>(options =>
options.UseNpgsql(connectionString));


var app = builder.Build();

// 🔽 Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
    db.Database.Migrate(); // 👈 This line applies migrations at startup
}


app.MapGet("/messages", async (DemoDbContext db) =>
await db.Messages.ToListAsync());

app.MapPost("/messages", async (DemoDbContext db, Message msg) =>
{
    db.Messages.Add(msg);
    await db.SaveChangesAsync();
    return Results.Created($"/messages/{msg.Id}", msg);
});

LoadTestEndpoints.Map(app);
app.Run("http://0.0.0.0:5000");
