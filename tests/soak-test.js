import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
    vus: 10,
    duration: '10m',
};

export default function () {
    http.get('http://localhost:5000/api/demo/fast');
    sleep(1);
}
