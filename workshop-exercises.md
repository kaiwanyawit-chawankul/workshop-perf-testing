Perfect 👍 let’s add a **`workshop-exercises.md`** so your repo looks like a real hands-on training package.

---

### 📘 `workshop-exercises.md`

````markdown
# Workshop Exercises: API Performance Testing

Welcome to the **API Performance Testing Workshop**!
This document contains **step-by-step exercises** you can follow to practice.

---

## 🏁 Prerequisites
- .NET 6+ installed
- [k6](https://k6.io/docs/get-started/installation/) installed
- Demo API running:
  ```bash
  cd src/DemoApi
  dotnet run
````

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

   * Memory usage of the API (check Task Manager / `dotnet-counters`)
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

   * Release allocated memory instead of storing it.
   * Use caching strategies or limit memory usage.
   * Add cancellation tokens to slow endpoints.

📌 **Goal:** Practice identifying and fixing performance bottlenecks.

---

## Exercise 6: CPU Test
1. Run a load test against `/cpu`:

   ```bash
   k6 run tests/load-test.js
   ```
(Update the URL in the script to /cpu)
2. Observe:

CPU usage on your machine (htop or Task Manager).

Whether requests slow down as CPU saturates.

📌 **Goal:** Learn how CPU-bound tasks can affect throughput.

---
## Bonus Challenges

* Add a new endpoint `/cpu` that performs heavy calculations.
* Write a k6 script to benchmark `/cpu` under load.
* Add metrics dashboards (Prometheus + Grafana).
* Automate performance tests in CI/CD pipeline (GitHub Actions).

---

✅ By completing these exercises, you’ll understand how to:

* Simulate **real-world traffic patterns**
* Detect performance bottlenecks
* Debug issues like **latency spikes** and **memory leaks**
* Improve API resilience under load

