import { useState } from 'react';
import { Card, Row, Col, Statistic, Tag, Form, Spin, Popconfirm } from 'antd';
import { Plus, Wallet, User, Trash, ArrowRight } from '@phosphor-icons/react';
import { useDashboard } from '../hooks/useDashboard';
import CreateWalletModal from '../components/CreateWalletModal';
import { useTranslation } from 'react-i18next';
import { getCurrencySymbol, formatNumber } from '../utils/formatters';
import { Button } from '../components/ui/button';

const customStyles = {
  volcanoButton: {
    backgroundColor: 'var(--primary)',
    borderColor: 'var(--primary)',
    borderRadius: '16px',
    fontWeight: 700,
    height: '40px',
    boxShadow: '0 8px 20px rgba(15, 23, 42, 0.2)',
    textTransform: 'uppercase',
    letterSpacing: '1px',
  },
  brandLogo: {
    background: 'linear-gradient(135deg, #0F172A 0%, #1E3A8A 100%)',
    boxShadow: '0 8px 16px rgba(15, 23, 42, 0.25)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: '14px',
    transition: 'all 0.3s ease',
  },
};

const Dashboard = () => {
  const { t } = useTranslation();
  const [form] = Form.useForm();
  const [isModalVisible, setIsModalVisible] = useState(false);
  const { data, loading, rates, createWallet, deleteWallet } = useDashboard(form);

  if (loading) {
    return (
      <div className="flex h-screen w-full items-center justify-center bg-background">
        <Spin size="large" />
      </div>
    );
  }

  return (
    <>
      <div className="mx-auto w-full max-w-[1600px] p-6 md:p-10">
        {/* Page greeting */}
        <div className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 className="text-xl font-bold text-foreground md:text-2xl">
              {t('dash_welcome', { name: data?.fullName })}
            </h2>
            <p className="mt-1 text-sm text-muted-foreground">{t('dash_subtitle')}</p>
          </div>
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-muted text-muted-foreground">
              <User size={20} weight="regular" aria-hidden="true" />
            </div>
            <Button onClick={() => setIsModalVisible(true)}>
              <Plus size={16} weight="bold" aria-hidden="true" />
              {t('dash_new_wallet')}
            </Button>
          </div>
        </div>

        {/* Asset distribution */}
        <section className="mb-12">
          <p className="mb-4 text-[10px] font-bold uppercase tracking-[0.2em] text-muted-foreground">
            {t('dash_asset_dist')}
          </p>
          <Row gutter={[24, 24]} justify="start">
            {data?.currencySummaries?.map((summary) => (
              <Col
                span={24 / (data?.currencySummaries?.length || 1)}
                key={summary.currency}
                xs={24}
                sm={12}
                lg={24 / (data?.currencySummaries?.length || 1)}
              >
                <Card
                  bordered={false}
                  className="shadow-sm"
                  styles={{ body: { padding: '20px' } }}
                  style={{
                    borderRadius: '20px',
                    borderLeft: '5px solid var(--primary)',
                    boxShadow: '0 4px 12px rgba(0,0,0,0.03)',
                  }}
                >
                  <Statistic
                    title={
                      <span className="text-[10px] font-black uppercase tracking-widest text-muted-foreground">
                        {t('dash_total_currency', { currency: summary.currency })}
                      </span>
                    }
                    value={summary.totalBalance}
                    precision={2}
                    prefix={
                      <span className="mr-1 text-primary">
                        {getCurrencySymbol(summary.currency)}
                      </span>
                    }
                    valueStyle={{ fontWeight: 900, color: '#020617', fontSize: '28px' }}
                  />
                </Card>
              </Col>
            ))}
          </Row>
        </section>

        {/* Wallets */}
        <section>
          <p className="mb-6 block text-[10px] font-bold uppercase tracking-[0.2em] text-muted-foreground">
            {t('dash_my_wallets')}
          </p>
          <Row gutter={[24, 24]}>
            {data?.wallets?.map((wallet) => (
              <Col xs={24} md={12} lg={8} key={wallet.id}>
                <Card
                  className="wallet-card-premium transition-all duration-300"
                  styles={{ body: { padding: '28px' } }}
                  actions={[
                    <Popconfirm
                      key="delete"
                      title={t('dash_delete_confirm_title')}
                      onConfirm={() => deleteWallet(wallet.id)}
                      okText={t('dash_delete_yes')}
                      cancelText={t('dash_delete_no')}
                    >
                      <Trash size={18} className="text-destructive" aria-hidden="true" />
                    </Popconfirm>,
                    <ArrowRight size={18} className="text-primary" key="go" aria-hidden="true" />,
                  ]}
                >
                  <div className="mb-7 flex items-center justify-between">
                    <div className="flex items-center justify-center rounded-2xl bg-muted p-4 shadow-sm">
                      <Wallet size={24} weight="regular" className="text-primary" aria-hidden="true" />
                    </div>
                    <div className="text-right">
                      <Tag
                        color="default"
                        style={{
                          borderRadius: '10px',
                          background: 'linear-gradient(90deg, #E8ECF1 0%, #F1F5F9 100%)',
                          color: 'var(--primary)',
                          border: 'none',
                          fontWeight: 900,
                          padding: '4px 14px',
                          margin: 0,
                          display: 'inline-flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                        }}
                      >
                        {wallet.currency}
                      </Tag>
                      <div className="mt-1.5 font-mono text-xs font-bold text-muted-foreground">
                        ID: #{wallet.id}
                      </div>
                    </div>
                  </div>

                  <div className="flex flex-col gap-1">
                    <span className="text-[11px] font-extrabold uppercase tracking-widest text-muted-foreground">
                      {t('dash_available_balance')}
                    </span>
                    <div className="flex items-baseline gap-1.5">
                      <span className="text-xl font-light text-primary">
                        {getCurrencySymbol(wallet.currency)}
                      </span>
                      <span className="text-3xl font-black tracking-tight text-foreground">
                        {formatNumber(wallet.balance)}
                      </span>
                    </div>
                  </div>
                </Card>
              </Col>
            ))}
          </Row>
        </section>
      </div>

      <CreateWalletModal
        visible={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        onFinish={(values) => createWallet(values, () => setIsModalVisible(false))}
        rates={rates}
        customStyles={customStyles}
      />

      <style dangerouslySetInnerHTML={{
        __html: `
        .wallet-card-premium {
          border: 1px solid var(--border) !important;
          background: linear-gradient(145deg, #ffffff 0%, #fdfcfc 100%) !important;
        }
        .wallet-card-premium:hover {
          border-color: var(--primary) !important;
          box-shadow: 0 20px 40px rgba(0,0,0,0.05) !important;
          transform: translateY(-5px);
        }
        .ant-modal-content { overflow: visible !important; border-radius: 32px !important; }
        .ant-select-dropdown { z-index: 10001 !important; border-radius: 16px !important; overflow: hidden !important; }
      `}} />
    </>
  );
};

export default Dashboard;
