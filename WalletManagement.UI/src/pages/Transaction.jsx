import React from 'react';
import { Typography, Card, Button, Form, Input, Select, Radio, Avatar, InputNumber, Row, Col, Space, Tag } from 'antd';
import { WalletOutlined, UserOutlined, PlusCircleOutlined, SafetyCertificateOutlined, ClockCircleOutlined } from '@ant-design/icons';
import { useTransaction } from '../hooks/useTransaction';
import { useTranslation } from 'react-i18next';
import { getCurrencySymbol, formatNumber } from '../utils/formatters';

const { Title, Text } = Typography;
const walletTypeMap = { 1: 'Vadesiz Hesap', 2: 'Vadeli Hesap', 3: 'Yatırım Hesabı' };

const Transaction = () => {
    const { t } = useTranslation();
    const [form] = Form.useForm();
    const {
        wallets,
        loading,
        selectedWallet,
        actionType,
        setActionType,
        userData,
        lastTransactionDate,
        handleWalletChange,
        executeTransaction
    } = useTransaction(form);

    return (
        <>
            <div className="ml-5 mb-5 mt-3 flex justify-between items-center">
                <div>
                    <Title level={3} style={{ margin: 0, fontWeight: 900 }}>{t('trans_title')}</Title>
                    <Text type="secondary" className="text-xs">{t('trans_subtitle')}</Text>
                </div>
                <Space className="mr-7">
                    <Avatar icon={<UserOutlined />} style={{ backgroundColor: '#fff2f0', color: '#ff4d4f' }} />
                    <Text className="font-bold text-slate-700">{userData?.fullName}</Text>
                </Space>
            </div>

            <Row gutter={24} className="flex-1 overflow-hidden mb-8 px-4" align="stretch">
                <Col xs={24} lg={10} style={{ display: 'flex', flexDirection: 'column' }}>
                    <Card className="rounded-[32px] border-none shadow-sm flex-1 flex flex-col" style={{ background: 'linear-gradient(135deg, #fffbf8 0%, #f9f0ea 100%)', border: '1px solid #feede4' }}>
                        <div className="flex-1 flex flex-col justify-center items-center py-4">
                            <div className="bg-white p-4 rounded-3xl shadow-sm mb-4 border border-orange-50">
                                <WalletOutlined style={{ fontSize: '32px', color: '#ff4d4f' }} />
                            </div>
                            <Text className="text-slate-400 block uppercase tracking-widest text-[10px] font-black">{t('trans_balance_label')}</Text>
                            <div className="mt-2">
                                <Title level={1} className="m-0 font-black text-slate-800" style={{ fontSize: '42px', lineHeight: 1 }}>
                                    {selectedWallet
                                        ? `${getCurrencySymbol(selectedWallet.currency)} ${formatNumber(selectedWallet.balance)}`
                                        : '---'}
                                </Title>
                            </div>
                            <Tag color="volcano" className="mt-6 px-6 py-1 rounded-full font-bold border-none text-[11px] uppercase tracking-wider">
                                {selectedWallet
                                    ? `${selectedWallet.currency} - ${t(`wallet_type_${selectedWallet.type}`)}`
                                    : t('trans_no_wallet')}
                            </Tag>
                        </div>
                        <div className="mt-auto pt-6 border-t border-orange-100 flex gap-4 text-left">
                            <div className="flex-1">
                                <Text className="text-slate-400 block text-[9px] font-black uppercase tracking-tighter">{t('trans_status_label')}</Text>
                                <Text className="text-green-600 font-bold text-xs"><PlusCircleOutlined className="mr-1" /> {t('trans_status_active')}</Text>
                            </div>
                            <div className="flex-1">
                                <Text className="text-slate-400 block text-[9px] font-black uppercase tracking-tighter">{t('trans_last_trans')}</Text>
                                <Text className="text-slate-700 font-bold text-xs"><ClockCircleOutlined className="mr-1" /> {lastTransactionDate}</Text>
                            </div>
                        </div>
                    </Card>
                </Col>

                <Col xs={24} lg={14} style={{ display: 'flex', flexDirection: 'column' }}>
                    <Form form={form} layout="vertical" onFinish={executeTransaction} requiredMark={false} className="flex-1 flex flex-col h-full min-h-0">
                        <Card
                            className="rounded-[32px] shadow-sm border-none flex-1 overflow-hidden min-h-0"
                            styles={{ body: { height: '100%', display: 'flex', flexDirection: 'column', padding: '24px', overflow: 'hidden', minHeight: 0 } }}
                        >
                            <div className="flex-1 pr-2 custom-scrollbar" style={{ overflowY: actionType === 'transfer' ? 'auto' : 'hidden', minHeight: 0, maxHeight: '100%' }}>
                                <div className="mb-6">
                                    <Text className="text-slate-400 block uppercase tracking-widest text-[10px] font-black mb-4">{t('trans_settings')}</Text>
                                    <Form.Item name="walletId" label={<span className="text-[11px] font-black uppercase text-slate-500">{t('trans_source_wallet')}</span>} rules={[{ required: true, message: t('trans_select_placeholder') }]}>
                                        <Select size="large" className="custom-select-large" placeholder={t('trans_select_placeholder')} onChange={handleWalletChange}>
                                            {wallets.map(w => <Select.Option key={w.id} value={w.id}>{w.currency} - {t(`wallet_type_${w.type}`)}</Select.Option>)}
                                        </Select>
                                    </Form.Item>
                                    <Form.Item label={<span className="text-[11px] font-black uppercase text-slate-500">{t('trans_type_label')}</span>}>
                                        <Radio.Group value={actionType} onChange={(e) => setActionType(e.target.value)} className="w-full flex p-1 bg-slate-50 rounded-2xl border border-slate-100" style={{ display: 'flex' }}>
                                            <Radio.Button value="deposit" className="flex-1 flex justify-center items-center rounded-xl h-11 font-black text-xs custom-radio">{t('trans_type_deposit')}</Radio.Button>
                                            <Radio.Button value="withdraw" className="flex-1 flex justify-center items-center rounded-xl h-11 font-black text-xs custom-radio">{t('trans_type_withdraw')}</Radio.Button>
                                            <Radio.Button value="transfer" className="flex-1 flex justify-center items-center rounded-xl h-11 font-black text-xs custom-radio">{t('trans_type_transfer')}</Radio.Button>
                                        </Radio.Group>
                                    </Form.Item>
                                    <Row gutter={16}>
                                        <Col span={12}>
                                            <Form.Item name="amount" label={<span className="text-[11px] font-black uppercase text-slate-500">{t('trans_amount_label')}</span>} rules={[{ required: true, message: t('trans_amount_placeholder') }]}>
                                                <InputNumber size="large" className="custom-input-number" prefix={getCurrencySymbol(selectedWallet?.currency)} placeholder="0.00" min={0.01} style={{ width: '100%' }} />
                                            </Form.Item>
                                        </Col>
                                        <Col span={12}>
                                            <Form.Item name="referenceId" label={<span className="text-[11px] font-black uppercase text-slate-500">{t('trans_ref_label')}</span>} rules={[{ required: true, message: t('trans_ref_placeholder') }]}>
                                                <Input size="large" className="rounded-xl" placeholder="REF-451" />
                                            </Form.Item>
                                        </Col>
                                    </Row>
                                </div>

                                {actionType === 'transfer' && (
                                    <div className="p-6 bg-orange-50/40 rounded-2xl border border-dashed border-orange-200 mb-6 animate-slide-down">
                                        <Text className="text-orange-500 block uppercase tracking-widest text-[9px] font-black mb-4">{t('trans_receiver_details')}</Text>
                                        <Row gutter={16}>
                                            <Col span={12}>
                                                <Form.Item name="target" label={<span className="text-[11px] font-bold text-orange-700">{t('trans_receiver_label')}</span>} rules={[{ required: true, message: t('trans_receiver_placeholder') }]}>
                                                    <Input size="large" className="rounded-xl border-orange-200" placeholder="TR... / 05..." />
                                                </Form.Item>
                                            </Col>
                                            <Col span={12}>
                                                <Form.Item name="description" label={<span className="text-[11px] font-bold text-orange-700">{t('trans_desc_label')}</span>} className="mb-0">
                                                    <Input size="large" className="rounded-xl border-orange-200" placeholder={t('trans_desc_placeholder')} />
                                                </Form.Item>
                                            </Col>
                                        </Row>
                                        <div className="mt-6">
                                            <Button type="primary" htmlType="submit" block loading={loading} className="h-12 rounded-2xl font-black text-xs tracking-widest shadow-xl" style={{ background: '#ff4d4f', border: 'none' }}>
                                                {t('trans_confirm_transfer')}
                                            </Button>
                                        </div>
                                    </div>
                                )}
                            </div>

                            {actionType !== 'transfer' && (
                                <div className="mt-auto pt-4 bg-white border-t border-slate-50">
                                    <Button type="primary" htmlType="submit" block loading={loading} className="h-14 rounded-2xl font-black text-sm tracking-widest shadow-xl" style={{ background: '#ff4d4f', border: 'none' }}>
                                        İ{t('trans_confirm_action')}
                                    </Button>
                                    <div className="mt-4 text-center">
                                        <Space className="text-slate-400 text-[10px] uppercase font-bold tracking-widest">
                                            <SafetyCertificateOutlined className="text-green-500" /> {t('trans_ssl_text')}
                                        </Space>
                                    </div>
                                </div>
                            )}
                        </Card>
                    </Form>
                </Col>
            </Row>

            <style dangerouslySetInnerHTML={{
                __html: `
                .custom-radio { border: none !important; background: #f7f6f4 !important; color: #94a3b8; transition: all 0.3s; text-align: center; }
                .ant-radio-button-wrapper-checked.custom-radio { background: #ffe0dd !important; color: #ff4d4f !important; box-shadow: 0 4px 12px rgba(255, 77, 79, 0.15) !important; border-radius: 12px !important; }
                .ant-radio-button-wrapper-checked.custom-radio:hover, .ant-radio-button-wrapper:hover { color: #ff4d4f !important; }
                .ant-radio-button-wrapper:before { display: none !important; }
                .custom-select-large .ant-select-selector { border-radius: 14px !important; height: 48px !important; display: flex !important; align-items: center !important; }
                .custom-scrollbar::-webkit-scrollbar { width: 4px; }
                .custom-scrollbar::-webkit-scrollbar-thumb { background: #ffe8db; border-radius: 10px; }
                .animate-slide-down { animation: slideDown 0.3s ease-out; }
                @keyframes slideDown { from { opacity: 0; transform: translateY(-5px); } to { opacity: 1; transform: translateY(0); } }
                .ant-card-body { padding: 20px !important; display: flex !important; flex-direction: column !important; }
                .ant-form-item { margin-bottom: 12px !important; }
            `}} />
        </>
    );
};

export default Transaction;