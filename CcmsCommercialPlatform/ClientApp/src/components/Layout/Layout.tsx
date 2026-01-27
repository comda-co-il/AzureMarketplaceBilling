import type { ReactNode } from 'react';
import { Header } from './Header';
import { Sidebar } from './Sidebar';

interface LayoutProps {
  children: ReactNode;
  showSidebar?: boolean;
  subscriptionId?: string;
}

export function Layout({ children, showSidebar = false, subscriptionId }: LayoutProps) {
  const sidebarItems = subscriptionId
    ? [
        { path: `/dashboard/${subscriptionId}`, label: 'Dashboard', icon: '📊' },
        { path: `/dashboard/${subscriptionId}/history`, label: 'Usage History', icon: '📈' },
      ]
    : [];

  return (
    <div className="ct-layout">
      <Header showNav={true} subscriptionId={subscriptionId} />
      <div className="ct-layout__content">
        {showSidebar && subscriptionId && <Sidebar items={sidebarItems} />}
        <main className={`ct-main ${showSidebar ? 'ct-main--with-sidebar' : ''}`}>
          {children}
        </main>
      </div>
    </div>
  );
}
