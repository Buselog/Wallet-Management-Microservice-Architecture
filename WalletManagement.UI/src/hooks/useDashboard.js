import { useState, useCallback, useEffect } from 'react';
import api from '../services/api';
import { message, Modal } from 'antd';
import { useNavigate } from 'react-router-dom';

export const useDashboard = (form) => {
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
            message.error("Veriler güncellenemedi.");
        } finally {
            setLoading(false);
        }
    }, [navigate]);

    useEffect(() => { fetchDashboardData(); }, [fetchDashboardData]);

    const createWallet = async (values, onSuccess) => {
        try {
            await api.post('/Wallet/create', values);
            Modal.success({
                title: 'İşlem Başarılı',
                content: 'Yeni cüzdanınız başarıyla oluşturuldu.',
                onOk: () => {
                    onSuccess();
                    form.resetFields();
                    fetchDashboardData();
                }
            });
        } catch (error) {
            const errorMsg = error.response?.data?.Message || "Cüzdan oluşturulamadı.";
            Modal.error({ title: 'Hata', content: errorMsg });
        }
    };

    const deleteWallet = async (id) => {
        try {
            await api.delete(`/Wallet/${id}/delete`);
            message.success("Cüzdan başarıyla kapatıldı.");
            fetchDashboardData();
        } catch (error) {
            message.warning("Silme işlemi başarısız.");
        }
    };

    return { data, loading, rates, createWallet, deleteWallet, fetchDashboardData };
};