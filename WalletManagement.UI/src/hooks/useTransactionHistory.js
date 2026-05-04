import { useState, useCallback, useEffect } from 'react';
import api from '../services/api';
import { notification } from 'antd';
import { handleApiError } from '../utils/errorHandler';
import { useTranslation } from 'react-i18next';

export const useTransactionHistory = () => {
    const { t } = useTranslation();
    const [wallets, setWallets] = useState([]);
    const [selectedWalletId, setSelectedWalletId] = useState(null);
    const [transactions, setTransactions] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [page, setPage] = useState(1);
    const [pageSize] = useState(3);
    const [startDate, setStartDate] = useState(null);
    const [endDate, setEndDate] = useState(null);
    const [userData, setUserData] = useState(null);

    const fetchInitialData = useCallback(async () => {
        try {
            const response = await api.get('/v1/Dashboard/summary');
            setUserData(response.data);
            const userWallets = response.data.wallets || [];
            setWallets(userWallets);
            if (userWallets.length > 0) setSelectedWalletId(userWallets[0].id);
        } catch (error) {
            handleApiError(error, null, (msg) => {
                notification.error({
                    message: t('trans_error_msg'),
                    description: msg
                });
            });
        }
    }, [t]);

    const fetchHistory = useCallback(async () => {
        if (!selectedWalletId) return;
        setLoading(true);
        try {
            const params = { pageNumber: page, pageSize: pageSize };
            if (startDate) params.startDate = startDate.startOf('day').toISOString();
            if (endDate) params.endDate = endDate.endOf('day').toISOString();

            const response = await api.get(`/WalletTransaction/history/${selectedWalletId}`, { params });
            setTransactions(response.data.items || response.data.Items || []);
            setTotalCount(response.data.totalCount || response.data.TotalCount || 0);
        } catch (error) {
            handleApiError(error, null, (msg) => {
                notification.error({
                    message: t('trans_history_error_msg'),
                    description: msg
                });
            });
        } finally {
            setLoading(false);
        }
    }, [selectedWalletId, page, pageSize, startDate, endDate]);

    useEffect(() => { fetchInitialData(); }, [fetchInitialData]);
    useEffect(() => { fetchHistory(); }, [fetchHistory]);

    const handleWalletChange = (id) => {
        setSelectedWalletId(id);
        setPage(1);
    };

    return {
        wallets,
        selectedWalletId,
        transactions,
        totalCount,
        loading,
        page,
        setPage,
        pageSize,
        setStartDate,
        setEndDate,
        userData,
        handleWalletChange
    };
};