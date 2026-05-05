import { useState, useCallback, useEffect } from 'react';
import { notification } from 'antd';
import api from '../services/api';
import dayjs from 'dayjs';
import { handleApiError } from '../utils/errorHandler';
import { useTranslation } from 'react-i18next';

export const useInvestment = (form) => {
    const { t } = useTranslation();
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
            handleApiError(error, null, (msg) => {
                notification.error({
                    message: t('inv_error_msg'),
                    description: msg,
                    placement: 'topRight'
                });
            });
        } finally {
            setLoading(false);
        }
    }, [t]);

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
                message: t('inv_success_msg'),
                description: t('inv_success_desc', { amount: values.amount, code: selectedRate.currencyCode })
            });
            setModalVisible(false);
            fetchData();
        } catch (error) {
            handleApiError(error, null, (msg) => {
                notification.error({
                    message: t('inv_error_msg'),
                    description: msg
                });
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