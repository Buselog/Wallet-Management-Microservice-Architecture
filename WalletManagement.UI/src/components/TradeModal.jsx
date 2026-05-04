import React from 'react';
import { Modal, Form, InputNumber, Select, Row, Col, Button, Typography } from 'antd';
import { TransactionOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { getCurrencySymbol, formatNumber } from '../utils/formatters';

const { Title, Text } = Typography;

const TradeModal = ({ visible, onCancel, tradeType, selectedRate, wallets, onFinish, loading, form }) => {
    const { t, i18n } = useTranslation();
    const langCode = i18n.language === 'tr' ? 'tr-TR' : 'en-US';

    const handleAmountChange = (value) => {
        const amt = value || 0;
        const rate = tradeType === 'BUY' ? selectedRate?.sellingRate : selectedRate?.buyingRate;
        const total = amt * rate;
        const el = document.getElementById('total-calc');
        if (el) {
            el.innerText = `₺ ${total.toLocaleString(langCode, { minimumFractionDigits: 2 })}`;
        }
    };

    return (
        <Modal
            title={null}
            open={visible}
            onCancel={onCancel}
            footer={null}
            centered
            width={650}
            className="trade-modal"
            styles={{ body: { padding: '20px 24px' } }}
        >
            <div className="text-center mb-4">
                <div className={`trade-icon-circle ${tradeType === 'BUY' ? 'buy' : 'sell'}`} style={{ width: '48px', height: '48px', marginBottom: '8px' }}>
                    <TransactionOutlined style={{ fontSize: '20px' }} />
                </div>
                <Title level={4} style={{ margin: 0, fontWeight: '900', fontSize: '18px' }}>
                    {selectedRate?.currencyCode} {tradeType === 'BUY' ? t('trade_buy_title') : t('trade_sell_title')}
                </Title>
                <Text className="text-slate-400 text-[10px] uppercase tracking-widest font-black">
                    {t('trade_rate_label')}: ₺{tradeType === 'BUY' ? selectedRate?.sellingRate : selectedRate?.buyingRate}
                </Text>
            </div>

            <Form form={form} layout="vertical" onFinish={onFinish} requiredMark={false}>
                <Form.Item
                    name="amount"
                    label={<span className="text-[10px] font-black uppercase text-slate-400 tracking-widest">{t('trade_amount_label')}</span>}
                    rules={[{ required: true, message: t('trade_amount_required') }]}
                    style={{ marginBottom: '12px' }}
                >
                    <InputNumber
                        className="w-full h-10 rounded-xl flex items-center font-black text-base"
                        style={{ width: '100%' }}
                        placeholder="0.00"
                        min={0.01}
                        onChange={handleAmountChange}
                    />
                </Form.Item>

                <Row gutter={12}>
                    <Col span={12}>
                        <Form.Item
                            name="sourceWalletId"
                            label={<span className="text-[10px] font-black uppercase text-slate-400 tracking-widest">{t('trade_source_wallet')}</span>}
                            rules={[{ required: true, message: t('trade_select_required') }]}
                            style={{ marginBottom: '12px' }}
                        >
                            <Select size="middle" className="rounded-lg" placeholder={t('trade_payment_placeholder')}>
                                {wallets.map(w => (
                                    <Select.Option key={w.id} value={w.id}>
                                        {w.currency} - {t(`wallet_type_${w.type}`)} ({getCurrencySymbol(w.currency)}{formatNumber(w.balance)})
                                    </Select.Option>
                                ))}
                            </Select>
                        </Form.Item>
                    </Col>
                    <Col span={12}>
                        <Form.Item
                            name="targetWalletId"
                            label={<span className="text-[10px] font-black uppercase text-slate-400 tracking-widest">{t('trade_target_wallet')}</span>}
                            rules={[{ required: true, message: t('trade_select_required') }]}
                            style={{ marginBottom: '12px' }}
                        >
                            <Select size="middle" className="rounded-lg" placeholder={t('trade_investment_placeholder')}>
                                {wallets.map(w => (
                                    <Select.Option key={w.id} value={w.id}>
                                        {w.currency} - {t(`wallet_type_${w.type}`)} ({getCurrencySymbol(w.currency)}{formatNumber(w.balance)})
                                    </Select.Option>
                                ))}
                            </Select>
                        </Form.Item>
                    </Col>
                </Row>

                <div className="p-3 bg-slate-50 rounded-xl border border-slate-100 mb-5">
                    <div className="flex justify-between items-center">
                        <Text className="text-[10px] font-black text-slate-500 uppercase">{t('trade_est_total')}</Text>
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
                    {tradeType === 'BUY' ? t('trade_confirm_buy') : t('trade_confirm_sell')}
                </Button>
            </Form>
        </Modal>
    );
};

export default TradeModal;