import React from 'react';
import { Form, Input, Button, Typography, Row, Col, Alert } from 'antd';
import { MailOutlined, LockOutlined, WalletOutlined, ArrowRightOutlined } from '@ant-design/icons';
import registerImg from '../assets/register-page.jpg';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { useTranslation } from 'react-i18next';
import LanguageSwitcher from '../components/LanguageSwitcher';

const { Title, Text } = Typography;

const Register = () => {
    const { t } = useTranslation();
    const [form] = Form.useForm();
    const navigate = useNavigate();

    const { loading, generalError, register } = useAuth(form);

    return (
        <div className="flex h-screen items-center justify-center bg-slate-50 p-4 overflow-hidden relative">
            <div className="absolute top-6 right-8 z-50">
                <LanguageSwitcher />
            </div>
            <div className="bg-white rounded-3x1 shadow-2xl overflow-hidden max-w-5xl w-full flex flex-col md:flex-row max-h-[99vh]">

                <div className="w-full md:w-1/2 p-8 flex flex-col justify-center">
                    <div className="mb-0">
                        <div className="flex items-center gap-2 mb-1">
                            <div className="bg-volcano-400 p-1.5 rounded-lg text-white flex items-center justify-center">
                                <WalletOutlined className="text-lg" />
                            </div>
                            <span className="font-bold text-lg tracking-tight text-slate-800">WalletManagement</span>
                        </div>
                        <Title level={2} className="!mb-0 !text-slate-900">{t('register_title')}</Title>
                        <Text className="text-slate-500 text-xs">{t('register_subtitle')}</Text>

                        {generalError && (
                            <div className="mt-4">
                                <Alert message={generalError} type="error" showIcon closable />
                            </div>
                        )}
                    </div>

                    <Form
                        form={form}
                        name="register"
                        layout="vertical"
                        onFinish={register}
                        requiredMark={false}
                    >
                        <Row gutter={12}>
                            <Col span={12}>
                                <Form.Item
                                    label={<span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">{t('first_name_label')}</span>}
                                    name="firstName"
                                    className="!mb-3"
                                    rules={[{ required: true, message: t('first_name_required') }]}
                                >
                                    <Input placeholder={t('first_name_placeholder')} size="middle" className="rounded-lg h-10 border-slate-200" />
                                </Form.Item>
                            </Col>
                            <Col span={12}>
                                <Form.Item
                                    label={<span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">{t('last_name_label')}</span>}
                                    name="lastName"
                                    className="!mb-3"
                                    rules={[{ required: true, message: t('last_name_required') }]}
                                >
                                    <Input placeholder={t('last_name_placeholder')} size="middle" className="rounded-lg h-10 border-slate-200" />
                                </Form.Item>
                            </Col>
                        </Row>

                        <Form.Item
                            label={<span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">{t('email_label')}</span>}
                            name="email"
                            className="!mb-3"
                            rules={[
                                { required: true, message: t('email_required') },
                                { type: 'email', message: t('email_invalid') }
                            ]}
                        >
                            <Input
                                prefix={<MailOutlined className="text-slate-400" />}
                                placeholder={t('email_placeholder')}
                                className="rounded-lg h-10 border-slate-200"
                            />
                        </Form.Item>

                        <Form.Item
                            label={<span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">{t('phone_label')}</span>}
                            name="phoneNumber"
                            className="!mb-3"
                            rules={[{ required: true, message: t('phone_required') }]}
                        >
                            <Input
                                placeholder="5xx xxx xx xx"
                                className="rounded-lg h-10 border-slate-200"
                            />
                        </Form.Item>

                        <Form.Item
                            label={<span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">{t('password_label')}</span>}
                            name="password"
                            className="!mb-4"
                            rules={[
                                { required: true, message: t('password_required') },
                                { min: 8, message: t('password_min') }
                            ]}
                        >
                            <Input.Password
                                prefix={<LockOutlined className="text-slate-400" />}
                                placeholder="••••••••"
                                className="rounded-lg h-10 border-slate-200"
                            />
                        </Form.Item>

                        <Form.Item className="!mb-2">
                            <Button
                                type="primary"
                                htmlType="submit"
                                block
                                loading={loading}
                                className="h-10 !bg-volcano-400 !hover:bg-volcano-600 rounded-lg font-bold text-sm flex items-center justify-center gap-2"
                            >
                                {t('register_button')} <ArrowRightOutlined />
                            </Button>
                        </Form.Item>

                        <div className="text-center text-slate-500 text-xs">
                            {t('already_have_account')} <Button type="link" className="p-0 text-xs font-bold !text-volcano-600" onClick={() => navigate('/login')}>{t('login_link')}</Button>
                        </div>
                    </Form>
                </div>

                <div className="hidden md:flex md:w-1/2 bg-slate-50 p-12 flex-col items-center justify-center relative">
                    <div className="absolute top-0 right-0 w-32 h-32 bg-blue-100 rounded-full -mr-16 -mt-16 opacity-50"></div>
                    <div className="relative z-10 w-full max-w-sm mb-5 transform hover:scale-105 transition-transform duration-500">
                        <img
                            src={registerImg}
                            alt="Register Image"
                            className="w-full h-auto drop-shadow-2xl rounded-3xl"
                        />
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

export default Register;