import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
    vus: 20,            // 20 users
    duration: '30s',    // for 30 seconds
};

export default function () {
    http.get('http://localhost:5000/api/demo/fast');
    sleep(1);
}
