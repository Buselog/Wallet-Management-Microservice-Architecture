import axios from 'axios';
import i18n from 'i18next';

const api = axios.create({
    baseURL: 'https://localhost:5000/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

api.interceptors.request.use((config) => {
    const currentLang = i18n.language || 'tr';
    config.headers['Accept-Language'] = currentLang === 'tr' ? 'tr-TR' : 'en-US';

    const token = localStorage.getItem('token');
    if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
});

export default api;