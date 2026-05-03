import React from 'react';
import { Typography, Table, Card, Button, Space, Tag, Avatar } from 'antd';
import { UserOutlined, ReloadOutlined, SafetyCertificateOutlined } from '@ant-design/icons';
import { useInvestment } from '../hooks/useInvestment';
import TradeModal from '../components/TradeModal';
import { Form } from 'antd';

const { Title, Text } = Typography;

const Investment = () => {
    const [form] = Form.useForm();
    const {
        rates, wallets, loading, modalVisible, setModalVisible,
        selectedRate, tradeType, userData, lastUpdate,
        fetchData, openTrade, executeTrade
    } = useInvestment(form);

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
        <>
            <div className="my-6 mx-8 flex justify-between items-end">
                <div>
                    <Title level={3} style={{ margin: 0, fontWeight: 900, color: '#1e293b' }}>Yatırım Merkezi</Title>
                    <Text type="secondary" className="text-xs">Merkez Bankası verileriyle anlık ve güvenli döviz ticareti.</Text>
                </div>
                <div className="text-right">
                    <Button icon={<ReloadOutlined />} onClick={fetchData} loading={loading} className="btn-refresh-custom">Verileri Yenile</Button>
                    <div className="mt-1"><Text className="text-[10px] text-slate-400 font-bold uppercase tracking-widest">Son Güncelleme: {lastUpdate}</Text></div>
                </div>
            </div>

            <div className="px-8 flex-1 flex flex-col min-h-0">
                <Card className="rounded-[32px] border-none shadow-sm flex-1 overflow-hidden" styles={{ body: { height: '100%', padding: 0 } }}>
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
            </div>

            <TradeModal
                visible={modalVisible}
                onCancel={() => setModalVisible(false)}
                tradeType={tradeType}
                selectedRate={selectedRate}
                wallets={wallets}
                onFinish={executeTrade}
                loading={loading}
                form={form}
            />

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
                .btn-refresh-custom { background-color: #fff2f0 !important; color: #ff4d4f !important; border-color: #ffccc7 !important; font-weight: 700 !important; border-radius: 12px !important; transition: all 0.3s ease !important; }
                .btn-refresh-custom:hover { background-color: #ff4d4f !important; color: white !important; border-color: #ff4d4f !important; box-shadow: 0 4px 12px rgba(255, 77, 79, 0.2) !important; }
                .custom-investment-table .ant-table-measure-row { visibility: collapse !important; line-height: 0 !important; height: 0 !important; }
                .custom-investment-table .ant-table-measure-row td { padding: 0 !important; border: 0 !important; height: 0 !important; }
            `}} />
        </>
    );
};

export default Investment;