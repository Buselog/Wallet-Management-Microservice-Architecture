import React, { useState } from 'react';
import { Form, Input, Button, Row, Col, Typography, message, Alert } from 'antd';
import { volcano } from '@ant-design/colors';
import { MailOutlined, LockOutlined, WalletOutlined, ArrowRightOutlined } from '@ant-design/icons';
import loginImg from '../assets/login-page.jpg'
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const { Title, Text } = Typography;

const Login = () => {
    const [loading, setLoading] = useState(false);
    const [form] = Form.useForm();
    const [generalError, setGeneralError] = useState(null);
    const navigate = useNavigate();

    const onFinish = async (values) => {
        setLoading(true);
        setGeneralError(null);
        try {
            const response = await api.post('/Auth/login', values);
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('customerNo', response.data.customerNo);
            message.success('Hesabınıza erişim sağlandı. Yönlendiriliyorsunuz..');
            navigate('/dashboard');
        } catch (error) {
            const status = error.response?.status;
            const responseData = error.response?.data;
            const errorMsg = responseData?.Message || responseData?.message;

            if (errorMsg) {
                if (status === 400) {
                    const parts = errorMsg.split(' | ');
                    const fields = [];

                    parts.forEach(part => {
                        if (part.includes(':')) {
                            const [property, msg] = part.split(':');
                            const fieldName = property.toLowerCase().includes('EMAIL') ? 'email' : 'password';
                            fields.push({ name: fieldName, errors: [msg] });
                        } else {
                            setGeneralError(part);
                        }
                    });
                    if (fields.length > 0) form.setFields(fields);
                }
                else {
                    setGeneralError(errorMsg);
                }
            } else {
                setGeneralError("Giriş yapılırken bir sorun oluştu.");
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex h-screen items-center justify-center bg-slate-50 p-4 overflow-hidden">
            <div className="bg-white rounded-3x1 shadow-2xl overflow-hidden max-w-5xl w-full flex flex-col md:flex-row max-h-[98vh]">

                <div className="w-full md:w-1/2 p-12 flex flex-col justify-center">
                    <div className="mb-8">
                        <div className="flex items-center gap-2 mb-6">
                            <div className="bg-volcano-400 p-2 rounded-lg text-white flex items-center justify-center">
                                <WalletOutlined className="text-xl" />
                            </div>
                            <span className="font-bold text-xl tracking-tight text-slate-800">WalletManagement</span>
                        </div>
                        <Title level={1} className="!mb-2 !text-slate-900">Giriş Yap</Title>
                        <Text className="text-slate-500">Dijital asistanına bağlan ve cüzdanını yönet.</Text>

                        {generalError && (
                            <div className="mt-4">
                                <Alert message={generalError} type="error" showIcon closable />
                            </div>
                        )}
                    </div>

                    <Form
                        form={form}
                        name="login"
                        layout="vertical"
                        onFinish={onFinish}
                        requiredMark={false}>
                        <Form.Item
                            label={<span className="text-xs font-bold text-slate-400 uppercase tracking-wider">E-POSTA</span>}
                            name="email"
                            rules={[{ required: true, message: 'E-posta alanı boş bırakılamaz.' }, { type: 'email', message: 'Geçerli bir e-posta giriniz.' }]}
                        >
                            <Input
                                prefix={<MailOutlined className="text-slate-400" />}
                                placeholder="example@gmail.com"
                                size="large"
                                className="rounded-xl h-12 border-slate-200"
                            />
                        </Form.Item>

                        <Form.Item
                            label={<span className="text-xs font-bold text-slate-400 uppercase tracking-wider">ŞİFRE</span>}
                            name="password"
                            rules={[{ required: true, message: 'Şifre alanı boş bırakılamaz.' }]}
                        >
                            <Input.Password
                                prefix={<LockOutlined className="text-slate-400" />}
                                placeholder="••••••••"
                                size="large"
                                className="rounded-xl h-12 border-slate-200"
                            />
                        </Form.Item>

                        <Form.Item className="mt-8">
                            <Button
                                type="primary"
                                htmlType="submit"
                                block
                                size="large"
                                loading={loading}
                                className="h-12 !bg-volcano-400 !hover:bg-volcano-600 rounded-xl font-bold text-base flex items-center justify-center gap-2"
                            >
                                Giriş Yap <ArrowRightOutlined />
                            </Button>
                        </Form.Item>

                        <div className="text-center text-slate-500 mt-4">
                            Henüz hesabın yok mu? <Button type="link" className="p-0 font-bold !text-volcano-600" onClick={() => navigate('/register')}>Kaydol</Button>
                        </div>
                    </Form>
                </div>

                <div className="hidden md:flex md:w-1/2 bg-slate-50 p-12 flex-col items-center justify-center relative">
                    <div className="absolute top-0 right-0 w-32 h-32 bg-blue-100 rounded-full -mr-16 -mt-16 opacity-50"></div>

                    <div className="relative z-10 w-full max-w-sm mb-5 transform hover:scale-105 transition-transform duration-500">
                        <img
                            src={loginImg}
                            alt="Login Image"
                            className="w-full h-auto drop-shadow-2xl rounded-3xl"
                        />
                    </div>

                    <div className="text-center relative z-10">
                        <Title level={3} className="!text-slate-800 !mb-2">Finansal Geleceğini Yönet</Title>
                        <Text className="text-slate-500 text-base">Tüm varlıklarını tek bir güvenli noktadan takip et, verimliliğini artır.</Text>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default Login;