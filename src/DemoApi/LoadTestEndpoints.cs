namespace DemoApi;

public static class LoadTestEndpoints
{
    public static void Map(WebApplication app)
    {
        List<byte[]> memoryLeakList = new();

        app.MapGet(
            "/api/demo/fast",
            () =>
            {
                return Results.Ok(new { message = "Fast response", timestamp = DateTime.UtcNow });
            }
        );

        app.MapGet(
            "/api/demo/slow",
            () =>
            {
                Thread.Sleep(5000); // simulate slow work
                return Results.Ok(new { message = "Slow response", timestamp = DateTime.UtcNow });
            }
        );

        app.MapGet(
            "/api/demo/leak",
            () =>
            {
                // Simulate memory leak
                var leak = new byte[10_000_000]; // 10 MB
                memoryLeakList.Add(leak);
                return Results.Ok(
                    new { message = "Allocated memory chunk", count = memoryLeakList.Count }
                );
            }
        );

        app.MapGet(
            "/api/demo/cpu",
            () =>
            {
                // CPU intensive calculation (naive Fibonacci)
                int n = 40;
                long Fib(int x) => x <= 1 ? x : Fib(x - 1) + Fib(x - 2);

                var result = Fib(n);
                return Results.Ok(
                    new { message = $"Fibonacci({n}) = {result}", timestamp = DateTime.UtcNow }
                );
            }
        );
    }
}
