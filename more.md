dotnet tool install --global dotnet-counters

dotnet-counters monitor --process-id 848 --counters System.Runtime



Press p to pause, r to resume, q to quit.
  Status: Running

[System.Runtime]
    CPU Usage (%)                                 24
    GC Heap Size (MB)                            811

https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters
https://learn.microsoft.com/en-us/troubleshoot/developer/webapps/aspnetcore/practice-troubleshoot-linux/3-2-task-managers-top-htop

https://learn.microsoft.com/en-us/dotnet/core/diagnostics/built-in-metrics-runtime
