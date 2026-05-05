import { useState, useCallback, useEffect } from 'react';
import { notification } from 'antd';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import dayjs from 'dayjs';
import { handleApiError } from '../utils/errorHandler';
import { useTranslation } from 'react-i18next';

export const useTransaction = (form) => {
    const { t, i18n } = useTranslation();
    const navigate = useNavigate();
    const [wallets, setWallets] = useState([]);
    const [loading, setLoading] = useState(false);
    const [selectedWallet, setSelectedWallet] = useState(null);
    const [actionType, setActionType] = useState('deposit');
    const [userData, setUserData] = useState(null);
    const [lastTransactionDate, setLastTransactionDate] = useState(null);

    const fetchLastTransaction = async (walletId) => {
        try {
            const res = await api.get(`/WalletTransaction/history/${walletId}`, {
                params: { pageNumber: 1, pageSize: 1 }
            });
            const lastItem = res.data.items?.[0] || res.data.Items?.[0];

            if (lastItem) {
                const dateFormat = i18n.language === 'tr' ? 'DD.MM.YYYY HH:mm' : 'MM/DD/YYYY HH:mm';

                setLastTransactionDate(
                    dayjs(lastItem.transactionDate || lastItem.createdDate).format(dateFormat)
                );
            } else {
                setLastTransactionDate(t('trans_no_history'));
            }
        } catch {
            setLastTransactionDate(t('trans_error'));
        }
    };

    const fetchInitialData = useCallback(async () => {
        try {
            const response = await api.get('/v1/Dashboard/summary');
            setUserData(response.data);
            const userWallets = response.data.wallets || [];
            setWallets(userWallets);

            const preId = localStorage.getItem('selected_wallet_id');
            if (preId) {
                const w = userWallets.find(x => x.id === parseInt(preId));
                if (w) {
                    setSelectedWallet(w);
                    form.setFieldsValue({ walletId: w.id });
                    fetchLastTransaction(w.id);
                }
            }
        } catch (error) {
            handleApiError(error, form, (msg) => console.error(msg));
        }
    }, [form]);

    useEffect(() => { fetchInitialData(); }, [fetchInitialData]);

    const handleWalletChange = (val) => {
        const w = wallets.find(x => x.id === val);
        setSelectedWallet(w);
        fetchLastTransaction(val);
    };

    const executeTransaction = async (values) => {
        setLoading(true);
        try {
            const endpoint = `/Wallet/${actionType}`;
            let payload = { ...values };
            if (actionType === 'transfer') payload.fromWalletId = values.walletId;

            await api.post(endpoint, payload);
            localStorage.removeItem('selected_wallet_id');
            notification.success({ message: t('trans_success_msg'), description: t('trans_success_desc') });
            setTimeout(() => navigate('/dashboard'), 1500);
        } catch (error) {
            handleApiError(error, form, (msg) => {
                notification.error({ message: t('trans_error_msg'), description: msg });
            });
        } finally {
            setLoading(false);
        }
    };

    return {
        wallets,
        loading,
        selectedWallet,
        actionType,
        setActionType,
        userData,
        lastTransactionDate,
        handleWalletChange,
        executeTransaction
    };
};