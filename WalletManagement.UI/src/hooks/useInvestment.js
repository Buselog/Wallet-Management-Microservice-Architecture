import { useState, useCallback, useEffect } from 'react';
import { notification } from 'antd';
import api from '../services/api';
import dayjs from 'dayjs';

export const useInvestment = (form) => {
    const [rates, setRates] = useState([]);
    const [wallets, setWallets] = useState([]);
    const [loading, setLoading] = useState(false);
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedRate, setSelectedRate] = useState(null);
    const [tradeType, setTradeType] = useState('BUY');
    const [userData, setUserData] = useState(null);
    const [lastUpdate, setLastUpdate] = useState('---');

    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const [ratesRes, dashRes] = await Promise.all([
                api.get('/InvestmentRate/rates'),
                api.get('/v1/Dashboard/summary')
            ]);

            setRates(ratesRes.data || []);
            setUserData(dashRes.data);
            setWallets(dashRes.data.wallets || []);

            const apiDate = ratesRes.data?.[0]?.lastUpdatedDate;
            setLastUpdate(apiDate ? dayjs(apiDate).format('HH:mm:ss') : dayjs().format('HH:mm:ss'));
        } catch (error) {
            notification.error({
                message: 'Veri Hatası',
                description: 'Piyasa verileri şu an ulaşılamıyor.',
                placement: 'topRight'
            });
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => { fetchData(); }, [fetchData]);

    const openTrade = (record, type) => {
        setTradeType(type);
        setSelectedRate(record);
        form.resetFields();
        setModalVisible(true);
    };

    const executeTrade = async (values) => {
        setLoading(true);
        try {
            const endpoint = tradeType === 'BUY' ? '/Trade/buy' : '/Trade/sell';
            const payload = {
                ...values,
                currencyCode: selectedRate.currencyCode,
                tradeType: tradeType
            };

            await api.post(endpoint, payload);
            notification.success({
                message: 'İşlem Başarılı',
                description: `${values.amount} ${selectedRate.currencyCode} işlemi başarıyla tamamlandı.`
            });
            setModalVisible(false);
            fetchData();
        } catch (error) {
            const errorData = error.response?.data;
            notification.error({
                message: 'İşlem Başarısız',
                description: errorData?.Message || errorData?.message || 'İşlem gerçekleştirilemedi.'
            });
        } finally {
            setLoading(false);
        }
    };

    return {
        rates, wallets, loading, modalVisible, setModalVisible,
        selectedRate, tradeType, userData, lastUpdate,
        fetchData, openTrade, executeTrade
    };
};