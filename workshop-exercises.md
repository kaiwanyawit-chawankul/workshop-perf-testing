# Workshop Exercises: API Performance Testing

Welcome to the **API Performance Testing Workshop**!
This document contains **step-by-step exercises** you can follow to practice.

---

## 🏁 Prerequisites

* .NET 9.0 or higher installed
* [k6](https://k6.io/docs/get-started/installation/) installed
* Demo API running:

  ```bash
  dotnet run --project ./src/DemoApi.AppHost/DemoApi.AppHost.csproj
  ```

API is available at `http://localhost:5000/api/demo`

---

## Exercise 1: Load Test a Fast Endpoint

1. Start the API.
2. Run the **load test** against `/fast`:

   ```bash
   k6 run tests/load-test.js
   ```
3. Observe:

   * Average response time
   * Requests per second
   * Any failed requests

📌 **Goal:** Understand how the API behaves with normal traffic.

---

## Exercise 2: Stress Test a Slow Endpoint

1. Run the **stress test** against `/slow`:

   ```bash
   k6 run tests/stress-test.js
   ```
2. Observe:

   * Response times increasing as load grows
   * Failure rate when API can’t handle load

📌 **Goal:** Identify the breaking point where performance drops.

---

## Exercise 3: Soak Test for Stability

1. Run the **soak test** for 10 minutes:

   ```bash
   k6 run tests/soak-test.js
   ```
2. Observe:

   * Memory usage of the API (Task Manager / `dotnet-counters`)
   * Whether response time stays stable

📌 **Goal:** Detect long-term degradation (e.g., slow memory leaks).

---

## Exercise 4: Spike Test a Memory Leak

1. Run the **spike test** against `/leak`:

   ```bash
   k6 run tests/spike-test.js
   ```
2. Observe:

   * API memory usage climbing rapidly
   * Number of requests completed before crash (if any)

📌 **Goal:** See how the API reacts to sudden load + memory pressure.

---

## Exercise 5: Debug & Fix

1. Run `/leak` multiple times → notice memory keeps growing.
2. Discuss fixes:

   * Release allocated memory instead of storing it
   * Use caching strategies or limit memory usage
   * Add cancellation tokens to slow endpoints

📌 **Goal:** Practice identifying and fixing performance bottlenecks.

---

## Exercise 6: CPU Test

1. Run a load test against `/cpu`:

   ```bash
   k6 run tests/load-test.js
   ```

   (Update the URL in the script to `/cpu`)
2. Observe:

   * CPU usage on your machine (`htop` or Task Manager)
   * Whether requests slow down as CPU saturates

📌 **Goal:** Learn how CPU-bound tasks can affect throughput.

---

## Exercise 7: Database Connection Pool & Static Connection

### 7.1 Setup

1. Configure a **low max pool size** for `/db-leak`:

```json
"ConnectionStrings": {
  "apidb": "Host=localhost;Port=5432;Database=apidb;Username=postgres;Password=postgres;Maximum Pool Size=10"
}
```

2. Start the API:

```bash
dotnet run
```

---

### 7.2 Test `/db-leak` (Connection Leak)

```bash
k6 run tests/db-pool-test.js
```

* **Observation:** After enough concurrent requests:

```
TimeoutException: The connection pool has been exhausted
```

* **Goal:** Understand how forgetting to dispose connections can exhaust the pool.

---

### 7.3 Test `/db-static` (Static/Global Connection)

* Works fine for single requests, but fails under **concurrent load**.

**Run concurrent load test:**

```javascript
import http from "k6/http";
import { check } from "k6";

export let options = {
  stages: [
    { duration: "5s", target: 50 },  // ramp up 50 users
    { duration: "20s", target: 50 }, // sustain load
    { duration: "5s", target: 0 },   // ramp down
  ],
};

export default function () {
  const res = http.get("http://localhost:5000/api/demo/db-static");
  check(res, { "status 200": (r) => r.status === 200 });
}
```

* **Observation:** You may see errors like:

```
InvalidOperationException: There is already an open DataReader associated with this Command
```

* **Goal:** Demonstrate that static/global connections are unsafe in multithreaded applications.

---

### 7.4 Test `/db-ok` (Good Pattern)

* Compare with the proper endpoint:

```bash
k6 run tests/db-pool-test.js
```

* **Observation:** Stable under load; no connection leaks or concurrency errors.

---

## Exercise 8: Redis Caching vs Slow DB

1. Call `/db-slow`:
   - Every request takes ~3 seconds (simulates a slow query).
2. Call `/db-cached`:
   - First request is slow (~3s).
   - Subsequent requests are instant (served from Redis).
3. Run a k6 load test against both endpoints:
   ```bash
   k6 run tests/load-test.js
   # Update URL inside script to /db-slow and /db-cached

---

✅ **Key takeaway:**

* `/db-leak` → pool exhaustion from not disposing connections
* `/db-static` → concurrency issues from shared connection
* `/db-ok` → proper per-request connection handling

---

✅ **By completing these exercises, you’ll understand how to:**

* Simulate **real-world traffic patterns**
* Detect performance bottlenecks
* Debug issues like **latency spikes**, **memory leaks**, and **DB pool exhaustion**
* Improve API resilience under load
