import { CheckCircle, XCircle, WarningCircle, Receipt } from '@phosphor-icons/react';
import { formatNumber } from '../../utils/formatters';
import { Button } from '../ui/button';
import { cn } from '../../lib/utils';

/**
 * Renders the extracted invoice data and payment eligibility returned by the OCR preview.
 */
export default function InvoicePreviewCard({ preview, onConfirm, onCancel, confirmLoading }) {
  if (!preview) return null;

  const checks = [
    {
      label: 'Already paid',
      ok: !preview.IsAlreadyPaid,
      failText: 'This invoice has already been paid.',
    },
    {
      label: 'Currency match',
      ok: preview.IsCurrencyMatched,
      failText: preview.IsCurrencyMatched
        ? ''
        : `Currency mismatch: invoice is ${preview.Currency}.`,
    },
    {
      label: 'Sufficient balance',
      ok: preview.HasSufficientBalance,
      failText: 'Insufficient balance in the selected wallet.',
    },
  ];

  const canPay = Boolean(preview.CanBePaid && preview.DraftToken);

  return (
    <div className="rounded-2xl border border-border bg-card p-6 shadow-sm">
      <div className="mb-4 flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-muted text-muted-foreground">
          <Receipt size={20} weight="regular" aria-hidden="true" />
        </div>
        <div className="min-w-0">
          <h3 className="truncate text-base font-bold text-foreground">
            {preview.BillerName || 'Unknown biller'}
          </h3>
          <p className="truncate text-xs text-muted-foreground">
            Invoice #{preview.InvoiceNumber || '—'}
          </p>
        </div>
        <span
          className={cn(
            'ml-auto rounded-full px-3 py-1 text-[10px] font-black uppercase tracking-widest',
            preview.CanBePaid
              ? 'bg-green-50 text-green-700'
              : 'bg-red-50 text-red-700',
          )}
        >
          {preview.CanBePaid ? 'Payable' : 'Not payable'}
        </span>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-xl bg-muted/40 p-4">
          <p className="text-[10px] font-black uppercase tracking-widest text-muted-foreground">
            Total amount
          </p>
          <p className="mt-1 text-2xl font-black text-foreground">
            {preview.TotalAmount != null ? formatNumber(preview.TotalAmount) : '—'}{' '}
            <span className="text-base font-semibold text-muted-foreground">{preview.Currency}</span>
          </p>
        </div>
        <div className="rounded-xl bg-muted/40 p-4">
          <p className="text-[10px] font-black uppercase tracking-widest text-muted-foreground">
            Issue date
          </p>
          <p className="mt-1 text-lg font-bold text-foreground">{preview.IssueDate || '—'}</p>
        </div>
        <div className="rounded-xl bg-muted/40 p-4">
          <p className="text-[10px] font-black uppercase tracking-widest text-muted-foreground">
            Status
          </p>
          <p className="mt-1 text-lg font-bold text-foreground">
            {preview.IsAlreadyPaid ? 'Already paid' : preview.CanBePaid ? 'Ready' : 'Blocked'}
          </p>
        </div>
      </div>

      <ul className="mt-4 space-y-2">
        {checks.map((check) => (
          <li key={check.label} className="flex items-center gap-2 text-sm">
            {check.ok ? (
              <CheckCircle size={16} weight="fill" className="text-green-600" aria-hidden="true" />
            ) : (
              <XCircle size={16} weight="fill" className="text-red-600" aria-hidden="true" />
            )}
            <span className={check.ok ? 'text-foreground' : 'text-destructive'}>
              {check.ok ? check.label : check.failText}
            </span>
          </li>
        ))}
      </ul>

      {preview.ValidationMessage && !preview.CanBePaid && (
        <div className="mt-4 flex items-start gap-2 rounded-xl border border-destructive/20 bg-destructive/5 p-3">
          <WarningCircle size={16} weight="bold" className="mt-0.5 text-destructive" aria-hidden="true" />
          <p className="text-sm text-destructive">{preview.ValidationMessage}</p>
        </div>
      )}

      <div className="mt-6 flex flex-col gap-2 sm:flex-row sm:justify-end">
        <Button variant="outline" size="md" onClick={onCancel} disabled={confirmLoading}>
          Scan another
        </Button>
        <Button
          variant="primary"
          size="md"
          onClick={onConfirm}
          disabled={!canPay || confirmLoading}
        >
          {confirmLoading && (
            <span
              className="inline-block h-4 w-4 animate-spin rounded-full border-2 border-primary-foreground border-t-transparent"
              aria-hidden="true"
            />
          )}
          Pay invoice
        </Button>
      </div>
    </div>
  );
}
