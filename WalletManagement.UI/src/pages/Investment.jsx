import React, { useEffect, useState, useCallback } from 'react';
import {
    Layout, Menu, Typography, Table, Card, Button, Modal, Col,
    Form, InputNumber, Select, Space, Tag, Avatar, notification, Divider, Row
} from 'antd';
import {
    AppstoreOutlined, SwapOutlined, HistoryOutlined, LineChartOutlined,
    LogoutOutlined, WalletOutlined, UserOutlined, ReloadOutlined,
    SafetyCertificateOutlined, TransactionOutlined
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import dayjs from 'dayjs';

const { Content, Sider } = Layout;
const { Title, Text } = Typography;

const Investment = () => {
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const [rates, setRates] = useState([]);
    const [wallets, setWallets] = useState([]);
    const [loading, setLoading] = useState(false);
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedRate, setSelectedRate] = useState(null);
    const [tradeType, setTradeType] = useState('BUY');
    const [userData, setUserData] = useState(null);
    const [lastUpdate, setLastUpdate] = useState('---');

    const walletTypeMap = { 1: 'Vadesiz', 2: 'Vadeli', 3: 'Yatırım' };

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
            const errorData = error.response?.data;
            notification.error({
                message: 'Veri Hatası',
                description: errorData?.Message || errorData?.message || 'Piyasa verileri şu an ulaşılamıyor.',
                placement: 'topRight'
            });
        } finally { setLoading(false); }
    }, []);

    useEffect(() => { fetchData(); }, [fetchData]);

    const openTrade = (record, type) => {
        setTradeType(type);
        setSelectedRate(record);
        form.resetFields();
        setModalVisible(true);
    };

    const onExecuteTrade = async (values) => {
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
        } finally { setLoading(false); }
    };

    const columns = [
        {
            title: 'DÖVİZ KODU',
            dataIndex: 'currencyCode',
            render: (code) => <Tag color="volcano" className="font-black px-3 py-1 rounded-lg border-none">{code}</Tag>
        },
        {
            title: 'BİRİM ADI',
            dataIndex: 'currencyName',
            render: (name) => <Text className="font-bold text-slate-600">{name || 'Yabancı Para'}</Text>
        },
        {
            title: 'ALIŞ (BUY)',
            dataIndex: 'buyingRate',
            render: (val) => <Text className="rate-font text-green-600">₺{val.toLocaleString('tr-TR', { minimumFractionDigits: 4 })}</Text>
        },
        {
            title: 'SATIŞ (SELL)',
            dataIndex: 'sellingRate',
            render: (val) => <Text className="rate-font text-red-500">₺{val.toLocaleString('tr-TR', { minimumFractionDigits: 4 })}</Text>
        },
        {
            title: 'İŞLEMLER',
            align: 'right',
            render: (_, record) => (
                <Space>
                    <Button className="btn-buy-custom" onClick={() => openTrade(record, 'BUY')}>AL</Button>
                    <Button className="btn-sell-custom" onClick={() => openTrade(record, 'SELL')}>SAT</Button>
                </Space>
            )
        }
    ];

    return (
        <Layout style={{ minHeight: '100vh', background: '#f8fafc', overflow: 'hidden' }}>
            <Sider width={260} theme="light" style={{ position: 'fixed', height: '100vh', borderRight: '1px solid #e2e8f0', zIndex: 1000 }}>
                <div className="p-8 flex items-center gap-3">
                    <div style={{ background: 'linear-gradient(135deg, #fb6365 0%, #c2494f 100%)', borderRadius: '14px' }} className="w-10 h-10 flex items-center justify-center">
                        <WalletOutlined style={{ color: '#fff', fontSize: '20px' }} />
                    </div>
                    <div className="flex flex-col">
                        <span style={{ fontSize: '22px', fontWeight: '900', color: '#ff4d4f', lineHeight: 1 }}>WalletApp</span>
                        <span style={{ fontSize: '10px', fontWeight: '800', color: '#94a3b8', textTransform: 'uppercase' }}>Management</span>
                    </div>
                </div>
                <Menu mode="inline" defaultSelectedKeys={['4']} className="border-none px-4"
                    items={[
                        { key: '1', icon: <AppstoreOutlined />, label: 'Ana Sayfa', onClick: () => navigate('/dashboard') },
                        { key: '2', icon: <SwapOutlined />, label: 'İşlem Yap', onClick: () => navigate('/transaction') },
                        { key: '3', icon: <HistoryOutlined />, label: 'İşlem Geçmişi', onClick: () => navigate('/history') },
                        { key: '4', icon: <LineChartOutlined />, label: 'Döviz Kurları' },
                        { type: 'divider' },
                        { key: 'logout', icon: <LogoutOutlined />, label: 'Çıkış Yap', danger: true, onClick: () => { localStorage.clear(); navigate('/login'); } }
                    ]}
                />
            </Sider>

            <Layout style={{ marginLeft: 260, background: 'transparent', height: '100vh', overflow: 'hidden' }}>
                <Content className="px-10 py-6 flex flex-col h-full overflow-hidden">
                    <div className="mb-6 flex justify-between items-end">
                        <div>
                            <Title level={2} style={{ margin: 0, fontWeight: 900 }}>Yatırım Merkezi</Title>
                            <Text type="secondary">Merkez Bankası verileriyle anlık ve güvenli döviz ticareti.</Text>
                        </div>
                        <div className="text-right">
                            <Button icon={<ReloadOutlined />} onClick={fetchData} loading={loading} className="rounded-xl font-black border-slate-200 shadow-sm">Verileri Yenile</Button>
                            <div className="mt-1"><Text className="text-[10px] text-slate-400 font-bold uppercase tracking-widest">Son Güncelleme: {lastUpdate}</Text></div>
                        </div>
                    </div>

                    <Card className="rounded-[32px] border-none shadow-sm flex-1 overflow-hidden"
                        styles={{ body: { height: '100%', padding: 0 } }}>
                        <Table
                            columns={columns}
                            dataSource={rates}
                            pagination={false}
                            loading={loading}
                            rowKey="currencyCode"
                            className="custom-investment-table"
                            scroll={{ y: 'calc(100vh - 280px)' }}
                        />
                    </Card>

                    <div className="mt-4 flex justify-between items-center bg-white p-4 rounded-2xl border border-slate-100 shadow-sm">
                        <Space className="text-slate-500 font-bold text-xs">
                            <Avatar size="small" icon={<UserOutlined />} style={{ backgroundColor: '#fff2f0', color: '#ff4d4f' }} />
                            {userData?.fullName} | Finansal Yönetici
                        </Space>
                        <Space className="text-slate-400 text-[10px] font-bold uppercase tracking-widest">
                            <SafetyCertificateOutlined className="text-green-500" /> 256-bit SSL Güvenlik Aktif
                        </Space>
                    </div>
                </Content>
            </Layout>

            <Modal
                title={null}
                open={modalVisible}
                onCancel={() => setModalVisible(false)}
                footer={null}
                centered
                width={520}
                className="trade-modal"
                styles={{ body: { padding: '20px 24px' } }}
            >
                <div className="text-center mb-4">
                    <div className={`trade-icon-circle ${tradeType === 'BUY' ? 'buy' : 'sell'}`} style={{ width: '48px', height: '48px', marginBottom: '8px' }}>
                        <TransactionOutlined style={{ fontSize: '20px' }} />
                    </div>
                    <Title level={4} style={{ margin: 0, fontWeight: '900', fontSize: '18px' }}>
                        {selectedRate?.currencyCode} {tradeType === 'BUY' ? 'ALIM İŞLEMİ' : 'SATIŞ İŞLEMİ'}
                    </Title>
                    <Text className="text-slate-400 text-[10px] uppercase tracking-widest font-black">
                        KUR: ₺{tradeType === 'BUY' ? selectedRate?.sellingRate : selectedRate?.buyingRate}
                    </Text>
                </div>

                <Form form={form} layout="vertical" onFinish={onExecuteTrade} requiredMark={false}>
                    <Form.Item
                        name="amount"
                        label={<span className="text-[10px] font-black uppercase text-slate-400 tracking-widest">Miktar</span>}
                        rules={[{ required: true, message: 'Miktar giriniz' }]}
                        style={{ marginBottom: '12px' }}
                    >
                        <InputNumber
                            className="w-full h-10 rounded-xl flex items-center font-black text-base"
                            style={{ width: '100%' }}
                            placeholder="0.00"
                            min={0.01}
                            onChange={() => {
                                const amt = form.getFieldValue('amount') || 0;
                                const rate = tradeType === 'BUY' ? selectedRate?.sellingRate : selectedRate?.buyingRate;
                                const total = amt * rate;
                                document.getElementById('total-calc').innerText = `₺ ${total.toLocaleString('tr-TR', { minimumFractionDigits: 2 })}`;
                            }}
                        />
                    </Form.Item>

                    <Row gutter={12}>
                        <Col span={12}>
                            <Form.Item
                                name="sourceWalletId"
                                label={<span className="text-[10px] font-black uppercase text-slate-400 tracking-widest">Kaynak Cüzdan</span>}
                                rules={[{ required: true, message: 'Seçiniz' }]}
                                style={{ marginBottom: '12px' }}
                            >
                                <Select size="middle" className="rounded-lg" placeholder="Ödeme">
                                    {wallets.map(w => (
                                        <Select.Option key={w.id} value={w.id}>
                                            {w.currency} - {walletTypeMap[w.type]} (₺{w.balance.toLocaleString('tr-TR')})
                                        </Select.Option>
                                    ))}
                                </Select>
                            </Form.Item>
                        </Col>
                        <Col span={12}>
                            <Form.Item
                                name="targetWalletId"
                                label={<span className="text-[10px] font-black uppercase text-slate-400 tracking-widest">Hedef Cüzdan</span>}
                                rules={[{ required: true, message: 'Seçiniz' }]}
                                style={{ marginBottom: '12px' }}
                            >
                                <Select size="middle" className="rounded-lg" placeholder="Yatırım">
                                    {wallets.map(w => (
                                        <Select.Option key={w.id} value={w.id}>
                                            {w.currency} - {walletTypeMap[w.type]} (₺{w.balance.toLocaleString('tr-TR')})
                                        </Select.Option>
                                    ))}
                                </Select>
                            </Form.Item>
                        </Col>
                    </Row>

                    <div className="p-3 bg-slate-50 rounded-xl border border-slate-100 mb-5">
                        <div className="flex justify-between items-center">
                            <Text className="text-[10px] font-black text-slate-500 uppercase">Tahmini Toplam:</Text>
                            <Title level={4} id="total-calc" style={{ margin: 0, color: '#ff4d4f', fontWeight: '900' }}>₺ 0.00</Title>
                        </div>
                    </div>

                    <Button
                        type="primary"
                        htmlType="submit"
                        block
                        loading={loading}
                        className={`h-12 rounded-xl font-black tracking-widest text-xs ${tradeType === 'BUY' ? 'btn-execute-buy' : 'btn-execute-sell'}`}
                        style={{ border: 'none' }}
                    >
                        {tradeType === 'BUY' ? 'ALIM İŞLEMİNİ ONAYLA' : 'SATIŞ İŞLEMİNİ ONAYLA'}
                    </Button>
                </Form>
            </Modal>

            <style dangerouslySetInnerHTML={{
                __html: `
                .rate-font { font-family: 'JetBrains Mono', monospace; font-weight: 800; font-size: 16px; }
                .custom-investment-table .ant-table-thead > tr > th { background: #fdfcfc !important; color: #94a3b8 !important; font-weight: 900 !important; font-size: 11px !important; padding: 20px !important; border-bottom: 1px solid #f1f5f9 !important; }
                .custom-investment-table .ant-table-tbody > tr > td { padding: 20px !important; border-bottom: 1px solid #f8fafc !important; }
                
                .btn-buy-custom { background: #f6ffed; color: #52c41a; border: 1px solid #b7eb8f; font-weight: 800; border-radius: 10px; width: 64px; height: 36px; }
                .btn-buy-custom:hover { background: #52c41a !important; color: white !important; }
                
                .btn-sell-custom { background: #fff1f0; color: #ff4d4f; border: 1px solid #ffccc7; font-weight: 800; border-radius: 10px; width: 64px; height: 36px; }
                .btn-sell-custom:hover { background: #ff4d4f !important; color: white !important; }
                
                .trade-modal .ant-modal-content { border-radius: 32px !important; padding: 30px !important; }
                .trade-icon-circle { width: 64px; height: 64px; border-radius: 20px; display: flex; align-items: center; justify-content: center; margin: 0 auto; }
                .trade-icon-circle.buy { background: #f6ffed; color: #52c41a; }
                .trade-icon-circle.sell { background: #fff1f0; color: #ff4d4f; }
                
                .btn-execute-buy { background: #52c41a !important; box-shadow: 0 8px 20px rgba(82, 196, 26, 0.2) !important; }
                .btn-execute-sell { background: #ff4d4f !important; box-shadow: 0 8px 20px rgba(255, 77, 79, 0.2) !important; }
                
                .ant-modal-mask { backdrop-filter: blur(4px); }
            `}} />
        </Layout>
    );
};

export default Investment;