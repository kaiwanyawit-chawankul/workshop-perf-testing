using Npgsql;
using StackExchange.Redis;

namespace DemoApi;

public static class IOBoundedEndpoints
{
    public static void Map(WebApplication app, string connectionString, IConnectionMultiplexer redis)
    {
        // ❌ BAD: Connection not disposed, pool leaks over time
        app.MapGet(
            "api/demo/db-leak",
            async () =>
            {
                var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                // Simulate a short delay while still leaking connection
                await Task.Delay(2000);

                using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
                var result = await cmd.ExecuteScalarAsync();

                return Results.Ok(result);
            }
        );

        // ❌ BAD: Static/global connection, shared across requests
        app.MapGet(
            "api/demo/db-static",
            async () =>
            {
                // WARNING: Using static connection is unsafe in multithreaded apps
                return Results.Ok(await StaticConnection.GetNow(connectionString));
            }
        );

        // ✅ GOOD: Properly opens and disposes per request
        app.MapGet(
            "api/demo/db-ok",
            async () =>
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
                var result = await cmd.ExecuteScalarAsync();

                return Results.Ok(result);
            }
        );

        var cache = redis.GetDatabase();

        // 🐢 Slow DB query
        app.MapGet(
            "api/demo/db-slow",
            async () =>
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                await Task.Delay(3000);
                using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
                var result = await cmd.ExecuteScalarAsync();
                return Results.Ok(new { Source = "DB", Value = result });
            }
        );

        // ⚡ Cached DB query
        app.MapGet(
            "api/demo/db-cached",
            async () =>
            {
                var cacheKey = "now-value";
                var cached = await cache.StringGetAsync(cacheKey);
                if (cached.HasValue)
                {
                    return Results.Ok(new { Source = "Cache", Value = cached.ToString() });
                }

                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                await Task.Delay(3000);
                using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
                var result = (await cmd.ExecuteScalarAsync())?.ToString();

                await cache.StringSetAsync(cacheKey, result, TimeSpan.FromSeconds(10));

                return Results.Ok(new { Source = "DB", Value = result });
            }
        );
    }
}

public static class StaticConnection
{
    private static NpgsqlConnection? _conn;

    public static async Task<object?> GetNow(string connectionString)
    {
        _conn ??= new NpgsqlConnection(connectionString);
        if (_conn.State != System.Data.ConnectionState.Open)
            await _conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT NOW()", _conn);
        return await cmd.ExecuteScalarAsync();
    }
}
