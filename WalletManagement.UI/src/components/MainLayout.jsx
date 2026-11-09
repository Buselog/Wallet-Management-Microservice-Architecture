import { useState } from 'react';
import Sidebar from './Sidebar';
import Header from './Header';
import { Drawer } from './ui/drawer';
import { cn } from '../lib/utils';

/**
 * Application shell: responsive sidebar + sticky header + scrollable content.
 * Desktop: collapsible sidebar on the left.
 * Mobile: sidebar rendered inside a drawer.
 */
export default function MainLayout({ children }) {
  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <div className="flex h-screen overflow-hidden bg-background text-foreground">
      {/* Desktop sidebar */}
      <aside
        className={cn(
          'hidden shrink-0 border-r border-sidebar-border bg-sidebar transition-[width] duration-200 lg:block',
          collapsed ? 'w-16' : 'w-64',
        )}
      >
        <Sidebar collapsed={collapsed} />
      </aside>

      {/* Mobile sidebar drawer */}
      <Drawer open={mobileOpen} onClose={() => setMobileOpen(false)}>
        <Sidebar collapsed={false} onNavigate={() => setMobileOpen(false)} />
      </Drawer>

      {/* Main column */}
      <div className="flex min-w-0 flex-1 flex-col overflow-hidden">
        <a
          href="#main-content"
          className="sr-only focus:not-sr-only focus:fixed focus:top-4 focus:left-4 focus:z-50 focus:rounded-md focus:bg-primary focus:px-4 focus:py-2 focus:text-primary-foreground focus:shadow-lg"
        >
          Skip to main content
        </a>
        <Header
          onToggleSidebar={() => setMobileOpen(true)}
          onToggleCollapse={() => setCollapsed((value) => !value)}
        />
        <main id="main-content" className="flex-1 overflow-y-auto p-4 md:p-6" tabIndex={-1}>
          {children}
        </main>
      </div>
    </div>
  );
}
