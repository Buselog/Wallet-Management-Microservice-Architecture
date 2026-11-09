import { Button, Dropdown, Space } from 'antd';
import { Globe } from '@phosphor-icons/react';
import { useTranslation } from 'react-i18next';

const LanguageSwitcher = () => {
    const { i18n } = useTranslation();

    const languages = [
        { key: 'tr', label: 'Türkçe', icon: '🇹🇷' },
        { key: 'en', label: 'English', icon: '🇺🇸' }
    ];

    const handleMenuClick = ({ key }) => {
        i18n.changeLanguage(key);
        localStorage.setItem('i18nextLng', key);
    };

    const menuItems = languages.map(lang => ({
        key: lang.key,
        label: (
            <Space>
                <span>{lang.icon}</span>
                {lang.label}
            </Space>
        ),
        onClick: handleMenuClick
    }));

    return (
        <Dropdown menu={{ items: menuItems }} placement="bottomRight" trigger={['click']}>
            <Button
                type="text"
                className="flex items-center gap-2 font-bold text-slate-600 hover:text-volcano-500"
                icon={<Globe size={18} weight="regular" aria-hidden="true" />}
            >
                {i18n.language?.toUpperCase().substring(0, 2)}
            </Button>
        </Dropdown>
    );
};

export default LanguageSwitcher;