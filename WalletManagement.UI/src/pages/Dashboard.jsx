import React, { useEffect, useState } from 'react';
import {
    Layout, Menu, Typography, Card, Row, Col, Button,
    Statistic, Modal, Select, Avatar, Space, Tag, Form,
    message, Spin, Popconfirm
} from 'antd';
import {
    AppstoreOutlined,
    SwapOutlined,
    HistoryOutlined,
    LineChartOutlined,
    LogoutOutlined,
    PlusOutlined,
    WalletOutlined,
    UserOutlined,
    DeleteOutlined
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const { Header, Content, Sider } = Layout;
const { Title, Text } = Typography;

const Dashboard = () => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [rates, setRates] = useState([]); // Dinamik kurlar için
    const [form] = Form.useForm();
    const navigate = useNavigate();

    const fetchSummary = async () => {
        try {
            const response = await api.get('/v1/Dashboard/summary');
            setData(response.data);
        } catch (error) {
            message.error("Veriler yüklenirken hata oluştu.");
        } finally {
            setLoading(false);
        }
    };

    const fetchRates = async () => {
        try {
            const response = await api.get('/Investment/rates');
            setRates(response.data);
        } catch (error) {
            console.error("Kurlar alınamadı");
        }
    };

    useEffect(() => {
        fetchSummary();
        fetchRates();
    }, []);

    const handleLogout = () => {
        localStorage.clear();
        navigate('/login');
    };

    const createWallet = async (values) => {
        try {
            await api.post('/api/Wallet/create', values);
            message.success("Cüzdan başarıyla oluşturuldu.");
            setIsModalVisible(false);
            form.resetFields();
            fetchSummary();
        } catch (error) {
        }
    };

    const deleteWallet = async (id) => {
        try {
            await api.delete(`/api/Wallet/${id}/delete`);
            message.success("Cüzdan kapatıldı.");
            fetchSummary();
        } catch (error) {
            message.error(error.response?.data?.Message || "Hata oluştu.");
        }
    };

    if (loading) return <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}><Spin size="large" /></div>;

    return (
        <Layout style={{ minHeight: '100vh' }}>
            <Sider
                width={260}
                style={{
                    overflow: 'auto',
                    height: '100vh',
                    position: 'fixed',
                    left: 0,
                    top: 0,
                    bottom: 0,
                    backgroundColor: '#fff',
                    borderRight: '1px solid #f0f0f0'
                }}
            >
                <div style={{ padding: '24px', display: 'flex', alignItems: 'center', gap: '10px' }}>
                    <WalletOutlined style={{ fontSize: '24px', color: '#ff4d4f' }} />
                    <span style={{ fontWeight: 'bold', fontSize: '18px' }}>Wallet App</span>
                </div>
                <Menu mode="inline" defaultSelectedKeys={['1']} style={{ borderRight: 0 }}>
                    <Menu.Item key="1" icon={<AppstoreOutlined />}>Ana Sayfa</Menu.Item>
                    <Menu.Item key="2" icon={<SwapOutlined />}>İşlem Yap</Menu.Item>
                    <Menu.Item key="3" icon={<HistoryOutlined />}>İşlem Geçmişi</Menu.Item>
                    <Menu.Item key="4" icon={<LineChartOutlined />}>Döviz Kurları</Menu.Item>
                    <Menu.Divider />
                    <Menu.Item key="logout" icon={<LogoutOutlined />} danger onClick={handleLogout}>Çıkış Yap</Menu.Item>
                </Menu>
            </Sider>

            <Layout style={{ marginLeft: 260, backgroundColor: '#f8fafc' }}>
                <Header style={{ background: '#fff', padding: '0 32px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', borderBottom: '1px solid #f0f0f0' }}>
                    <Title level={4} style={{ margin: 0 }}>Hoş Geldin, {data?.FullName}</Title>
                    <Button type="primary" danger icon={<PlusOutlined />} onClick={() => setIsModalVisible(true)}>
                        Yeni Cüzdan
                    </Button>
                </Header>

                <Content style={{ padding: '32px', overflow: 'initial' }}>
                    <Title level={5} style={{ color: '#64748b', marginBottom: '16px' }}>Varlıklarınızın Güncel Dağılımı</Title>
                    <Row gutter={[16, 16]} style={{ marginBottom: '32px' }}>
                        {data?.CurrencySummaries.map(summary => (
                            <Col xs={24} sm={12} lg={6} key={summary.Currency}>
                                <Card bordered={false} className="stat-card">
                                    <Statistic
                                        title={`TOPLAM ${summary.Currency} BAKİYE`}
                                        value={summary.TotalBalance}
                                        precision={2}
                                        suffix={summary.Currency === 'TRY' ? '₺' : summary.Currency}
                                    />
                                </Card>
                            </Col>
                        ))}
                    </Row>

                    <Title level={4} style={{ marginBottom: '24px' }}>Cüzdanlarım</Title>
                    <Row gutter={[20, 20]}>
                        {data?.Wallets.map(wallet => (
                            <Col xs={24} md={12} lg={8} key={wallet.Id}>
                                <Card
                                    hoverable
                                    style={{ borderRadius: '12px', border: '1px solid #e2e8f0' }}
                                    actions={[
                                        <Popconfirm title="Cüzdanı silmek istediğinize emin misiniz?" onConfirm={() => deleteWallet(wallet.Id)}>
                                            <DeleteOutlined key="delete" style={{ color: '#ff4d4f' }} />
                                        </Popconfirm>,
                                        <ArrowRightOutlined key="go" onClick={() => navigate(`/wallet/${wallet.Id}`)} />
                                    ]}
                                >
                                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                                        <Space direction="vertical" size={0}>
                                            <Text type="secondary" strong style={{ fontSize: '12px' }}>#{wallet.Id}</Text>
                                            <Tag color="blue">{wallet.Currency}</Tag>
                                        </Space>
                                        <Text strong>{wallet.TypeName || "Cüzdan"}</Text>
                                    </div>
                                    <div style={{ marginTop: '20px' }}>
                                        <Title level={3} style={{ margin: 0 }}>
                                            {wallet.Currency === 'TRY' ? '₺' : ''} {wallet.Balance.toLocaleString('tr-TR', { minimumFractionDigits: 2 })} {wallet.Currency !== 'TRY' ? wallet.Currency : ''}
                                        </Title>
                                    </div>
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </Content>
            </Layout>

            <Modal
                title="Yeni Cüzdan Oluştur"
                open={isModalVisible}
                onCancel={() => setIsModalVisible(false)}
                onOk={() => form.submit()}
                okText="Oluştur"
                cancelText="İptal"
                okButtonProps={{ danger: true }}
            >
                <Form form={form} layout="vertical" onFinish={createWallet}>
                    <Form.Item name="type" label="Cüzdan Tipi" rules={[{ required: true }]}>
                        <Select placeholder="Seçiniz...">
                            <Select.Option value={1}>Vadesiz Hesap (Checking)</Select.Option>
                            <Select.Option value={2}>Vadeli Hesap (Saving)</Select.Option>
                            <Select.Option value={3}>Yatırım Hesabı (Investment)</Select.Option>
                        </Select>
                    </Form.Item>
                    <Form.Item name="currency" label="Para Birimi" rules={[{ required: true }]}>
                        <Select placeholder="Para birimi seçiniz...">
                            <Select.Option value="TRY">TRY - Türk Lirası</Select.Option>
                            {rates.map(rate => (
                                <Select.Option key={rate.CurrencyCode} value={rate.CurrencyCode}>
                                    {rate.CurrencyCode} - {rate.CurrencyCode === 'USD' ? 'Amerikan Doları' : 'Döviz'}
                                </Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                </Form>
            </Modal>
        </Layout>
    );
};

export default Dashboard;