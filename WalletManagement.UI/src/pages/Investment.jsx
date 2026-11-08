import { Table, Card, Space, Tag, Form } from 'antd';
import { User, ArrowsClockwise, ShieldCheck } from '@phosphor-icons/react';
import { useInvestment } from '../hooks/useInvestment';
import TradeModal from '../components/TradeModal';
import { getCurrencyName } from '../utils/formatters';
import { useTranslation } from 'react-i18next';
import { Button } from '../components/ui/button';

const Investment = () => {
  const { t } = useTranslation();
  const [form] = Form.useForm();
  const {
    rates, wallets, loading, modalVisible, setModalVisible,
    selectedRate, tradeType, userData, lastUpdate,
    fetchData, openTrade, executeTrade,
  } = useInvestment(form);

  const columns = [
    {
      title: t('inv_col_code'),
      dataIndex: 'currencyCode',
      render: (code) => (
        <Tag color="volcano" className="font-black px-3 py-1 rounded-lg border-none">
          {code}
        </Tag>
      ),
    },
    {
      title: t('inv_col_name'),
      dataIndex: 'currencyName',
      render: (_, record) => <span className="font-bold text-slate-600">{getCurrencyName(record.currencyCode)}</span>,
    },
    {
      title: t('inv_col_buy'),
      dataIndex: 'buyingRate',
      render: (val) => (
        <span className="rate-font text-green-600">
          ₺{val.toLocaleString(t('lang_code') === 'en-US' ? 'en-US' : 'tr-TR', { minimumFractionDigits: 4 })}
        </span>
      ),
    },
    {
      title: t('inv_col_sell'),
      dataIndex: 'sellingRate',
      render: (val) => (
        <span className="rate-font text-red-500">
          ₺{val.toLocaleString(t('lang_code') === 'en-US' ? 'en-US' : 'tr-TR', { minimumFractionDigits: 4 })}
        </span>
      ),
    },
    {
      title: t('inv_col_actions'),
      render: (_, record) => (
        <Space>
          <Button
            variant="ghost"
            size="sm"
            className="btn-buy-custom"
            onClick={() => openTrade(record, 'BUY')}
          >
            {t('inv_buy_btn')}
          </Button>
          <Button
            variant="ghost"
            size="sm"
            className="btn-sell-custom"
            onClick={() => openTrade(record, 'SELL')}
          >
            {t('inv_sell_btn')}
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <>
      <div className="my-6 mx-8 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h2 className="text-2xl font-black text-slate-800">{t('inv_title')}</h2>
          <p className="text-xs text-muted-foreground">{t('inv_subtitle')}</p>
        </div>
        <div className="text-right">
          <Button variant="outline" size="md" onClick={fetchData} disabled={loading}>
            <ArrowsClockwise size={16} weight="bold" className={loading ? 'animate-spin' : ''} aria-hidden="true" />
            {t('inv_refresh_btn')}
          </Button>
          <div className="mt-1">
            <span className="text-[10px] text-slate-400 font-bold uppercase tracking-widest">
              {t('inv_last_update', { date: lastUpdate })}
            </span>
          </div>
        </div>
      </div>

      <div className="px-8 flex-1 flex flex-col min-h-0">
        <Card
          className="rounded-[32px] border-none shadow-sm flex-1 overflow-hidden"
          styles={{ body: { height: '100%', padding: 0 } }}
        >
          <Table
            columns={columns}
            dataSource={rates}
            pagination={false}
            loading={loading}
            rowKey="currencyCode"
            className="custom-investment-table"
            scroll={{ y: 'calc(100vh - 280px)' }}
          />
        </Card>

        <div className="mt-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between bg-white p-4 rounded-2xl border border-slate-100 shadow-sm">
          <Space className="text-slate-500 font-bold text-xs">
            <div className="flex h-6 w-6 items-center justify-center rounded-full bg-muted text-muted-foreground">
              <User size={14} weight="regular" aria-hidden="true" />
            </div>
            {userData?.fullName} | {t('inv_manager_title')}
          </Space>
          <Space className="text-slate-400 text-[10px] font-bold uppercase tracking-widest">
            <ShieldCheck size={14} weight="bold" className="text-green-500" aria-hidden="true" />{' '}
            {t('inv_security_text')}
          </Space>
        </div>
      </div>

      <TradeModal
        visible={modalVisible}
        onCancel={() => setModalVisible(false)}
        tradeType={tradeType}
        selectedRate={selectedRate}
        wallets={wallets}
        onFinish={executeTrade}
        loading={loading}
        form={form}
      />

      <style dangerouslySetInnerHTML={{
        __html: `
        .rate-font { font-family: 'JetBrains Mono', monospace; font-weight: 800; font-size: 16px; }
        .custom-investment-table .ant-table-thead > tr > th { background: #fdfcfc !important; color: #94a3b8 !important; font-weight: 900 !important; font-size: 11px !important; padding: 20px !important; border-bottom: 1px solid #f1f5f9 !important; }
        .custom-investment-table .ant-table-tbody > tr > td { padding: 20px !important; border-bottom: 1px solid #f8fafc !important; }
        .btn-buy-custom { background: #f6ffed; color: #52c41a; border: 1px solid #b7eb8f; font-weight: 800; border-radius: 10px; width: 64px; height: 36px; }
        .btn-buy-custom:hover { background: #52c41a !important; color: white !important; }
        .btn-sell-custom { background: #fff1f0; color: #ff4d4f; border: 1px solid #ffccc7; font-weight: 800; border-radius: 10px; width: 64px; height: 36px; }
        .btn-sell-custom:hover { background: #ff4d4f !important; color: white !important; }
        .trade-modal .ant-modal-content { border-radius: 32px !important; padding: 30px !important; }
        .trade-icon-circle { width: 64px; height: 64px; border-radius: 20px; display: flex; align-items: center; justify-content: center; margin: 0 auto; }
        .trade-icon-circle.buy { background: #f6ffed; color: #52c41a; }
        .trade-icon-circle.sell { background: #fff1f0; color: #ff4d4f; }
        .btn-execute-buy { background: #52c41a !important; box-shadow: 0 8px 20px rgba(82, 196, 26, 0.2) !important; }
        .btn-execute-sell { background: #ff4d4f !important; box-shadow: 0 8px 20px rgba(255, 77, 79, 0.2) !important; }
        .ant-modal-mask { backdrop-filter: blur(4px); }
        .btn-refresh-custom { background-color: #fff2f0 !important; color: #ff4d4f !important; border-color: #ffccc7 !important; font-weight: 700 !important; border-radius: 12px !important; transition: all 0.3s ease !important; }
        .btn-refresh-custom:hover { background-color: #ff4d4f !important; color: white !important; border-color: #ff4d4f !important; box-shadow: 0 4px 12px rgba(255, 77, 79, 0.2) !important; }
        .custom-investment-table .ant-table-measure-row { visibility: collapse !important; line-height: 0 !important; height: 0 !important; }
        .custom-investment-table .ant-table-measure-row td { padding: 0 !important; border: 0 !important; height: 0 !important; }
      `}} />
    </>
  );
};

export default Investment;
