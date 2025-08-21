import http from "k6/http";
import { check } from "k6";

export let options = {
  stages: [
    { duration: "5s", target: 50 },
    { duration: "20s", target: 50 },
    { duration: "5s", target: 0 },
  ],
};

export default function () {
  // Uncomment one endpoint at a time

  // BAD: leaks connections
  // const res = http.get("http://localhost:5000/api/demo/db-leak");

  // BAD: static/global connection
  const res = http.get("http://localhost:5000/api/demo/db-static");

  // GOOD: properly disposed
  // const res = http.get("http://localhost:5000/api/demo/db-ok");

  check(res, { "status 200": (r) => r.status === 200 });
}
