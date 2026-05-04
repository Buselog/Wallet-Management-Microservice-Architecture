import React from 'react';
import { Layout, Menu, Typography } from 'antd';
import { AppstoreOutlined, SwapOutlined, HistoryOutlined, LineChartOutlined, LogoutOutlined, WalletOutlined } from '@ant-design/icons';
import { useNavigate, useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import LanguageSwitcher from '../components/LanguageSwitcher';

const { Sider, Content } = Layout;

const brandLogoStyle = {
    background: 'linear-gradient(135deg, #fb6365 0%, #c2494f 100%)',
    boxShadow: '0 8px 16px rgba(255, 77, 79, 0.25)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: '14px',
    width: '40px',
    height: '40px'
};

const MainLayout = ({ children }) => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const location = useLocation();

    const getSelectedKey = () => {
        const path = location.pathname;
        if (path === '/dashboard') return '1';
        if (path === '/transaction') return '2';
        if (path === '/history') return '3';
        if (path === '/investment') return '4';
        return '1';
    };

    const handleLogout = () => {
        localStorage.clear();
        navigate('/login');
    };

    return (
        <Layout style={{ minHeight: '100vh', background: 'transparent' }}>
            <Sider
                width={260}
                theme="light"
                style={{
                    overflow: 'auto',
                    height: '100vh',
                    position: 'fixed',
                    left: 0,
                    top: 0,
                    bottom: 0,
                    borderRight: '1px solid #e2e8f0',
                    zIndex: 1000
                }}
            >
                <div className="p-8 flex items-center gap-3">
                    <div style={brandLogoStyle}>
                        <WalletOutlined style={{ color: '#e6dbdb', fontSize: '20px' }} />
                    </div>
                    <div className="flex flex-col">
                        <span style={{ fontSize: '22px', fontWeight: '900', color: '#ff4d4f', lineHeight: '1' }}>WalletApp</span>
                        <span className="text-[10px] font-bold text-slate-400 tracking-[0.2em] uppercase">{t('brand_subtitle')}</span>
                    </div>
                </div>
                <Menu
                    mode="inline"
                    selectedKeys={[getSelectedKey()]}
                    className="border-none px-4 custom-menu"
                    items={[
                        { key: '1', icon: <AppstoreOutlined />, label: t('menu_dashboard'), onClick: () => navigate('/dashboard') },
                        { key: '2', icon: <SwapOutlined />, label: t('menu_transaction'), onClick: () => navigate('/transaction') },
                        { key: '3', icon: <HistoryOutlined />, label: t('menu_history'), onClick: () => navigate('/history') },
                        { key: '4', icon: <LineChartOutlined />, label: t('menu_rates'), onClick: () => navigate('/investment') },
                        { type: 'divider' },
                        { key: 'logout', icon: <LogoutOutlined />, label: t('menu_logout'), danger: true, onClick: handleLogout }
                    ]}
                />

                <div className="px-6 mt-4">
                    <div
                        className="flex items-center justify-between p-3 rounded-2xl"
                        style={{
                            background: 'linear-gradient(135deg, #fff2f0 0%, #fff7f6 100%)',
                            border: '1,5px solid #ffccc7'
                        }}
                    >
                        <span style={{ fontSize: '10px', fontWeight: '900', color: '#ff4d4f', letterSpacing: '1px' }}>
                            {t('lang_label') || 'LANGUAGE'}
                        </span>
                        <LanguageSwitcher />
                    </div>
                </div>
            </Sider>
            <Layout style={{ marginLeft: 260, backgroundColor: 'transparent' }}>
                <Content>{children}</Content>
            </Layout>

            <style dangerouslySetInnerHTML={{
                __html: `
                .custom-menu .ant-menu-item-selected {
                    background-color: #fff2f0 !important;
                    color: #ff4d4f !important;
                    font-weight: 800;
                    border-radius: 14px !important;
                }

                .custom-menu .ant-menu-item:hover {
                    color: #ff4d4f !important;
                }

                .custom-menu .ant-menu-item-selected .anticon {
                    color: #ff4d4f !important;
                }

                .custom-menu.ant-menu-inline .ant-menu-item:after {
                    border-right: none !important;
                }
                
                .ant-menu-item-divider {
                    border-color: #f1f5f9 !important;
                }
            `}} />
        </Layout>
    );
};

export default MainLayout;
