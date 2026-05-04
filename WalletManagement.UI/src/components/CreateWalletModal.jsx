import React from 'react';
import { Modal, Form, Select, Button, Typography } from 'antd';
import { useTranslation } from 'react-i18next';
import { getCurrencyName, getCurrencySymbol } from '../utils/formatters';

const { Title, Text } = Typography;

const CreateWalletModal = ({ visible, onCancel, onFinish, rates, customStyles }) => {

    const { t } = useTranslation();

    return (
        <Modal
            title={<Title level={3} style={{ margin: 0, fontWeight: '900' }}>{t('modal_wallet_title')}</Title>}
            open={visible}
            onCancel={onCancel}
            footer={null}
            centered
            width={480}
            styles={{ mask: { backdropFilter: 'blur(10px)' }, content: { borderRadius: '32px' } }}
        >
            <Form layout="vertical" onFinish={onFinish} style={{ marginTop: '32px' }}>
                <Form.Item name="type" label={<Text style={{ fontSize: '11px', fontWeight: '900', color: '#94a3b8' }}>{t('modal_wallet_type_label')}</Text>} rules={[{ required: true }]}>
                    <Select size="large" placeholder={t('modal_wallet_type_placeholder')}>
                        <Select.Option value={1}>{t('wallet_type_1')}</Select.Option>
                        <Select.Option value={2}>{t('wallet_type_2')}</Select.Option>
                        <Select.Option value={3}>{t('wallet_type_3')}</Select.Option>
                    </Select>
                </Form.Item>

                <Form.Item name="currency" label={<Text style={{ fontSize: '11px', fontWeight: '900', color: '#94a3b8' }}>{t('modal_currency_label')}</Text>} rules={[{ required: true }]}>
                    <Select size="large" placeholder={t('modal_currency_placeholder')}>
                        <Select.Option value="TRY">
                            <span style={{ marginRight: '8px' }}>{getCurrencySymbol('TRY')}</span>
                            {getCurrencyName('TRY')}
                        </Select.Option>
                        {rates.map(r => (
                            <Select.Option key={r.currencyCode} value={r.currencyCode}>
                                <span style={{ marginRight: '8px', color: '#ff4d4f', fontWeight: 'bold' }}>
                                    {getCurrencySymbol(r.currencyCode)}
                                </span>
                                {getCurrencyName(r.currencyCode)}
                            </Select.Option>
                        ))}
                    </Select>
                </Form.Item>

                <Button type="primary" block htmlType="submit" style={customStyles.volcanoButton}>
                    {t('modal_wallet_submit')}
                </Button>
            </Form>
        </Modal>
    );
};

export default CreateWalletModal;