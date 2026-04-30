import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:5000/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

api.interceptors.request.use((config) => {
    const lang = localStorage.getItem('lang') || 'tr-TR';
    config.headers['Accept-Language'] = lang;
    return config;
});

export default api;