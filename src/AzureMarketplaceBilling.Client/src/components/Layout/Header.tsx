import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { LanguageSelector } from '../Common';

interface HeaderProps {
  showNav?: boolean;
  subscriptionId?: string;
}

export function Header({ showNav = true, subscriptionId }: HeaderProps) {
  const { t } = useTranslation();

  return (
    <header className="ct-header-container">
      <div className="ct-header-logo">
        <Link to="/" className="ct-header-logo__link">
          <span className="ct-header-logo__icon">🔐</span>
          <span className="ct-header-logo__text">{t('common.appName')}</span>
        </Link>
      </div>
      <div className="ct-header-actions">
        {showNav && (
          <nav className="ct-header-nav">
            <Link to="/pricing" className="ct-header-nav__item">
              {t('nav.pricing')}
            </Link>
            {subscriptionId && (
              <Link
                to={`/dashboard/${subscriptionId}`}
                className="ct-header-nav__item"
              >
                {t('nav.dashboard')}
              </Link>
            )}
            <Link to="/admin" className="ct-header-nav__item">
              {t('nav.admin')}
            </Link>
          </nav>
        )}
        <LanguageSelector />
      </div>
    </header>
  );
}
