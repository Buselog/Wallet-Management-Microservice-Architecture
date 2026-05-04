import { useState } from 'react';
import { message } from 'antd';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { handleApiError } from '../utils/errorHandler';
import { useTranslation } from 'react-i18next';

export const useAuth = (form) => {
    const { t } = useTranslation();
    const [loading, setLoading] = useState(false);
    const [generalError, setGeneralError] = useState(null);
    const navigate = useNavigate();

    const login = async (values) => {
        setLoading(true);
        setGeneralError(null);
        try {
            const response = await api.post('/Auth/login', values);
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('customerNo', response.data.customerNo);
            message.success(t('auth_login_success'));
            navigate('/dashboard');
        } catch (error) {
            handleApiError(error, form, setGeneralError);
        } finally {
            setLoading(false);
        }
    };

    const register = async (values) => {
        setLoading(true);
        setGeneralError(null);
        try {
            await api.post('/Auth/register', values);
            message.success(t('auth_register_success'));
            navigate('/login');
        } catch (error) {
            handleApiError(error, form, setGeneralError);
        } finally {
            setLoading(false);
        }
    };

    return { loading, generalError, login, register };
};