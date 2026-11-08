import { useEffect, useRef } from 'react';
import { createPortal } from 'react-dom';
import { cn } from '../../lib/utils';

/**
 * Lightweight accessible drawer (left side). Used for the mobile sidebar.
 * - Locks body scroll while open
 * - Closes on Escape
 * - Focuses the panel on open
 */
export function Drawer({ open, onClose, children, className }) {
  const panelRef = useRef(null);

  useEffect(() => {
    if (!open) return undefined;
    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    const handleKeyDown = (event) => {
      if (event.key === 'Escape') onClose?.();
    };
    document.addEventListener('keydown', handleKeyDown);

    // Move focus into the panel for keyboard users.
    const focusTimer = window.setTimeout(() => panelRef.current?.focus(), 0);

    return () => {
      document.body.style.overflow = previousOverflow;
      document.removeEventListener('keydown', handleKeyDown);
      window.clearTimeout(focusTimer);
    };
  }, [open, onClose]);

  if (!open) return null;

  return createPortal(
    <div
      className="fixed inset-0 z-50 flex"
      role="dialog"
      aria-modal="true"
    >
      <button
        type="button"
        aria-label="Close menu"
        className="absolute inset-0 bg-black/40 backdrop-blur-sm"
        onClick={onClose}
      />
      <div
        ref={panelRef}
        tabIndex={-1}
        className={cn(
          'relative z-10 h-full w-72 max-w-[85vw] bg-sidebar text-sidebar-foreground shadow-xl outline-none',
          className,
        )}
      >
        {children}
      </div>
    </div>,
    document.body,
  );
}
