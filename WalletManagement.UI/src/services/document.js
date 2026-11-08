import api from './api';

/**
 * Uploads a receipt/invoice file for OCR preview.
 * @param {File} file
 * @param {number} walletId
 * @returns {Promise<any>} InvoicePreviewResponseDto
 */
export const previewInvoice = async (file, walletId) => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('walletId', String(walletId));

  const response = await api.post('/Document/preview', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return response.data;
};

/**
 * Confirms a previously previewed invoice payment.
 * @param {string} draftToken
 * @returns {Promise<boolean>}
 */
export const confirmInvoice = async (draftToken) => {
  const response = await api.post('/Document/confirm', { DraftToken: draftToken });
  return response.data;
};
