# 🧠 Monitoring CPU & Memory Usage on macOS

When testing endpoints like `/cpu` or `/leak`, it’s important to monitor how your app behaves under load. On a Mac, you’ve got several built-in and third-party tools to track CPU and memory usage.

---

## 🔍 GUI Tools

### 1. **Activity Monitor** (built-in)

* Open Spotlight (`Cmd + Space`) → type **Activity Monitor**
* Go to the **CPU** tab:
  - Find your running process (e.g. `DemoApi`)
  - Watch the **% CPU** value climb when `/cpu` is under load
* Switch to the **Memory** tab:
  - See how much memory your app is using
  - Look at **Memory Pressure** at the bottom for overall system usage

---

## 🔍 CLI Tools

### 2. **top** (built-in)

```bash
top -o cpu
````

* Sorts by CPU usage
* Look for your `dotnet` process
* `%CPU` will spike during load testing

To monitor memory:

```bash
top -o rsize
```

* `RSIZE` = resident memory (actual RAM used)
* `VSIZE` = virtual memory

---

### 3. **[htop](https://learn.microsoft.com/en-us/troubleshoot/developer/webapps/aspnetcore/practice-troubleshoot-linux/3-2-task-managers-top-htop)** (nicer CLI experience)

```bash
brew install htop
htop
```

* Press `1` to view per-core CPU usage
* Use `/` to search for `DemoApi` or `dotnet`
* View the **RES** column to monitor memory
* Use `F6` to sort by memory or CPU

---

### 4. **[dotnet-counters](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters)** (real-time .NET metrics)

```bash
dotnet tool install --global dotnet-counters
dotnet-counters monitor --process-id <PID>
```

* Replace `<PID>` with your API’s process ID
* Real-time display of:

  * CPU %
  * GC activity
  * Allocations
  * Thread pool usage

---

#### ✅ Grab the PID quickly:

```bash
ps aux | grep dotnet
```

---

## 🔍 .NET-Specific Memory Monitoring

### 5. **dotnet-counters** (again – for memory too!)

```bash
dotnet-counters monitor --process-id <PID>
```

Useful memory metrics:

* `gc-heap-size` → managed heap size
* `gen-0`, `gen-1`, `gen-2` collections → GC frequency
* `assembly-count` → track if assemblies are leaking

---

### 6. **dotnet-gcdump** (for memory leaks & snapshots)

If you suspect a memory leak:

```bash
dotnet tool install --global dotnet-gcdump
dotnet-gcdump collect -p <PID> -o dump.gcdump
```

* Analyze `dump.gcdump` in Visual Studio or JetBrains Rider
* Helps investigate retained objects, large allocations, etc.

---

## ✅ Summary: What to Watch

| Test Endpoint | What to Look For       | Tool Suggestions                  |
| ------------- | ---------------------- | --------------------------------- |
| `/cpu`        | CPU spikes             | Activity Monitor, top, htop       |
| `/leak`       | Steady memory growth   | Activity Monitor, dotnet-counters, gcdump           |
