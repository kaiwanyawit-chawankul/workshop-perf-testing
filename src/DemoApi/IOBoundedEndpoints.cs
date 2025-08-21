using Npgsql;
public static class IOBoundedEndpoints
{
    public static void Map(WebApplication app, string connectionString)
    {
        app.MapGet("/db-leak", async () =>
        {
            // BAD: Connection not disposed, pool leaks
            var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            // Simulate a long-running query
            await Task.Delay(5000);

            using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
            var result = await cmd.ExecuteScalarAsync();

            return Results.Ok(result);
        });

        app.MapGet("/db-ok", async () =>
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
            var result = await cmd.ExecuteScalarAsync();

            return Results.Ok(result);
        });
    }
}