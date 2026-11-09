import { useLocation, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { cn } from '../lib/utils';
import { Wallet } from '@phosphor-icons/react';
import { NAV_ITEMS, LOGOUT_ITEM } from '../config/navigation';

const SIDEBAR_WIDTH = 'w-64';
const SIDEBAR_COLLAPSED_WIDTH = 'w-16';

/**
 * Application sidebar. Renders brand, primary navigation and logout.
 * @param {Object} props
 * @param {boolean} props.collapsed - Whether the sidebar is in icon-only mode.
 * @param {() => void} [props.onNavigate] - Called after a navigation item is clicked (e.g. close mobile drawer).
 */
export default function Sidebar({ collapsed = false, onNavigate }) {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();

  const handleNavigate = (path) => {
    navigate(path);
    onNavigate?.();
  };

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
    onNavigate?.();
  };

  return (
    <div
      className={cn(
        'flex h-full flex-col bg-sidebar text-sidebar-foreground',
        collapsed ? SIDEBAR_COLLAPSED_WIDTH : SIDEBAR_WIDTH,
      )}
    >
      {/* Brand */}
      <div className={cn('flex items-center gap-3 px-5 py-6', collapsed && 'justify-center px-2')}>
        <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-primary text-primary-foreground shadow-sm">
          <Wallet size={20} weight="fill" aria-hidden="true" />
        </div>
        {!collapsed && (
          <div className="flex flex-col leading-tight">
            <span className="text-lg font-black tracking-tight text-primary">WalletApp</span>
            <span className="text-[10px] font-bold uppercase tracking-[0.2em] text-muted-foreground">
              {t('brand_subtitle')}
            </span>
          </div>
        )}
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-3" aria-label="Primary">
        <ul className="flex flex-col gap-1">
          {NAV_ITEMS.map((item) => {
            const Icon = item.icon;
            const isActive = location.pathname === item.path;
            return (
              <li key={item.key}>
                <button
                  type="button"
                  onClick={() => handleNavigate(item.path)}
                  aria-current={isActive ? 'page' : undefined}
                  title={collapsed ? t(item.labelKey) : undefined}
                  className={cn(
                    'flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-semibold transition-colors',
                    'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-sidebar-ring',
                    isActive
                      ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                      : 'text-sidebar-foreground/80 hover:bg-sidebar-accent/60 hover:text-sidebar-foreground',
                    collapsed && 'justify-center px-2',
                  )}
                >
                  <Icon size={20} weight={isActive ? 'fill' : 'regular'} className="shrink-0" aria-hidden="true" />
                  {!collapsed && <span className="truncate">{t(item.labelKey)}</span>}
                </button>
              </li>
            );
          })}
        </ul>
      </nav>

      {/* Footer: language + logout */}
      <div className="border-t border-sidebar-border px-3 py-4">
        <button
          type="button"
          onClick={handleLogout}
          className={cn(
            'flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-semibold text-destructive transition-colors',
            'hover:bg-destructive/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-destructive',
            collapsed && 'justify-center px-2',
          )}
        >
          <LOGOUT_ITEM.icon size={20} weight="regular" className="shrink-0" aria-hidden="true" />
          {!collapsed && <span className="truncate">{t(LOGOUT_ITEM.labelKey)}</span>}
        </button>
      </div>
    </div>
  );
}
