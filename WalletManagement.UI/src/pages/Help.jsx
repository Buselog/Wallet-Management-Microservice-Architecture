import { Card, Divider, Typography } from 'antd';
import {
  BookOpen, User, SquaresFour, ArrowsLeftRight,
  ClockCounterClockwise, ChartLine, Globe,
} from '@phosphor-icons/react';
import { useTranslation } from 'react-i18next';

const { Paragraph, Text } = Typography;

const Help = () => {
  const { t } = useTranslation();

  const sections = [
    { icon: <User size={18} weight="regular" className="text-primary" />, title: t('help_auth_title'), desc: t('help_auth_desc') },
    { icon: <SquaresFour size={18} weight="regular" className="text-primary" />, title: t('help_dash_title'), desc: t('help_dash_desc') },
    { icon: <ArrowsLeftRight size={18} weight="regular" className="text-primary" />, title: t('help_trans_title'), desc: t('help_trans_desc') },
    { icon: <ClockCounterClockwise size={18} weight="regular" className="text-primary" />, title: t('help_history_title'), desc: t('help_history_desc') },
    { icon: <ChartLine size={18} weight="regular" className="text-primary" />, title: t('help_rates_title'), desc: t('help_rates_desc') },
    { icon: <Globe size={18} weight="regular" className="text-primary" />, title: t('help_lang_title'), desc: t('help_lang_title_desc') },
  ];

  return (
    <div className="p-8" style={{ maxWidth: '1000px', margin: '0 auto' }}>
      <Card
        className="rounded-3xl border-none shadow-sm p-4"
        title={
          <div className="flex items-center gap-3 py-2">
            <BookOpen size={26} weight="regular" className="text-primary" aria-hidden="true" />
            <span style={{ fontSize: '22px', fontWeight: 900, letterSpacing: '-0.5px' }}>
              {t('help_title')}
            </span>
          </div>
        }
      >
        <Paragraph className="text-slate-500 text-base" style={{ marginBottom: '24px' }}>
          {t('help_subtitle')}
        </Paragraph>

        {sections.map((section, index) => (
          <div key={index} style={{ marginBottom: '24px' }}>
            <div className="flex items-center gap-3 mb-2">
              <div
                className="flex items-center justify-center rounded-lg"
                style={{ width: '32px', height: '32px', backgroundColor: '#fff2f0', borderRadius: '8px' }}
              >
                {section.icon}
              </div>
              <Text style={{ fontSize: '16px', fontWeight: 700, color: '#1e293b' }}>
                {section.title}
              </Text>
            </div>
            <Paragraph
              className="text-slate-500 style-paragraph"
              style={{ paddingLeft: '44px', color: '#64748b', fontSize: '14px', lineHeight: '1.6' }}
            >
              {section.desc}
            </Paragraph>
            {index !== sections.length - 1 && <Divider style={{ margin: '16px 0', borderColor: '#f1f5f9' }} />}
          </div>
        ))}

        <Divider style={{ borderColor: '#f1f5f9' }} />
        <div
          className="p-4 rounded-2xl text-center"
          style={{ background: 'linear-gradient(135deg, #fff2f0 0%, #fff7f6 100%)', border: '1px dashed #ffccc7' }}
        >
          <Text type="secondary" style={{ fontSize: '12px', color: '#ff4d4f', fontWeight: 600 }}>
            {t('trans_ssl_text')}
          </Text>
        </div>
      </Card>
    </div>
  );
};

export default Help;
