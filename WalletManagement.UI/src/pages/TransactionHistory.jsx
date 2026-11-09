import { Table, Card, DatePicker, Tag, Radio, Empty } from 'antd';
import { Wallet, ArrowUp, ArrowDown } from '@phosphor-icons/react';
import dayjs from 'dayjs';
import { useTransactionHistory } from '../hooks/useTransactionHistory';
import { useTranslation } from 'react-i18next';
import { getCurrencySymbol, formatNumber } from '../utils/formatters';

const TransactionHistory = () => {
  const { t, i18n } = useTranslation();
  const {
    wallets,
    selectedWalletId,
    transactions,
    totalCount,
    loading,
    page,
    setPage,
    pageSize,
    setStartDate,
    setEndDate,
    handleWalletChange,
  } = useTransactionHistory();

  const activeWallet = wallets.find((w) => w.id === selectedWalletId);
  const currencySymbol = activeWallet ? getCurrencySymbol(activeWallet.currency) : '₺';

  const columns = [
    {
      title: t('hist_col_date'),
      dataIndex: 'transactionDate',
      render: (text, record) => (
        <span className="font-bold text-slate-500">
          {dayjs(record.transactionDate || record.createdDate).format(
            i18n.language === 'tr' ? 'DD.MM.YYYY HH:mm:ss' : 'MM/DD/YYYY HH:mm:ss',
          )}
        </span>
      ),
    },
    {
      title: t('hist_col_type'),
      dataIndex: 'transactionType',
      render: (type, record) => {
        const isPositive = (record.amount || 0) > 0;
        return (
          <Tag
            icon={isPositive ? <ArrowUp size={12} weight="bold" /> : <ArrowDown size={12} weight="bold" />}
            color={isPositive ? 'success' : 'error'}
            style={{ borderRadius: '8px', fontWeight: 900, border: 'none', padding: '2px 10px' }}
          >
            {type?.toUpperCase()}
          </Tag>
        );
      },
    },
    {
      title: t('hist_col_amount'),
      dataIndex: 'amount',
      render: (amount) => {
        const isPositive = amount > 0;
        return (
          <span style={{ fontWeight: 900, fontSize: '17px', color: isPositive ? '#52c41a' : '#ff4d4f' }}>
            {isPositive ? '+' : '-'} {currencySymbol}
            {formatNumber(Math.abs(amount))}
          </span>
        );
      },
    },
    {
      title: t('hist_col_ref'),
      dataIndex: 'referenceId',
      render: (ref) => <span className="text-slate-300 font-mono text-[11px]">{ref || '-'}</span>,
    },
  ];

  return (
    <>
      <div className="flex flex-col gap-6 my-6 mx-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p className="text-xs text-muted-foreground">{t('hist_subtitle')}</p>
          </div>

          <div className="flex gap-3 bg-white p-3 rounded-2xl border border-slate-100 shadow-sm">
            <div className="flex flex-col">
              <span style={{ fontSize: '9px', fontWeight: 900, color: '#ff4d4f', marginBottom: '4px' }}>
                {t('hist_start_date')}
              </span>
              <DatePicker placeholder={t('hist_date_placeholder')} onChange={setStartDate} className="h-8 w-32 rounded-lg" />
            </div>
            <div className="flex flex-col">
              <span style={{ fontSize: '9px', fontWeight: 900, color: '#ff4d4f', marginBottom: '4px' }}>
                {t('hist_end_date')}
              </span>
              <DatePicker placeholder={t('hist_date_placeholder')} onChange={setEndDate} className="h-8 w-32 rounded-lg" />
            </div>
          </div>
        </div>

        <div>
          <span style={{ color: '#ff4d4f', fontWeight: 900, fontSize: '11px', letterSpacing: '2px', display: 'block', marginBottom: '12px' }}>
            {t('hist_wallet_selection')}
          </span>
          <Radio.Group value={selectedWalletId} onChange={(e) => handleWalletChange(e.target.value)}>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
              {wallets.map((w) => (
                <Radio.Button key={w.id} value={w.id} className="wallet-radio-btn">
                  <Wallet size={14} weight="regular" className="mr-2" aria-hidden="true" />
                  {w.currency} # {w.id}
                </Radio.Button>
              ))}
            </div>
          </Radio.Group>
        </div>
      </div>

      <div className="mb-6 mx-6">
        <Card className="rounded-[24px] border-none shadow-sm bg-white overflow-hidden flex-initial">
          <Table
            columns={columns}
            dataSource={transactions}
            loading={loading}
            rowKey="id"
            size="middle"
            pagination={{
              current: page,
              total: totalCount,
              pageSize: pageSize,
              onChange: (p) => setPage(p),
              position: ['bottomCenter'],
              className: 'custom-pagination',
              showTotal: (total) => (
                <span style={{ position: 'absolute', left: 20, color: '#94a3b8', fontSize: '11px', fontWeight: 800 }}>
                  {t('hist_total_records', { total })}
                </span>
              ),
            }}
            locale={{ emptyText: <Empty description={t('hist_empty')} /> }}
          />
        </Card>
      </div>

      <style dangerouslySetInnerHTML={{
        __html: `
        .wallet-radio-btn { height: 44px !important; line-height: 44px !important; border-radius: 12px !important; border: 1px solid #e2e8f0 !important; font-weight: 700 !important; }
        .ant-radio-button-wrapper-checked.wallet-radio-btn { background: #ff4d4f !important; border-color: #ff4d4f !important; color: white !important; }
        .wallet-radio-btn:before { display: none !important; }
        .ant-table-thead > tr > th { background: #fdfcfc !important; color: #94a3b8 !important; font-weight: 900 !important; font-size: 11px !important; }
        .ant-table-tbody > tr:hover > td { background: #fff2f0 !important; }
        .custom-pagination .ant-pagination-item-active { background: #ff4d4f; border-color: #ff4d4f; }
        .custom-pagination .ant-pagination-item-active a { color: white !important; }
      `}} />
    </>
  );
};

export default TransactionHistory;
