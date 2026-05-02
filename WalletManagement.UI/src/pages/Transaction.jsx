import React, { useEffect, useState, useCallback } from 'react';
import {
    Layout, Menu, Typography, Card, Button, Form, Input,
    Select, Radio, Avatar, InputNumber, notification, Row, Col, Space, Tag
} from 'antd';
import {
    AppstoreOutlined, SwapOutlined, HistoryOutlined, LineChartOutlined,
    LogoutOutlined, WalletOutlined, UserOutlined, SendOutlined,
    PlusCircleOutlined, MinusCircleOutlined, SafetyCertificateOutlined,
    ClockCircleOutlined
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import dayjs from 'dayjs';

const { Content, Sider } = Layout;
const { Title, Text } = Typography;

const walletTypeMap = {
    1: 'Vadesiz Hesap',
    2: 'Vadeli Hesap',
    3: 'Yatırım Hesabı'
};

const Transaction = () => {
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const [wallets, setWallets] = useState([]);
    const [loading, setLoading] = useState(false);
    const [selectedWallet, setSelectedWallet] = useState(null);
    const [actionType, setActionType] = useState('deposit');
    const [userData, setUserData] = useState(null);
    const [lastTransactionDate, setLastTransactionDate] = useState('İşlem yok');

    const fetchLastTransaction = async (walletId) => {
        try {
            const res = await api.get(`/WalletTransaction/history/${walletId}`, {
                params: { pageNumber: 1, pageSize: 1 }
            });
            const lastItem = res.data.items?.[0] || res.data.Items?.[0];
            setLastTransactionDate(lastItem ? dayjs(lastItem.transactionDate || lastItem.createdDate).format('DD.MM.YYYY HH:mm') : 'İşlem yok');
        } catch {
            setLastTransactionDate('Hata');
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
        } catch (error) { notification.error({ message: 'Hata', description: 'Veriler alınamadı.' }); }
    }, [form]);

    useEffect(() => { fetchInitialData(); }, [fetchInitialData]);

    const onWalletChange = (val) => {
        const w = wallets.find(x => x.id === val);
        setSelectedWallet(w);
        fetchLastTransaction(val);
    };

    const onFinish = async (values) => {
        setLoading(true);
        try {
            let endpoint = `/Wallet/${actionType}`;
            let payload = { ...values };
            if (actionType === 'transfer') payload.fromWalletId = values.walletId;

            const response = await api.post(endpoint, payload);
            notification.success({ message: 'Başarılı', description: response.data.message || 'İşlem gerçekleştirildi.' });
            localStorage.removeItem('selected_wallet_id');
            setTimeout(() => navigate('/dashboard'), 1500);
        } catch (error) {
            const errorMsg = error.response?.data?.Message || error.response?.data?.message;
            if (errorMsg && error.response?.status === 400) {
                const parts = errorMsg.split(' | ');
                const fields = [];
                parts.forEach(part => {
                    if (part.includes(':')) {
                        const [property, msg] = part.split(':').map(s => s.trim());
                        const propertyMap = { 'WalletId': 'walletId', 'FromWalletId': 'walletId', 'Amount': 'amount', 'ReferenceId': 'referenceId', 'Target': 'target' };
                        const fieldName = propertyMap[property];
                        if (fieldName) fields.push({ name: fieldName, errors: [msg] });
                    }
                });
                if (fields.length > 0) form.setFields(fields);
                else notification.error({ message: 'Hata', description: errorMsg });
            } else { notification.error({ message: 'Hata', description: errorMsg || 'Bir sorun oluştu.' }); }
        } finally { setLoading(false); }
    };

    return (
        <Layout style={{ minHeight: '100vh', background: '#f8fafc', overflow: 'hidden' }}>
            <Sider width={260} theme="light" style={{ position: 'fixed', height: '100vh', borderRight: '1px solid #e2e8f0', zIndex: 1000 }}>
                <div className="p-8 flex items-center gap-3">
                    <div style={{ background: 'linear-gradient(135deg, #fb6365 0%, #c2494f 100%)', borderRadius: '14px' }} className="w-10 h-10 flex items-center justify-center">
                        <WalletOutlined style={{ color: '#fff', fontSize: '20px' }} />
                    </div>
                    <div className="flex flex-col">
                        <span style={{ fontSize: '22px', fontWeight: '900', color: '#ff4d4f', lineHeight: 1 }}>WalletApp</span>
                        <span className="text-[10px] font-bold text-slate-400 tracking-[0.2em] uppercase">Management</span>
                    </div>
                </div>
                <Menu mode="inline" defaultSelectedKeys={['2']} className="border-none px-4"
                    items={[
                        { key: '1', icon: <AppstoreOutlined />, label: 'Ana Sayfa', onClick: () => navigate('/dashboard') },
                        { key: '2', icon: <SwapOutlined />, label: 'İşlem Yap' },
                        { key: '3', icon: <HistoryOutlined />, label: 'İşlem Geçmişi', onClick: () => navigate('/history') },
                        { key: '4', icon: <LineChartOutlined />, label: 'Döviz Kurları' },
                        { type: 'divider' },
                        { key: 'logout', icon: <LogoutOutlined />, label: 'Çıkış Yap', danger: true, onClick: () => { localStorage.clear(); navigate('/login'); } }
                    ]}
                />
            </Sider>

            <Layout style={{ marginLeft: 260, background: 'transparent', height: '100vh', overflow: 'hidden' }}>
                <Content className="px-10 py-6 flex flex-col h-full overflow-hidden">
                    <div className="mb-6 flex justify-between items-center">
                        <div>
                            <Title level={3} style={{ margin: 0, fontWeight: 900 }}>İşlem Merkezi</Title>
                            <Text type="secondary" className="text-xs">Güvenli varlık yönetimi ve anlık transfer.</Text>
                        </div>
                        <Space>
                            <Avatar icon={<UserOutlined />} style={{ backgroundColor: '#fff2f0', color: '#ff4d4f' }} />
                            <Text className="font-bold text-slate-700">{userData?.fullName}</Text>
                        </Space>
                    </div>

                    <Row gutter={24} className="flex-1 overflow-hidden" align="stretch">
                        <Col xs={24} lg={10} style={{ display: 'flex', flexDirection: 'column' }}>
                            <Card className="rounded-[32px] border-none shadow-sm flex-1 flex flex-col" style={{ background: 'linear-gradient(135deg, #fff9f5 0%, #ffffff 100%)', border: '1px solid #ffe8db' }}>
                                <div className="flex-1 flex flex-col justify-center items-center py-4"> {/* py-4 ve justify-center eklendi */}
                                    <div className="bg-white p-4 rounded-3xl shadow-sm mb-4 border border-orange-50">
                                        <WalletOutlined style={{ fontSize: '32px', color: '#ff4d4f' }} />
                                    </div>
                                    <Text className="text-slate-400 block uppercase tracking-widest text-[10px] font-black">SEÇİLİ CÜZDAN BAKİYESİ</Text>
                                    <div className="mt-2">
                                        <Title level={1} className="m-0 font-black text-slate-800" style={{ fontSize: '42px', lineHeight: 1 }}>
                                            {selectedWallet ? `${selectedWallet.currency === 'TRY' ? '₺' : '$'} ${selectedWallet.balance.toLocaleString('tr-TR')}` : '---'}
                                        </Title>
                                    </div>
                                    <Tag color="volcano" className="mt-6 px-6 py-1 rounded-full font-bold border-none text-[11px] uppercase tracking-wider">
                                        {selectedWallet ? `${selectedWallet.currency} - ${walletTypeMap[selectedWallet.type]}` : 'CÜZDAN SEÇİNİZ'}
                                    </Tag>
                                </div>
                                <div className="mt-auto pt-6 border-t border-orange-100 flex gap-4 text-left">
                                    <div className="flex-1">
                                        <Text className="text-slate-400 block text-[9px] font-black uppercase tracking-tighter">Hesap Durumu</Text>
                                        <Text className="text-green-600 font-bold text-xs"><PlusCircleOutlined className="mr-1" /> AKTİF</Text>
                                    </div>
                                    <div className="flex-1">
                                        <Text className="text-slate-400 block text-[9px] font-black uppercase tracking-tighter">Son İşlem Tarihi</Text>
                                        <Text className="text-slate-700 font-bold text-xs"><ClockCircleOutlined className="mr-1" /> {lastTransactionDate}</Text>
                                    </div>
                                </div>
                            </Card>
                        </Col>

                        <Col xs={24} lg={14} style={{ display: 'flex', flexDirection: 'column' }}>
                            <Form form={form} layout="vertical" onFinish={onFinish} requiredMark={false} className="flex-1 flex flex-col h-full min-h-0">
                                <Card
                                    className="rounded-[32px] shadow-sm border-none flex-1 overflow-hidden min-h-0"
                                    styles={{
                                        body: {
                                            height: '100%', display: 'flex', flexDirection: 'column', padding: '24px', overflow: 'hidden',
                                            minHeight: 0
                                        }
                                    }}
                                >
                                    <div
                                        className="flex-1 pr-2 custom-scrollbar"
                                        style={{
                                            overflowY: actionType === 'transfer' ? 'auto' : 'hidden',
                                            minHeight: 0,
                                            maxHeight: '100%'
                                        }}
                                    >
                                        <div className="mb-6">
                                            <Text className="text-slate-400 block uppercase tracking-widest text-[10px] font-black mb-4">1. İŞLEM AYARLARI</Text>

                                            <Form.Item name="walletId" label={<span className="text-[11px] font-black uppercase text-slate-500">Kaynak Cüzdan</span>} rules={[{ required: true, message: 'Lütfen cüzdan seçin' }]}>
                                                <Select size="large" className="custom-select-large" placeholder="Seçim yapın" onChange={onWalletChange}>
                                                    {wallets.map(w => <Select.Option key={w.id} value={w.id}>{w.currency} - {walletTypeMap[w.type]}</Select.Option>)}
                                                </Select>
                                            </Form.Item>

                                            <Form.Item label={<span className="text-[11px] font-black uppercase text-slate-500">İşlem Türü</span>}>
                                                <Radio.Group value={actionType} onChange={(e) => setActionType(e.target.value)} className="w-full flex gap-3 p-1 bg-slate-50 rounded-2xl border border-slate-100">
                                                    <Radio.Button value="deposit" className="flex-1 flex justify-center items-center rounded-xl h-11 font-black text-xs custom-radio">Para Yatır</Radio.Button>
                                                    <Radio.Button value="withdraw" className="flex-1 flex justify-center items-center rounded-xl h-11 font-black text-xs custom-radio">Para Çek</Radio.Button>
                                                    <Radio.Button value="transfer" className="flex-1 flex justify-center items-center rounded-xl h-11 font-black text-xs custom-radio">Transfer</Radio.Button>
                                                </Radio.Group>
                                            </Form.Item>

                                            <Row gutter={16}>
                                                <Col span={12}>
                                                    <Form.Item name="amount" label={<span className="text-[11px] font-black uppercase text-slate-500">Tutar</span>} rules={[{ required: true, message: 'Tutar giriniz' }]}>
                                                        <InputNumber size="large" className="custom-input-number" prefix={selectedWallet?.currency === 'TRY' ? '₺' : '$'} placeholder="0.00" min={0.01} style={{ width: '100%' }} />
                                                    </Form.Item>
                                                </Col>
                                                <Col span={12}>
                                                    <Form.Item name="referenceId" label={<span className="text-[11px] font-black uppercase text-slate-500">Referans Numarası</span>} rules={[{ required: true, message: 'Referans gerekli' }]}>
                                                        <Input size="large" className="rounded-xl" placeholder="REF-451" />
                                                    </Form.Item>
                                                </Col>
                                            </Row>
                                        </div>

                                        {actionType === 'transfer' && (
                                            <>
                                                <div className="p-6 bg-orange-50/40 rounded-2xl border border-dashed border-orange-200 mb-6 animate-slide-down">
                                                    <Text className="text-orange-500 block uppercase tracking-widest text-[9px] font-black mb-4">2. ALICI DETAYLARI</Text>
                                                    <Row gutter={16}>
                                                        <Col span={12}>
                                                            <Form.Item name="target" label={<span className="text-[11px] font-bold text-orange-700">Alıcı (IBAN/Tel)</span>} rules={[{ required: true, message: 'Alıcı bilgisi giriniz' }]}>
                                                                <Input size="large" className="rounded-xl border-orange-200" placeholder="TR... / 05..." />
                                                            </Form.Item>
                                                        </Col>
                                                        <Col span={12}>
                                                            <Form.Item name="description" label={<span className="text-[11px] font-bold text-orange-700">Açıklama</span>} className="mb-0">
                                                                <Input size="large" className="rounded-xl border-orange-200" placeholder="İşlem notu" />
                                                            </Form.Item>
                                                        </Col>
                                                    </Row>
                                                </div>

                                                <div className="mt-4 pb-4">
                                                    <Button type="primary" htmlType="submit" block loading={loading} className="h-12 rounded-2xl font-black text-xs tracking-widest shadow-xl" style={{ background: '#ff4d4f', border: 'none' }}>
                                                        İŞLEMİ ONAYLA VE GERÇEKLEŞTİR
                                                    </Button>
                                                    <div className="mt-4 text-center">
                                                        <Space className="text-slate-400 text-[10px] uppercase font-bold tracking-widest">
                                                            <SafetyCertificateOutlined className="text-green-500" /> 256-bit SSL Koruma Aktif
                                                        </Space>
                                                    </div>
                                                </div>
                                            </>
                                        )}
                                    </div>

                                    {actionType !== 'transfer' && (
                                        <div className="mt-auto pt-4 bg-white border-t border-slate-50">
                                            <Button type="primary" htmlType="submit" block loading={loading} className="h-14 rounded-2xl font-black text-sm tracking-widest shadow-xl" style={{ background: '#ff4d4f', border: 'none' }}>
                                                İŞLEMİ ONAYLA VE GERÇEKLEŞTİR
                                            </Button>
                                            <div className="mt-4 text-center">
                                                <Space className="text-slate-400 text-[10px] uppercase font-bold tracking-widest">
                                                    <SafetyCertificateOutlined className="text-green-500" /> 256-bit SSL Koruma Aktif
                                                </Space>
                                            </div>
                                        </div>
                                    )}
                                </Card>
                            </Form>
                        </Col>
                    </Row>
                </Content>
            </Layout>

            <style dangerouslySetInnerHTML={
                {

                    __html: `
                .custom-radio { border: none !important; background: transparent !important; color: #94a3b8; transition: all 0.3s; }
                .ant-radio-button-wrapper-checked.custom-radio { background: #ffffff !important; color: #ff4d4f !important; box-shadow: 0 4px 12px rgba(255, 77, 79, 0.15) !important; border-radius: 12px !important; }
                .ant-radio-button-wrapper-checked.custom-radio:hover, .ant-radio-button-wrapper:hover { color: #ff4d4f !important; }
                .ant-radio-button-wrapper:before { display: none !important; }
                .custom-select-large .ant-select-selector { border-radius: 14px !important; height: 48px !important; display: flex !important; align-items: center !important; }
                .custom-input-number { width: 100% !important; border-radius: 12px !important; }
                .custom-scrollbar::-webkit-scrollbar { width: 4px; }
                .custom-scrollbar::-webkit-scrollbar-thumb { background: #ffe8db; border-radius: 10px; }
                .animate-slide-down { animation: slideDown 0.3s ease-out; }
                @keyframes slideDown { from { opacity: 0; transform: translateY(-5px); } to { opacity: 1; transform: translateY(0); } }
    height: 100% !important;

             .ant-card-body {
               padding: 20px !important; 
               display: flex !important;
               flex-direction: column !important;
            }

             .ant-form-item {
             margin-bottom: 12px !important;
            }`}}
            />
        </Layout>
    );
};

export default Transaction;
