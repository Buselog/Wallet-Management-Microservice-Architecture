import {
  SquaresFour,
  ArrowsLeftRight,
  ClockCounterClockwise,
  ChartLine,
  Receipt,
  Question,
  SignOut,
} from '@phosphor-icons/react';

/**
 * Single source of truth for the dashboard navigation.
 * `labelKey` is an i18n key; `icon` is a Phosphor component.
 */
export const NAV_ITEMS = [
  { key: 'dashboard', path: '/dashboard', labelKey: 'menu_dashboard', icon: SquaresFour },
  { key: 'transaction', path: '/transaction', labelKey: 'menu_transaction', icon: ArrowsLeftRight },
  { key: 'history', path: '/history', labelKey: 'menu_history', icon: ClockCounterClockwise },
  { key: 'investment', path: '/investment', labelKey: 'menu_rates', icon: ChartLine },
  { key: 'receipt-ocr', path: '/receipt-ocr', labelKey: 'menu_receipt_ocr', icon: Receipt },
  { key: 'help', path: '/help', labelKey: 'menu_help', icon: Question },
];

export const LOGOUT_ITEM = { key: 'logout', labelKey: 'menu_logout', icon: SignOut };
