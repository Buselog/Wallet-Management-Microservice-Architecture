import { useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { List, Moon, Sun, Bell, UserCircle } from '@phosphor-icons/react';
import { useState, useEffect } from 'react';
import { cn } from '../lib/utils';
import { NAV_ITEMS } from '../config/navigation';
import LanguageSwitcher from './LanguageSwitcher';
import { Button } from './ui/button';

const HEADER_HEIGHT = 'h-16';

/**
 * Top application bar: mobile menu toggle, page title, actions.
 * @param {Object} props
 * @param {() => void} props.onToggleSidebar - Opens the mobile drawer.
 * @param {() => void} [props.onToggleCollapse] - Toggles the desktop sidebar.
 */
export default function Header({ onToggleSidebar, onToggleCollapse }) {
  const { t } = useTranslation();
  const location = useLocation();
  const [isDark, setIsDark] = useState(
    typeof document !== 'undefined' && document.documentElement.classList.contains('dark'),
  );

  useEffect(() => {
    document.documentElement.classList.toggle('dark', isDark);
    try {
      localStorage.setItem('theme', isDark ? 'dark' : 'light');
    } catch (e) {
      // ignore storage errors
    }
  }, [isDark]);

  const current = NAV_ITEMS.find((item) => item.path === location.pathname);
  const titleKey = current?.labelKey ?? 'menu_dashboard';
  const pageTitle = t(titleKey);

  return (
    <header
      className={cn(
        'sticky top-0 z-30 flex items-center justify-between gap-4 border-b border-border bg-background/80 px-4 backdrop-blur',
        HEADER_HEIGHT,
      )}
    >
      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          className="lg:hidden"
          onClick={onToggleSidebar}
          aria-label={t('menu_toggle') || 'Toggle menu'}
        >
          <List size={20} weight="bold" aria-hidden="true" />
        </Button>
        {onToggleCollapse && (
          <Button
            variant="ghost"
            size="icon"
            className="hidden lg:inline-flex"
            onClick={onToggleCollapse}
            aria-label={t('menu_collapse') || 'Collapse sidebar'}
          >
            <List size={20} weight="bold" aria-hidden="true" />
          </Button>
        )}
        <div className="flex flex-col leading-tight">
          <h1 className="text-base font-bold text-foreground md:text-lg">{pageTitle}</h1>
          <span className="hidden text-xs text-muted-foreground md:inline">
            {t('brand_subtitle')}
          </span>
        </div>
      </div>

      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" aria-label="Notifications">
          <Bell size={20} weight="regular" aria-hidden="true" />
        </Button>
        <Button
          variant="ghost"
          size="icon"
          aria-label={isDark ? 'Switch to light theme' : 'Switch to dark theme'}
          onClick={() => setIsDark((value) => !value)}
        >
          {isDark ? (
            <Sun size={20} weight="regular" aria-hidden="true" />
          ) : (
            <Moon size={20} weight="regular" aria-hidden="true" />
          )}
        </Button>
        <LanguageSwitcher />
        <div className="ml-1 flex h-9 w-9 items-center justify-center rounded-full bg-muted text-muted-foreground">
          <UserCircle size={22} weight="regular" aria-hidden="true" />
        </div>
      </div>
    </header>
  );
}
