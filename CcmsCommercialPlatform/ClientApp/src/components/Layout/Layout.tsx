import type { ReactNode } from 'react';
import { useTranslation } from 'react-i18next';
import { Header } from './Header';
import { Sidebar } from './Sidebar';

interface LayoutProps {
  children: ReactNode;
  showSidebar?: boolean;
  subscriptionId?: string;
}

export function Layout({ children, showSidebar = false, subscriptionId }: LayoutProps) {
  const { t } = useTranslation();

  const sidebarItems = subscriptionId
    ? [
        { path: `/dashboard/${subscriptionId}`, label: t('nav.dashboard'), icon: '📊' },
        { path: `/dashboard/${subscriptionId}/history`, label: t('nav.usage'), icon: '📈' },
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
