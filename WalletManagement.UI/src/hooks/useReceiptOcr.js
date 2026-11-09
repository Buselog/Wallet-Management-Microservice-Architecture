import { useState, useCallback } from 'react';
import { message, notification } from 'antd';
import api from '../services/api';
import { previewInvoice, confirmInvoice } from '../services/document';
import { handleApiError } from '../utils/errorHandler';
import { useTranslation } from 'react-i18next';

const MAX_FILE_SIZE = 5 * 1024 * 1024; // 5 MB, matches backend
const ALLOWED_EXTENSIONS = ['.pdf', '.png', '.jpg', '.jpeg'];

/**
 * Manages the receipt/invoice OCR flow:
 * file selection -> manual scan (preview) -> review -> confirm payment.
 */
export const useReceiptOcr = () => {
  const { t } = useTranslation();

  const [file, setFile] = useState(null);
  const [preview, setPreview] = useState(null);
  const [wallets, setWallets] = useState([]);
  const [selectedWalletId, setSelectedWalletId] = useState(null);
  const [loading, setLoading] = useState(false);
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [error, setError] = useState(null);

  const loadWallets = useCallback(async () => {
    try {
      const response = await api.get('/v1/Dashboard/summary');
      const list = response.data?.wallets || response.data?.Wallets || [];
      setWallets(Array.isArray(list) ? list : []);
      if (Array.isArray(list) && list.length > 0 && selectedWalletId == null) {
        setSelectedWalletId(list[0].id ?? list[0].Id);
      }
    } catch (err) {
      handleApiError(err, null, (msg) => {
        notification.error({ message: t('trans_error_msg'), description: msg });
      });
    }
  }, [selectedWalletId, t]);

  const validateFile = useCallback((candidate) => {
    if (!candidate) return t('receipt_file_required');
    const extension = candidate.name.slice(candidate.name.lastIndexOf('.')).toLowerCase();
    if (!ALLOWED_EXTENSIONS.includes(extension)) {
      return t('receipt_invalid_extension');
    }
    if (candidate.size > MAX_FILE_SIZE) {
      return t('receipt_file_too_large');
    }
    return null;
  }, [t]);

  const selectFile = useCallback((candidate) => {
    const validationError = validateFile(candidate);
    if (validationError) {
      setError(validationError);
      setFile(null);
      setPreview(null);
      return false;
    }
    setError(null);
    setFile(candidate);
    setPreview(null);
    return true;
  }, [validateFile]);

  const clearFile = useCallback(() => {
    setFile(null);
    setPreview(null);
    setError(null);
  }, []);

  const scan = useCallback(async () => {
    if (!file) {
      setError(t('receipt_file_required'));
      return;
    }
    if (selectedWalletId == null) {
      setError(t('receipt_wallet_required'));
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const result = await previewInvoice(file, selectedWalletId);
      setPreview(result);
      if (result?.CanBePaid) {
        message.success(t('receipt_preview_success'));
      } else if (result?.ValidationMessage) {
        message.warning(result.ValidationMessage);
      }
    } catch (err) {
      setPreview(null);
      handleApiError(err, null, (msg) => {
        setError(msg || t('receipt_scan_failed'));
        notification.error({ message: t('receipt_scan_failed'), description: msg });
      });
    } finally {
      setLoading(false);
    }
  }, [file, selectedWalletId, t]);

  const confirm = useCallback(async () => {
    if (!preview?.DraftToken) return false;
    setConfirmLoading(true);
    try {
      const result = await confirmInvoice(preview.DraftToken);
      if (result) {
        message.success(t('receipt_payment_success'));
        clearFile();
        return true;
      }
      message.error(t('receipt_payment_failed'));
      return false;
    } catch (err) {
      handleApiError(err, null, (msg) => {
        notification.error({ message: t('receipt_payment_failed'), description: msg });
      });
      return false;
    } finally {
      setConfirmLoading(false);
    }
  }, [preview, clearFile, t]);

  const reset = useCallback(() => {
    clearFile();
  }, [clearFile]);

  return {
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
  };
};
