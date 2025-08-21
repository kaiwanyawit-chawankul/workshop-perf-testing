import http from "k6/http";
import { check } from "k6";

export let options = {
  stages: [
    { duration: "10s", target: 10 },
    { duration: "10s", target: 100 }, // sudden spike
    { duration: "10s", target: 0 },
  ],
};

export default function () {
  const res = http.get("http://localhost:5000/api/demo/leak");
  check(res, { "status 200": (r) => r.status === 200 });
}
