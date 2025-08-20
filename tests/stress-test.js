import http from 'k6/http';

export let options = {
    stages: [
        { duration: '10s', target: 10 },
        { duration: '20s', target: 50 },
        { duration: '20s', target: 100 },
        { duration: '10s', target: 0 },
    ],
};

export default function () {
    http.get('http://localhost:5000/api/demo/slow');
}
