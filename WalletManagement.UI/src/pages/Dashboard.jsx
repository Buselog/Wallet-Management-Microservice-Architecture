import React, { useEffect, useState } from 'react';
import { Layout, Menu, Typography, Card, Row, Col, Button, Statistic, Modal, Select, Avatar, Space, Tag, Divider, Form, message } from 'antd';
import {
    AppstoreOutlined,
    SwapOutlined,
    HistoryOutlined,
    LineChartOutlined,
    LogoutOutlined,
    PlusOutlined,
    WalletOutlined,
    UserOutlined,
    ArrowRightOutlined,
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
    const [form] = Form.useForm();
    const navigate = useNavigate();

    // 1. Veri çekme fonksiyonu
    const fetchDashboardData = async () => {
        setLoading(true);
        try {
            const token = localStorage.getItem('token');
            const customerNo = localStorage.getItem('customerNo') || "123456";

            if (!token) {
                navigate('/login');
                return;
            }

            const response = await api.get(`/Dashboard/user-summary?customerNo=${customerNo}`);
            setData(response.data);
        } catch (error) {
            console.error("Dashboard hatası:", error);
            message.error("Veriler yüklenirken bir sorun oluştu.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDashboardData();
    }, []);

    const handleLogout = () => {
        localStorage.clear();
        navigate('/login');
    };

    return (
        <Layout className="min-h-screen bg-[#f8fafc]">
            <Sider
                width={260}
                className="overflow-auto h-screen fixed left-0 top-0 bottom-0 bg-white border-r border-slate-100 z-50 shadow-sm"
            >
                <div className="p-8 flex items-center gap-3">
                    <div className="bg-volcano-400 p-2 rounded-xl shadow-lg shadow-volcano-100">
                        <WalletOutlined className="text-white text-xl" />
                    </div>
                    <span className="font-black text-lg tracking-tighter text-slate-800">WalletApp</span>
                </div>

                <Menu mode="inline" defaultSelectedKeys={['1']} className="border-none px-4 font-medium">
                    <Menu.Item key="1" icon={<AppstoreOutlined />} className="rounded-xl mb-2">Ana Sayfa</Menu.Item>
                    <Menu.Item key="2" icon={<SwapOutlined />} className="rounded-xl mb-2">İşlem Yap</Menu.Item>
                    <Menu.Item key="3" icon={<HistoryOutlined />} className="rounded-xl mb-2">İşlem Geçmişi</Menu.Item>
                    <Menu.Item key="4" icon={<LineChartOutlined />} className="rounded-xl mb-2">Kurlar</Menu.Item>

                    <div className="mt-12 px-6 mb-4 text-[10px] font-bold text-slate-400 uppercase tracking-[0.2em]">Hesap Yönetimi</div>
                    <Menu.Item key="5" icon={<LogoutOutlined />} onClick={handleLogout} className="rounded-xl text-red-500 hover:!text-red-600">Çıkış Yap</Menu.Item>
                </Menu>
            </Sider>

            <Layout className="ml-[260px] bg-transparent transition-all duration-300">
                <Header className="bg-white/80 backdrop-blur-md sticky top-0 z-40 px-10 border-b border-slate-100 flex items-center justify-between h-20 shadow-sm">
                    <div className="flex flex-col">
                        <Title level={4} className="!mb-0 !text-slate-800 font-bold">
                            Hoş Geldin, {data?.FullName?.split(' ')[0] || 'Değerli Kullanıcı'}
                        </Title>
                        <Text className="text-slate-400 text-xs font-medium">Finansal durumuna göz at.</Text>
                    </div>
                    <Space size="large" className="bg-slate-50 p-1 px-4 rounded-2xl border border-slate-100">
                        <Text className="font-bold text-slate-600">{data?.FullName || '...'}</Text>
                        <Avatar icon={<UserOutlined />} className="bg-volcano-100 text-volcano-500" />
                    </Space>
                </Header>

                <Content className="p-10 max-w-[1400px]">
                    <Row gutter={[24, 24]} className="mb-12">
                        {data?.CurrencySummaries?.map((summary, idx) => (
                            <Col xs={24} sm={12} lg={8} key={summary.Currency}>
                                <Card className="border-none shadow-xl shadow-slate-200/50 rounded-[32px] bg-white group hover:-translate-y-1 transition-all duration-300">
                                    <Statistic
                                        title={<span className="text-[10px] font-black text-slate-400 uppercase tracking-widest">TOPLAM {summary.Currency}</span>}
                                        value={summary.TotalBalance}
                                        precision={2}
                                        valueStyle={{ color: '#0f172a', fontWeight: '900', fontSize: '28px' }}
                                        prefix={<span className="text-slate-300 mr-2">{summary.Currency === 'TRY' ? '₺' : '$'}</span>}
                                    />
                                </Card>
                            </Col>
                        ))}
                    </Row>

                    <div className="flex justify-between items-end mb-8">
                        <div>
                            <div className="bg-volcano-100 text-volcano-600 px-3 py-1 rounded-full text-[10px] font-bold inline-block mb-2">CÜZDANLARIN</div>
                            <Title level={2} className="!mb-0 !text-slate-800 font-black">Varlık Listesi</Title>
                        </div>
                        <Button
                            type="primary"
                            icon={<PlusOutlined />}
                            className="h-12 px-8 rounded-2xl bg-slate-900 hover:!bg-volcano-500 border-none font-bold text-sm shadow-lg shadow-slate-200"
                            onClick={() => setIsModalVisible(true)}
                        >
                            Yeni Cüzdan
                        </Button>
                    </div>

                    <Row gutter={[24, 24]}>
                        {data?.Wallets?.map(wallet => (
                            <Col xs={24} md={12} lg={8} key={wallet.Id}>
                                <Card
                                    className="border-slate-100 rounded-[32px] shadow-sm hover:shadow-xl transition-all duration-500 group relative overflow-hidden"
                                    actions={[
                                        <div className="flex items-center justify-center gap-2 py-2 font-bold text-slate-400 hover:text-volcano-500">
                                            İşlemler <ArrowRightOutlined className="text-xs" />
                                        </div>
                                    ]}
                                >
                                    <div className="flex justify-between items-center mb-8">
                                        <div className="bg-slate-50 p-4 rounded-3xl group-hover:bg-volcano-50 transition-colors">
                                            <WalletOutlined className="text-2xl text-slate-300 group-hover:text-volcano-400" />
                                        </div>
                                        <div className="text-right">
                                            <Tag className="rounded-full border-none bg-slate-100 text-slate-500 font-black px-4 py-1">
                                                {wallet.Currency}
                                            </Tag>
                                            <div className="text-[10px] text-slate-300 mt-1 font-mono">ID: {wallet.Id}</div>
                                        </div>
                                    </div>
                                    <div className="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1">Kullanılabilir Bakiye</div>
                                    <div className="text-3xl font-black text-slate-900 tracking-tighter">
                                        {wallet.Currency === 'TRY' ? '₺' : '$'} {wallet.Balance.toLocaleString('tr-TR', { minimumFractionDigits: 2 })}
                                    </div>
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </Content>
            </Layout>

            <Modal
                title={<Title level={4} className="!mb-0 font-black">Yeni Bir Cüzdan Aç</Title>}
                open={isModalVisible}
                onCancel={() => setIsModalVisible(false)}
                footer={null}
                centered
                width={440}
                className="modern-modal"
            >
                <Form form={form} layout="vertical" className="mt-6">
                    <Form.Item label={<span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Cüzdan Tipi</span>}>
                        <Select placeholder="Örn: Yatırım Cüzdanı" size="large" className="rounded-2xl h-12 overflow-hidden border-slate-200 shadow-sm">
                            <Select.Option value="Standard">Standart Nakit Cüzdanı</Select.Option>
                            <Select.Option value="Investment">Dijital Yatırım Cüzdanı</Select.Option>
                        </Select>
                    </Form.Item>
                    <Form.Item label={<span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Para Birimi</span>}>
                        <Select placeholder="Para birimi seçiniz" size="large" className="rounded-2xl h-12 overflow-hidden border-slate-200 shadow-sm">
                            <Select.Option value="TRY">TRY - Türk Lirası</Select.Option>
                            <Select.Option value="USD">USD - Amerikan Doları</Select.Option>
                            <Select.Option value="EUR">EUR - Euro</Select.Option>
                        </Select>
                    </Form.Item>
                    <Button type="primary" block size="large" className="h-14 bg-volcano-400 hover:!bg-volcano-500 border-none rounded-2xl font-bold text-base shadow-lg shadow-volcano-100 mt-6">
                        Cüzdanı Oluştur
                    </Button>
                </Form>
            </Modal>
        </Layout>
    );
};

export default Dashboard;