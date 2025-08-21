using Npgsql;

namespace DemoApi;

public static class IOBoundedEndpoints
{
    public static void Map(WebApplication app, string connectionString)
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
