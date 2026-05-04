import React, { useState } from 'react';
import { Typography, Card, Row, Col, Button, Statistic, Avatar, Space, Tag, Form, Spin, Popconfirm, Layout } from 'antd';
import { PlusOutlined, WalletOutlined, UserOutlined, DeleteOutlined, ArrowRightOutlined, LoadingOutlined } from '@ant-design/icons';
import { useDashboard } from '../hooks/useDashboard';
import CreateWalletModal from '../components/CreateWalletModal';
import { useTranslation } from 'react-i18next';
import { getCurrencySymbol, formatNumber } from '../utils/formatters';

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
    const { t } = useTranslation();
    const [form] = Form.useForm();
    const [isModalVisible, setIsModalVisible] = useState(false);
    const { data, loading, rates, createWallet, deleteWallet } = useDashboard(form);

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
                        {t('dash_welcome', { name: data?.fullName })}
                    </Title>
                    <Text style={{ color: '#94a3b8', fontSize: '12px' }}>{t('dash_subtitle')}</Text>
                </div>
                <Space size="large">
                    <Avatar icon={<UserOutlined />} style={{ backgroundColor: '#fff2f0', color: '#ff4d4f', border: '1px solid #ffccc7' }} />
                    <Button
                        type="primary"
                        style={customStyles.volcanoButton}
                        icon={<PlusOutlined />}
                        onClick={() => setIsModalVisible(true)}
                    >
                        {t('dash_new_wallet')}
                    </Button>
                </Space>
            </Header>

            <Content className="p-10 max-w-[1600px]">
                <div className="mb-12">
                    <Text className="text-slate-400 font-bold text-[10px] tracking-[0.2em] uppercase">{t('dash_asset_dist')}</Text>
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
                                        title={<span className="text-[10px] font-black text-slate-400 uppercase tracking-widest">{t('dash_total_currency', { currency: summary.currency })}</span>}
                                        value={summary.totalBalance}
                                        precision={2}
                                        prefix={<span className="text-volcano-500 mr-1">{getCurrencySymbol(summary.currency)}</span>}
                                        valueStyle={{ fontWeight: 900, color: '#1e293b', fontSize: '28px' }}
                                    />
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </div>

                <Text className="text-slate-400 font-bold text-[10px] tracking-[0.2em] uppercase mb-6 block">
                    {t('dash_my_wallets')}
                </Text>
                <Row gutter={[24, 24]}>
                    {data?.wallets?.map(wallet => (
                        <Col xs={24} md={12} lg={8} key={wallet.id}>
                            <Card
                                className="wallet-card-premium transition-all duration-300"
                                styles={{ body: { padding: '28px' } }}
                                actions={[
                                    <Popconfirm title={t('dash_delete_confirm_title')} onConfirm={() => deleteWallet(wallet.id)} okText={t('dash_delete_yes')} cancelText={t('dash_delete_no')}>
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
                                        {t('dash_available_balance')}
                                    </Text>
                                    <div style={{ display: 'flex', alignItems: 'baseline', gap: '6px' }}>
                                        <span style={{ fontSize: '20px', fontWeight: '300', color: '#ff4d4f' }}>
                                            {getCurrencySymbol(wallet.currency)}
                                        </span>
                                        <span style={{ fontSize: '32px', fontWeight: '900', color: '#0f172a', letterSpacing: '-1px' }}>
                                            {formatNumber(wallet.balance)}
                                        </span>
                                    </div>
                                </div>
                            </Card>
                        </Col>
                    ))}
                </Row>
            </Content>

            <CreateWalletModal
                visible={isModalVisible}
                onCancel={() => setIsModalVisible(false)}
                onFinish={(values) => createWallet(values, () => setIsModalVisible(false))}
                rates={rates}
                customStyles={customStyles}
            />

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
