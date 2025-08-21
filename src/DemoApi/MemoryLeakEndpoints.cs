using System.Diagnostics;
namespace DemoApi;
public static class MemoryLeakEndpoints
{
    // Persisted across requests so memory "leaks"
    private static readonly List<byte[]> _leak = new();
    private static long _totalBytes; // track without O(n) Sum()

    public static void Map(WebApplication app)
    {
        // Allocates 100 MB per call by default (override with ?mb=###)
        app.MapGet(
            "api/demo/leak",
            (int? mb) =>
            {
                int mbToAllocate = mb.GetValueOrDefault(100); // default 100 MB
                // allocate in 1 MB chunks to hit LOH and be predictable
                for (int i = 0; i < mbToAllocate; i++)
                {
                    var chunk = new byte[1024 * 1024]; // 1 MB
                    _leak.Add(chunk);
                    _totalBytes += chunk.Length;
                }

                return Results.Ok(
                    new
                    {
                        allocatedThisCallMB = mbToAllocate,
                        totalLeakedMB = _totalBytes / (1024 * 1024),
                        pid = Process.GetCurrentProcess().Id,
                    }
                );
            }
        );

        // Clear the leak between exercises
        app.MapPost(
            "api/demo/leak/clear",
            () =>
            {
                var clearedMB = _totalBytes / (1024 * 1024);
                _leak.Clear();
                _totalBytes = 0;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                return Results.Ok(new { clearedMB, nowLeakedMB = 0 });
            }
        );
    }
}
