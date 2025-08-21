# API Performance Testing Workshop

This repository is a hands-on workshop for learning **API performance testing**.
We’ll build a demo API in C# and test it with performance testing tools.

---

## 🚀 What You’ll Learn

- How to simulate **different API behaviors** (fast, slow, memory leaks).
- How to run **load, stress, soak, and spike tests**.
- How to detect and fix performance bottlenecks.

---

## 📂 Project Structure

- `src/DemoApi` → C# ASP.NET Core Web API with test endpoints.
- `tests/` → Performance test scripts (k6).
- `workshop-exercises.md` → Guided exercises.

---

## 🔧 Setup

1. Clone this repo:

   ```bash
   git clone https://github.com/yourname/api-performance-workshop.git
   cd api-performance-workshop
   ```

2. Run the API:
   ```bash
    cd src/DemoApi
    dotnet run
   ```

The API will be available at http://localhost:5000/api/demo

3. Install [k6](https://k6.io/docs/get-started/installation/)

## 🏃 Running Tests

Load Test:

```bash
k6 run tests/load-test.js
```

Stress Test:

```bash
k6 run tests/stress-test.js
```

Soak Test:

```bash
k6 run tests/soak-test.js
```

Spike Test:

```bash
k6 run tests/spike-test.js
```

## 📘 Exercises

See [workshop-exercises.md](workshop-exercises.md) for step-by-step practice:

1. Run a load test on /fast
1. Run a stress test on /slow
1. Run a spike test on /leak
1. Detect memory usage in API
1. Fix bottlenecks and re-test

## 🚀 How to check

See [how-to-check.md](how-to-check.md) for tools and steps.

