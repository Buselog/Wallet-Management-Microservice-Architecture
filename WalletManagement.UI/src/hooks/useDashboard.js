import { useState, useCallback, useEffect } from 'react';
import api from '../services/api';
import { message, Modal, notification } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { handleApiError } from '../utils/errorHandler';

export const useDashboard = (form) => {
    const { t } = useTranslation();
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [rates, setRates] = useState([]);
    const navigate = useNavigate();

    const fetchDashboardData = useCallback(async () => {
        try {
            setLoading(true);
            const [summaryRes, ratesRes] = await Promise.all([
                api.get('/v1/Dashboard/summary'),
                api.get('/InvestmentRate/rates')
            ]);
            setData(summaryRes.data);
            setRates(ratesRes.data || []);
        } catch (error) {
            if (error.response?.status === 401) navigate('/login');

            handleApiError(error, null, (msg) => {
                message.error(msg || t('dash_fetch_error'));
            });
        } finally {
            setLoading(false);
        }
    }, [navigate, t]);

    useEffect(() => { fetchDashboardData(); }, [fetchDashboardData]);

    const createWallet = async (values, onSuccess) => {
        try {
            await api.post('/Wallet/create', values);
            Modal.success({
                title: t('dash_create_success_title'),
                content: t('dash_create_success_content'),
                onOk: () => {
                    onSuccess();
                    form.resetFields();
                    fetchDashboardData();
                }
            });
        } catch (error) {
            handleApiError(error, null, (msg) => {
                Modal.error({
                    title: t('dash_create_error_title'),
                    content: msg || t('dash_create_error_default')
                });
            });
        }
    };

    const deleteWallet = async (id) => {
        try {
            await api.delete(`/Wallet/${id}/delete`);
            message.success(t('dash_delete_success'));
            fetchDashboardData();
        } catch (error) {
            handleApiError(error, null, (msg) => {
                message.warning(msg || t('dash_delete_error'));
            });
        }
    };

    return { data, loading, rates, createWallet, deleteWallet, fetchDashboardData };
};