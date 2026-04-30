import React from 'react';
import { Layout, Typography, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { LogoutOutlined } from '@ant-design/icons';

const { Header, Content } = Layout;
const { Title } = Typography;

const Dashboard = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <Layout className="min-h-screen bg-slate-50">
            <Header className="bg-white px-8 flex items-center justify-between border-b border-slate-200">
                <Title level={4} className="!mb-0 !text-slate-800">Cüzdan Yönetimi</Title>
                <Button
                    type="text"
                    icon={<LogoutOutlined />}
                    onClick={handleLogout}
                    className="text-slate-500 hover:text-volcano-600"
                >
                    Çıkış Yap
                </Button>
            </Header>

            <Content className="p-8">
                <div className="bg-white p-12 rounded-2xl border border-dashed border-slate-300 flex flex-col items-center justify-center">
                    <Title level={3} className="!text-slate-400">Dashboard</Title>
                    <p className="text-slate-400">Dahboard Alanı</p>
                </div>
            </Content>
        </Layout>
    );
};

export default Dashboard;