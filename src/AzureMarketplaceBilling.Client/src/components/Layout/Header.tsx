import { Link } from 'react-router-dom';

interface HeaderProps {
  showNav?: boolean;
  subscriptionId?: string;
}

export function Header({ showNav = true, subscriptionId }: HeaderProps) {
  return (
    <header className="ct-header-container">
      <div className="ct-header-logo">
        <Link to="/" className="ct-header-logo__link">
          <span className="ct-header-logo__icon">🔐</span>
          <span className="ct-header-logo__text">ComsignTrust CMS</span>
        </Link>
      </div>
      {showNav && (
        <nav className="ct-header-nav">
          <Link to="/pricing" className="ct-header-nav__item">
            Pricing
          </Link>
          {subscriptionId && (
            <Link
              to={`/dashboard/${subscriptionId}`}
              className="ct-header-nav__item"
            >
              Dashboard
            </Link>
          )}
          <Link to="/admin" className="ct-header-nav__item">
            Admin
          </Link>
        </nav>
      )}
    </header>
  );
}
