import React from 'react';
import { Form, Input, Button, Typography, Alert } from 'antd';
import { MailOutlined, LockOutlined, WalletOutlined, ArrowRightOutlined } from '@ant-design/icons';
import loginImg from '../assets/login-page.jpg'
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { useTranslation } from 'react-i18next'

const { Title, Text } = Typography;

const Login = () => {
    const { t } = useTranslation();
    const [form] = Form.useForm();
    const navigate = useNavigate();

    const { loading, generalError, login } = useAuth(form);

    return (
        <div className="flex h-screen items-center justify-center bg-slate-50 p-4 overflow-hidden relative">
            <div className="absolute top-6 right-8 z-50">
                <LanguageSwitcher />
            </div>
            <div className="bg-white rounded-3x1 shadow-2xl overflow-hidden max-w-5xl w-full flex flex-col md:flex-row max-h-[98vh]">

                <div className="w-full md:w-1/2 p-12 flex flex-col justify-center">
                    <div className="mb-8">
                        <div className="flex items-center gap-2 mb-6">
                            <div className="bg-volcano-400 p-2 rounded-lg text-white flex items-center justify-center">
                                <WalletOutlined className="text-xl" />
                            </div>
                            <span className="font-bold text-xl tracking-tight text-slate-800">WalletManagement</span>
                        </div>
                        <Title level={1} className="!mb-2 !text-slate-900">{t('login_title')}</Title>
                        <Text className="text-slate-500">{t('login_subtitle')}</Text>

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
                        onFinish={login}
                        requiredMark={false}>

                        <Form.Item
                            label={<span className="text-xs font-bold text-slate-400 uppercase tracking-wider">{t('email_label')}</span>}
                            name="email"
                            rules={[{ required: true, message: t('email_required') }, { type: 'email', message: t('email_invalid') }]}
                        >
                            <Input
                                prefix={<MailOutlined className="text-slate-400" />}
                                placeholder={t('email_placeholder')}
                                size="large"
                                className="rounded-xl h-12 border-slate-200"
                            />
                        </Form.Item>

                        <Form.Item
                            label={<span className="text-xs font-bold text-slate-400 uppercase tracking-wider">{t('password_label')}</span>}
                            name="password"
                            rules={[{ required: true, message: t('password_required') }]}
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
                                {t('login_button')} <ArrowRightOutlined />
                            </Button>
                        </Form.Item>

                        <div className="text-center text-slate-500 mt-4">
                            {t('no_account')} <Button type="link" className="p-0 font-bold !text-volcano-600" onClick={() => navigate('/register')}>{t('register_link')}</Button>
                        </div>
                    </Form>
                </div>

                <div className="hidden md:flex md:w-1/2 bg-slate-50 p-12 flex-col items-center justify-center relative">
                    <div className="absolute top-0 right-0 w-32 h-32 bg-blue-100 rounded-full -mr-16 -mt-16 opacity-50"></div>
                    <div className="relative z-10 w-full max-w-sm mb-5 transform hover:scale-105 transition-transform duration-500">
                        <img src={loginImg} alt="Login" className="w-full h-auto drop-shadow-2xl rounded-3xl" />
                    </div>
                    <div className="text-center relative z-10">
                        <Title level={3} className="!text-slate-800 !mb-2">{t('hero_title')}</Title>
                        <Text className="text-slate-500 text-base">{t('hero_subtitle')}</Text>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default Login;