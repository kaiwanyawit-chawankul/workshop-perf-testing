import http from "k6/http";
import { check } from "k6";

export let options = {
  duration: "10m",
  vus: 10,
};

export default function () {
  const res = http.get("http://localhost:5000/api/demo/fast");
  check(res, { "status 200": (r) => r.status === 200 });
}
