import React, { useEffect, useState, useCallback } from 'react';
import {
    Layout, Menu, Typography, Table, Card, Space, DatePicker,
    Tag, Avatar, Radio, Spin, Empty
} from 'antd';
import {
    AppstoreOutlined, SwapOutlined, HistoryOutlined, LineChartOutlined,
    LogoutOutlined, WalletOutlined, UserOutlined, FilterOutlined,
    ArrowUpOutlined, ArrowDownOutlined
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import dayjs from 'dayjs';

const { Content, Sider } = Layout;
const { Title, Text } = Typography;

const TransactionHistory = () => {
    const [wallets, setWallets] = useState([]);
    const [selectedWalletId, setSelectedWalletId] = useState(null);
    const [transactions, setTransactions] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [page, setPage] = useState(1);
    const [pageSize] = useState(3); // Her sayfada tam 3 satır
    const [startDate, setStartDate] = useState(null);
    const [endDate, setEndDate] = useState(null);
    const [userData, setUserData] = useState(null);

    const navigate = useNavigate();

    const fetchInitialData = useCallback(async () => {
        try {
            const response = await api.get('/v1/Dashboard/summary');
            setUserData(response.data);
            const userWallets = response.data.wallets || [];
            setWallets(userWallets);
            if (userWallets.length > 0) setSelectedWalletId(userWallets[0].id);
        } catch (error) { console.error("Veri yüklenemedi"); }
    }, []);

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
        } catch (error) { console.error("Geçmiş yüklenemedi"); }
        finally { setLoading(false); }
    }, [selectedWalletId, page, pageSize, startDate, endDate]);

    useEffect(() => { fetchInitialData(); }, [fetchInitialData]);
    useEffect(() => { fetchHistory(); }, [fetchHistory]);

    const columns = [
        {
            title: 'TARİH',
            dataIndex: 'transactionDate',
            render: (text, record) => <Text className="font-bold text-slate-500">{dayjs(record.transactionDate || record.createdDate).format('DD.MM.YYYY HH:mm:ss')}</Text>
        },
        {
            title: 'İŞLEM TÜRÜ',
            dataIndex: 'transactionType',
            render: (type, record) => {
                const isPositive = (record.amount || 0) > 0;
                return (
                    <Tag
                        icon={isPositive ? <ArrowUpOutlined /> : <ArrowDownOutlined />}
                        color={isPositive ? 'success' : 'error'}
                        style={{ borderRadius: '8px', fontWeight: '900', border: 'none', padding: '2px 10px' }}
                    >
                        {type?.toUpperCase()}
                    </Tag>
                );
            },
        },
        {
            title: 'MİKTAR',
            dataIndex: 'amount',
            align: 'right',
            render: (amount) => {
                const isPositive = amount > 0;
                return (
                    <span style={{ fontWeight: '900', fontSize: '17px', color: isPositive ? '#52c41a' : '#ff4d4f' }}>
                        {isPositive ? '+' : '-'} ₺{Math.abs(amount).toLocaleString('tr-TR', { minimumFractionDigits: 2 })}
                    </span>
                );
            }
        },
        {
            title: 'REFERANS NO',
            dataIndex: 'referenceId',
            render: (ref) => <Text className="text-slate-300 font-mono text-[11px]">{ref || '-'}</Text>,
        }
    ];

    return (
        <Layout style={{ minHeight: '100vh', background: 'linear-gradient(90deg, #e1e9eb 0%, #e8ebef 100%)', overflow: 'hidden' }}>
            <Sider width={260} theme="light" style={{ position: 'fixed', height: '100vh', borderRight: '1px solid #e2e8f0', zIndex: 1000 }}>
                <div className="p-8 flex items-center gap-3">
                    <div style={{ background: 'linear-gradient(135deg, #fb6365 0%, #c2494f 100%)', borderRadius: '14px' }} className="w-10 h-10 flex items-center justify-center">
                        <WalletOutlined style={{ color: '#fff', fontSize: '20px' }} />
                    </div>
                    <div className="flex flex-col">
                        <span style={{ fontSize: '22px', fontWeight: '900', color: '#ff4d4f', letterSpacing: '-0.5px' }}>WalletApp</span>
                        <span className="text-[10px] font-bold text-slate-400 tracking-[0.2em] uppercase">Management</span>
                    </div>
                </div>
                <Menu mode="inline" defaultSelectedKeys={['3']} className="border-none px-4"
                    items={[
                        { key: '1', icon: <AppstoreOutlined />, label: 'Ana Sayfa', onClick: () => navigate('/dashboard') },
                        { key: '2', icon: <SwapOutlined />, label: 'İşlem Yap' },
                        { key: '3', icon: <HistoryOutlined />, label: 'İşlem Geçmişi' },
                        { key: '4', icon: <LineChartOutlined />, label: 'Güncel Döviz Kurları' },
                        { type: 'divider' },
                        { key: 'logout', icon: <LogoutOutlined />, label: 'Çıkış Yap', danger: true, onClick: () => { localStorage.clear(); navigate('/login'); } }
                    ]}
                />
            </Sider>

            <Layout style={{ marginLeft: 260, backgroundColor: 'transparent', height: '100vh', overflow: 'hidden' }}>
                <Content className="px-10 py-6 flex flex-col h-full">

                    {/* ÜST BAŞLIK VE AYRI TAKVİMLER */}
                    <div className="flex justify-between items-start mb-6">
                        <div>
                            <Title level={2} style={{ margin: 0, fontWeight: '900', color: '#1e293b' }}>
                                <i className="bi bi-list-ul text-danger"></i> İşlem Geçmişi
                            </Title>
                            <Text style={{ color: '#94a3b8' }}>Cüzdan hareketlerini saniye bazlı takip edin</Text>
                        </div>

                        {/* TAKVİMLER - Kompakt ve Sağ Üstte */}
                        <div className="flex gap-3 bg-white p-3 rounded-2xl border border-slate-100 shadow-sm">
                            <div className="flex flex-col">
                                <Text style={{ fontSize: '9px', fontWeight: '900', color: '#ff4d4f', marginBottom: '4px' }}>BAŞLANGIÇ TARİHİ</Text>
                                <DatePicker placeholder="gg.aa.yyyy" onChange={setStartDate} className="h-8 w-32 rounded-lg" />
                            </div>
                            <div className="flex flex-col">
                                <Text style={{ fontSize: '9px', fontWeight: '900', color: '#ff4d4f', marginBottom: '4px' }}>BİTİŞ TARİHİ</Text>
                                <DatePicker placeholder="gg.aa.yyyy" onChange={setEndDate} className="h-8 w-32 rounded-lg" />
                            </div>
                        </div>
                    </div>

                    {/* CÜZDAN SEÇİMİ */}
                    <div className="mb-6">
                        <Text style={{ color: '#ff4d4f', fontWeight: '900', fontSize: '11px', letterSpacing: '2px', display: 'block', marginBottom: '12px' }}>CÜZDAN SEÇİMİ</Text>
                        <Radio.Group value={selectedWalletId} onChange={(e) => { setSelectedWalletId(e.target.value); setPage(1); }}>
                            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                                {wallets.map(w => (
                                    <Radio.Button key={w.id} value={w.id} className="wallet-radio-btn">
                                        <WalletOutlined style={{ marginRight: '8px' }} />
                                        {w.currency} # {w.id}
                                    </Radio.Button>
                                ))}
                            </div>
                        </Radio.Group>
                    </div>

                    {/* TABLO - 3 Satır ve Kayıt Sayısı */}
                    <Card className="rounded-[24px] border-none shadow-sm bg-white overflow-hidden flex-initial">
                        <Table
                            columns={columns}
                            dataSource={transactions}
                            loading={loading}
                            rowKey="id"
                            size="middle"
                            pagination={{
                                current: page,
                                total: totalCount,
                                pageSize: pageSize,
                                onChange: (p) => setPage(p),
                                position: ['bottomCenter'],
                                className: "custom-pagination",
                                showTotal: (total) => (
                                    <span style={{ position: 'absolute', left: 20, color: '#94a3b8', fontSize: '11px', fontWeight: '800' }}>
                                        TOPLAM {total} KAYIT
                                    </span>
                                )
                            }}
                            locale={{ emptyText: <Empty description="İşlem kaydı bulunamadı." /> }}
                        />
                    </Card>

                </Content>
            </Layout>

            <style dangerouslySetInnerHTML={{
                __html: `
                .wallet-radio-btn {
                    height: 44px !important;
                    line-height: 44px !important;
                    border-radius: 12px !important;
                    border: 1px solid #e2e8f0 !important;
                    font-weight: 700 !important;
                }
                .wallet-radio-btn:hover { color: #ffa39e !important; }
                .ant-radio-button-wrapper-checked.wallet-radio-btn {
                    background: #ff4d4f !important;
                    border-color: #ff4d4f !important;
                    color: white !important;
                }
                .wallet-radio-btn:before { display: none !important; }
                
                .ant-table-thead > tr > th { background: #fdfcfc !important; color: #94a3b8 !important; font-weight: 900 !important; font-size: 11px !important; padding: 12px 16px !important; }
                .ant-table-tbody > tr > td { padding: 12px 16px !important; }
                .ant-table-tbody > tr:hover > td { background: #fff2f0 !important; }
                
                .custom-pagination .ant-pagination-item-active { background: #ff4d4f; border-color: #ff4d4f; }
                .custom-pagination .ant-pagination-item-active a { color: white !important; }
            `}} />
        </Layout>
    );
};

export default TransactionHistory;