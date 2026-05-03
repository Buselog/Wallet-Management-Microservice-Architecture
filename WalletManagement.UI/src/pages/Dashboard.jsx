import React, { useEffect, useState, useCallback } from 'react';
import {
    Layout, Menu, Typography, Card, Row, Col, Button,
    Statistic, Modal, Select, Avatar, Space, Tag, Form,
    message, Spin, Popconfirm, Divider
} from 'antd';
import {
    AppstoreOutlined, SwapOutlined, HistoryOutlined, LineChartOutlined,
    LogoutOutlined, PlusOutlined, WalletOutlined, UserOutlined,
    DeleteOutlined, ArrowRightOutlined, LoadingOutlined
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const { Header, Content } = Layout;
const { Title, Text } = Typography;


const customStyles = {
    volcanoButton: {
        backgroundColor: '#ff4d4f',
        borderColor: '#ff4d4f',
        borderRadius: '16px',
        fontWeight: '700',
        height: '40px',
        boxShadow: '0 8px 20px rgba(255, 77, 79, 0.2)',
        textTransform: 'uppercase',
        letterSpacing: '1px'
    },
    brandLogo: {
        background: 'linear-gradient(135deg, #fb6365 0%, #c2494f 100%)',
        boxShadow: '0 8px 16px rgba(255, 77, 79, 0.25)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        borderRadius: '14px',
        transition: 'all 0.3s ease'
    },
};

const Dashboard = () => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [rates, setRates] = useState([]);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [form] = Form.useForm();
    const navigate = useNavigate();

    const fetchDashboardData = useCallback(async () => {
        try {
            setLoading(true);
            const summaryRes = await api.get('/v1/Dashboard/summary');
            setData(summaryRes.data);

            const ratesRes = await api.get('/InvestmentRate/rates');
            setRates(ratesRes.data || []);
        } catch (error) {
            if (error.response?.status === 401) navigate('/login');
            message.error("Veriler güncellenemedi.");
        } finally {
            setLoading(false);
        }
    }, [navigate]);

    const createWallet = async (values) => {
        try {

            await api.post('/Wallet/create', values);
            Modal.success({
                title: 'İşlem Başarılı',
                content: 'Yeni cüzdanınız başarıyla oluşturuldu ve kullanıma hazır.',
                centered: true,
                okText: 'Tamam',
                okButtonProps: { style: { backgroundColor: '#52c41a', borderColor: '#52c41a', borderRadius: '8px' } },
                onOk: () => {
                    setIsModalVisible(false);
                    form.resetFields();
                    fetchDashboardData();
                }
            });
        } catch (error) {
            const errorMsg = error.response?.data?.Message || error.response?.data?.message || "Cüzdan oluşturulamadı.";

            Modal.error({
                title: 'Cüzdan Oluşturulamadı',
                content: errorMsg,
                centered: true,
                okText: 'Anladım',
                okButtonProps: { style: { backgroundColor: '#ff4d4f', borderColor: '#ff4d4f', borderRadius: '8px' } },
            });
        }
    };

    const deleteWallet = async (id) => {
        try {
            await api.delete(`/Wallet/${id}/delete`);

            message.success("Cüzdan başarıyla kapatıldı.");
            fetchDashboardData();
        } catch (error) {
            const errorMsg = error.response?.data?.Message || error.response?.data?.message || "Silme işlemi başarısız.";

            Modal.warning({
                title: 'Silme İşlemi Gerçekleştirilemedi',
                content: errorMsg,
                centered: true,
                okText: 'Tamam'
            });
        }
    };

    useEffect(() => { fetchDashboardData(); }, [fetchDashboardData]);

    const handleLogout = () => {
        localStorage.clear();
        navigate('/login');
    };

    if (loading) return (
        <div className="h-screen w-full flex items-center justify-center bg-white">
            <Spin indicator={<LoadingOutlined style={{ fontSize: 48, color: '#ff4d4f' }} spin />} />
        </div>
    );

    return (
        <>
            <Header style={{
                background: '#fff',
                padding: '0 32px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                borderBottom: '1px solid #f0f0f0',
                position: 'sticky',
                top: 0,
                zIndex: 10,
                height: '72px'
            }}>
                <div style={{ display: 'flex', flexDirection: 'column' }}>
                    <Title level={4} style={{ margin: 0, color: '#1e293b', fontWeight: '800' }}>
                        Hoş Geldiniz, {data?.fullName}
                    </Title>
                    <Text style={{ color: '#94a3b8', fontSize: '12px' }}>Varlıklarınızın güncel durumu aşağıdadır.</Text>
                </div>
                <Space size="large">
                    <Avatar icon={<UserOutlined />} style={{ backgroundColor: '#fff2f0', color: '#ff4d4f', border: '1px solid #ffccc7' }} />
                    <Button
                        type="primary"
                        style={customStyles.volcanoButton}
                        icon={<PlusOutlined />}
                        onClick={() => setIsModalVisible(true)}
                    >
                        Yeni Cüzdan
                    </Button>
                </Space>
            </Header>

            <Content className="p-10 max-w-[1600px]">
                <div className="mb-12">
                    <Text className="text-slate-400 font-bold text-[10px] tracking-[0.2em] uppercase">Varlık Dağılımı</Text>
                    <Row gutter={[24, 24]} className="mt-4" justify="start">
                        {data?.currencySummaries?.map(summary => (
                            <Col
                                span={24 / (data?.currencySummaries?.length || 1)}
                                key={summary.currency}
                                xs={24} sm={12} lg={24 / (data?.currencySummaries?.length || 1)}
                            >
                                <Card
                                    bordered={false}
                                    className="shadow-sm"
                                    style={{
                                        borderRadius: '20px',
                                        borderLeft: '5px solid #ff4d4f',
                                        boxShadow: '0 4px 12px rgba(0,0,0,0.03)'
                                    }}
                                >
                                    <div className="absolute top-0 left-0 w-1.5 h-full bg-volcano-500" />
                                    <Statistic
                                        title={<span className="text-[10px] font-black text-slate-400 uppercase tracking-widest">TOPLAM {summary.currency}</span>}
                                        value={summary.totalBalance}
                                        precision={2}
                                        prefix={<span className="text-volcano-500 mr-1">{summary.currency === 'TRY' ? '₺' : (summary.currency === 'USD' ? '$' : '€')}</span>}
                                        valueStyle={{ fontWeight: 900, color: '#1e293b', fontSize: '28px' }}
                                    />
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </div>

                <Text className="text-slate-400 font-bold text-[10px] tracking-[0.2em] uppercase mb-6 block">
                    Cüzdanlarım
                </Text>
                <Row gutter={[24, 24]}>
                    {data?.wallets?.map(wallet => (
                        <Col xs={24} md={12} lg={8} key={wallet.id}>
                            <Card
                                className="wallet-card-premium transition-all duration-300"
                                styles={{ body: { padding: '28px' } }}
                                actions={[
                                    <Popconfirm title="Cüzdanı silmek istediğinize emin misiniz?" onConfirm={() => deleteWallet(wallet.id)} okText="Evet" cancelText="Hayır">
                                        <DeleteOutlined key="delete" style={{ color: '#ff4d4f', fontSize: '18px' }} />
                                    </Popconfirm>,
                                    <ArrowRightOutlined key="go" style={{ color: '#f0484b', fontSize: '18px' }} />
                                ]}
                            >
                                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
                                    <div style={{
                                        backgroundColor: '#fff2f0',
                                        padding: '16px',
                                        borderRadius: '20px',
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'center',
                                        boxShadow: '0 4px 12px rgba(255, 77, 79, 0.1)'
                                    }}>
                                        <WalletOutlined style={{ fontSize: '24px', color: '#ff4d4f' }} />
                                    </div>
                                    <div style={{ textAlign: 'right' }}>
                                        <Tag
                                            color="default"
                                            style={{
                                                borderRadius: '10px',
                                                background: 'linear-gradient(90deg, #dedfe0 0%, #f0d7cf 100%)',
                                                color: '#f15f62',
                                                border: 'none',
                                                fontWeight: '900',
                                                padding: '4px 14px',
                                                margin: 0,
                                                display: 'inline-flex',
                                                alignItems: 'center',
                                                justifyContent: 'center'
                                            }}
                                        >
                                            {wallet.currency}
                                        </Tag>
                                        <div style={{ color: '#bfc8d3', fontSize: '12px', fontWeight: 'bold', marginTop: '6px', fontFamily: 'monospace' }}>
                                            ID: #{wallet.id}
                                        </div>
                                    </div>
                                </div>

                                <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
                                    <Text style={{ color: '#94a3b8', fontSize: '11px', fontWeight: '800', textTransform: 'uppercase', letterSpacing: '1px' }}>
                                        Kullanılabilir Bakiye
                                    </Text>
                                    <div style={{ display: 'flex', alignItems: 'baseline', gap: '6px' }}>
                                        <span style={{ fontSize: '20px', fontWeight: '300', color: '#ff4d4f' }}>
                                            {wallet.currency === 'TRY' ? '₺' : (wallet.currency === 'USD' ? '$' : '€')}
                                        </span>
                                        <span style={{ fontSize: '32px', fontWeight: '900', color: '#0f172a', letterSpacing: '-1px' }}>
                                            {wallet.balance.toLocaleString('tr-TR', { minimumFractionDigits: 2 })}
                                        </span>
                                    </div>
                                </div>
                            </Card>
                        </Col>
                    ))}
                </Row>
            </Content>

            <Modal
                title={<Title level={3} style={{ margin: 0, fontWeight: '900', color: '#1e293b' }}>Yeni Cüzdan Oluştur</Title>}
                open={isModalVisible}
                onCancel={() => setIsModalVisible(false)}
                footer={null}
                centered
                width={480}
                styles={{
                    mask: { backdropFilter: 'blur(10px)' },
                    content: { borderRadius: '32px' }
                }}
            >
                <Form form={form} layout="vertical" onFinish={createWallet} style={{ marginTop: '32px' }}>
                    <Form.Item
                        name="type"
                        label={<Text style={{ fontSize: '11px', fontWeight: '900', color: '#94a3b8', letterSpacing: '1px' }}>CÜZDAN TİPİ</Text>}
                        rules={[{ required: true, message: 'Lütfen bir tip seçin' }]}
                    >
                        <Select
                            size="large"
                            placeholder="Hesap türünü belirleyin"
                            getPopupContainer={trigger => trigger.closest('.ant-modal-content')}
                            className="custom-select"
                        >
                            <Select.Option value={1}>Vadesiz Hesap (Checking)</Select.Option>
                            <Select.Option value={2}>Vadeli Hesap (Saving)</Select.Option>
                            <Select.Option value={3}>Yatırım Hesabı (Investment)</Select.Option>
                        </Select>
                    </Form.Item>

                    <Form.Item
                        name="currency"
                        label={<Text style={{ fontSize: '11px', fontWeight: '900', color: '#94a3b8', letterSpacing: '1px' }}>PARA BİRİMİ</Text>}
                        rules={[{ required: true, message: 'Lütfen bir para birimi seçin' }]}
                    >
                        <Select
                            size="large"
                            placeholder="İşlem yapılacak birimi seçin"
                            getPopupContainer={trigger => trigger.closest('.ant-modal-content')}
                            className="custom-select"
                        >
                            <Select.Option value="TRY">TRY - Türk Lirası</Select.Option>
                            {rates.map(r => (
                                <Select.Option key={r.currencyCode} value={r.currencyCode}>
                                    {r.currencyCode} - Küresel Birim
                                </Select.Option>
                            ))}
                        </Select>
                    </Form.Item>

                    <Button
                        type="primary"
                        block
                        htmlType="submit"
                        style={customStyles.volcanoButton}
                    >
                        Cüzdanı Aktifleştir
                    </Button>
                </Form>
            </Modal>

            <style dangerouslySetInnerHTML={{
                __html: `
    .ant-btn-primary { background-color: #ff4d4f !important; border-color: #ff4d4f !important; }
    .ant-btn-primary:hover { background-color: #ff7875 !important; border-color: #ff7875 !important; }
    
   
    .ant-modal-content { overflow: visible !important; border-radius: 32px !important; }
    .ant-select-dropdown { z-index: 10001 !important; border-radius: 16px !important; overflow: hidden !important; }
    
    .custom-menu .ant-menu-item-selected {
        background-color: #fff2f0 !important;
        color: #ff4d4f !important;
        font-weight: 800;
        border-radius: 14px !important;
    }
    .wallet-card-premium {
        border: 1px solid #f0f0f0 !important;
        background: linear-gradient(145deg, #ffffff 0%, #fdfcfc 100%) !important;
    }
    .wallet-card-premium:hover {
        border-color: #ff4d4f !important;
        box-shadow: 0 20px 40px rgba(0,0,0,0.05) !important;
        transform: translateY(-5px);
    }
`}} />
        </>
    );
};

export default Dashboard;
