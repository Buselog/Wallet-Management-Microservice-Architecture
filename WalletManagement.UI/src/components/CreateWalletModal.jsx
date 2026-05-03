import React from 'react';
import { Modal, Form, Select, Button, Typography } from 'antd';

const { Title, Text } = Typography;

const CreateWalletModal = ({ visible, onCancel, onFinish, rates, customStyles }) => {
    return (
        <Modal
            title={<Title level={3} style={{ margin: 0, fontWeight: '900' }}>Yeni Cüzdan Oluştur</Title>}
            open={visible}
            onCancel={onCancel}
            footer={null}
            centered
            width={480}
            styles={{ mask: { backdropFilter: 'blur(10px)' }, content: { borderRadius: '32px' } }}
        >
            <Form layout="vertical" onFinish={onFinish} style={{ marginTop: '32px' }}>
                <Form.Item name="type" label={<Text style={{ fontSize: '11px', fontWeight: '900', color: '#94a3b8' }}>CÜZDAN TİPİ</Text>} rules={[{ required: true }]}>
                    <Select size="large" placeholder="Hesap türünü belirleyin">
                        <Select.Option value={1}>Vadesiz Hesap</Select.Option>
                        <Select.Option value={2}>Vadeli Hesap</Select.Option>
                        <Select.Option value={3}>Yatırım Hesabı</Select.Option>
                    </Select>
                </Form.Item>

                <Form.Item name="currency" label={<Text style={{ fontSize: '11px', fontWeight: '900', color: '#94a3b8' }}>PARA BİRİMİ</Text>} rules={[{ required: true }]}>
                    <Select size="large" placeholder="İşlem yapılacak birimi seçin">
                        <Select.Option value="TRY">TRY - Türk Lirası</Select.Option>
                        {rates.map(r => (
                            <Select.Option key={r.currencyCode} value={r.currencyCode}>{r.currencyCode}</Select.Option>
                        ))}
                    </Select>
                </Form.Item>

                <Button type="primary" block htmlType="submit" style={customStyles.volcanoButton}>
                    Cüzdanı Aktifleştir
                </Button>
            </Form>
        </Modal>
    );
};

export default CreateWalletModal;