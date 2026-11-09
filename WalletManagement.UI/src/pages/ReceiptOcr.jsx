import { useEffect } from 'react';
import { Select } from 'antd';
import { useReceiptOcr } from '../hooks/useReceiptOcr';
import { useTranslation } from 'react-i18next';
import ReceiptDropzone from '../components/receipts/ReceiptDropzone';
import InvoicePreviewCard from '../components/receipts/InvoicePreviewCard';
import { Button } from '../components/ui/button';
import { CircleNotch } from '@phosphor-icons/react';

const ReceiptOcr = () => {
  const { t } = useTranslation();
  const {
    file,
    preview,
    wallets,
    selectedWalletId,
    setSelectedWalletId,
    loading,
    confirmLoading,
    error,
    selectFile,
    clearFile,
    scan,
    confirm,
    reset,
    loadWallets,
  } = useReceiptOcr();

  useEffect(() => {
    loadWallets();
  }, [loadWallets]);

  const handleConfirm = async () => {
    const ok = await confirm();
    if (ok) {
      // reset already handled inside confirm
    }
  };

  const handleCancel = () => {
    reset();
  };

  return (
    <div className="mx-auto w-full max-w-4xl p-6 md:p-10">
      <div className="mb-8">
        <h2 className="text-2xl font-black text-foreground">{t('receipt_title')}</h2>
        <p className="mt-1 text-sm text-muted-foreground">{t('receipt_subtitle')}</p>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-5">
        {/* Left: upload + wallet */}
        <div className="lg:col-span-3 flex flex-col gap-6">
          <div className="rounded-2xl border border-border bg-card p-6 shadow-sm">
            <label className="mb-2 block text-[11px] font-black uppercase tracking-widest text-muted-foreground">
              {t('receipt_wallet_label')}
            </label>
            <Select
              size="large"
              className="w-full"
              placeholder={t('receipt_wallet_placeholder')}
              value={selectedWalletId ?? undefined}
              onChange={(value) => setSelectedWalletId(value)}
              options={wallets.map((w) => ({
                value: w.id ?? w.Id,
                label: `${w.currency} - ${t(`wallet_type_${w.type ?? w.Type}`)} (#${w.id ?? w.Id})`,
              }))}
            />

            <div className="mt-6">
              <ReceiptDropzone
                file={file}
                onSelectFile={selectFile}
                onClear={clearFile}
                error={error}
                loading={loading}
              />
            </div>

            <div className="mt-6 flex flex-col gap-2 sm:flex-row sm:justify-end">
              <Button
                variant="primary"
                size="md"
                onClick={scan}
                disabled={!file || loading}
              >
                {loading && (
                  <CircleNotch size={16} className="animate-spin" aria-hidden="true" />
                )}
                {t('receipt_scan_btn')}
              </Button>
            </div>
          </div>
        </div>

        {/* Right: preview */}
        <div className="lg:col-span-2">
          {preview ? (
            <InvoicePreviewCard
              preview={preview}
              onConfirm={handleConfirm}
              onCancel={handleCancel}
              confirmLoading={confirmLoading}
            />
          ) : (
            <div className="flex h-full min-h-[220px] items-center justify-center rounded-2xl border border-dashed border-border bg-muted/20 p-6 text-center">
              <div>
                <p className="text-sm font-semibold text-muted-foreground">
                  {t('receipt_preview_empty')}
                </p>
                <p className="mt-1 text-xs text-muted-foreground/80">
                  {t('receipt_preview_empty_hint')}
                </p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ReceiptOcr;
