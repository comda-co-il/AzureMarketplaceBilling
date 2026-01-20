import { useNavigate } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { Button } from '../components/Common';

export function LandingPage() {
  const navigate = useNavigate();

  const features = [
    { icon: '🔐', title: 'PKI Certificates', description: 'Issue and manage digital certificates for secure authentication' },
    { icon: '🖨️', title: 'Card Printing', description: 'High-quality ID card printing with advanced security features' },
    { icon: '💳', title: 'DESFire Encoding', description: 'Secure contactless card encoding for access control' },
    { icon: '📡', title: 'Prox Credentials', description: 'Legacy proximity card support and encoding' },
    { icon: '👆', title: 'Biometrics', description: 'Fingerprint and facial recognition enrollment' },
    { icon: '📱', title: 'Mobile Wallet', description: 'Digital credentials for Apple and Google Wallet' },
    { icon: '🔑', title: 'FIDO Authentication', description: 'Passwordless authentication with FIDO2 keys' },
  ];

  return (
    <Layout>
      <div className="ct-landing">
        {/* Hero Section */}
        <section className="ct-hero">
          <div className="ct-hero__content">
            <h1 className="ct-hero__title">
              ComsignTrust <span className="ct-hero__highlight">CMS</span>
            </h1>
            <h2 className="ct-hero__subtitle">
              Credential Management System
            </h2>
            <p className="ct-hero__description">
              A comprehensive platform for managing physical and digital credentials 
              across your organization. From ID cards to mobile wallets, 
              secure your workforce with enterprise-grade credential management.
            </p>
            <div className="ct-hero__actions">
              <Button
                variant="primary"
                size="large"
                onClick={() => navigate('/pricing')}
              >
                View Pricing
              </Button>
              <Button
                variant="outline"
                size="large"
                onClick={() => navigate('/pricing')}
              >
                Start Free Trial
              </Button>
            </div>
          </div>
          <div className="ct-hero__visual">
            <div className="ct-hero__card-stack">
              <div className="ct-hero__card ct-hero__card--1">🪪</div>
              <div className="ct-hero__card ct-hero__card--2">🔐</div>
              <div className="ct-hero__card ct-hero__card--3">📱</div>
            </div>
          </div>
        </section>

        {/* Features Section */}
        <section className="ct-features">
          <h2 className="ct-features__title">
            Seven Credential Types, One Platform
          </h2>
          <p className="ct-features__subtitle">
            Manage all your organization's credential needs from a single, unified platform
          </p>
          <div className="ct-features__grid">
            {features.map((feature, index) => (
              <div key={index} className="ct-feature-card">
                <div className="ct-feature-card__icon">{feature.icon}</div>
                <h3 className="ct-feature-card__title">{feature.title}</h3>
                <p className="ct-feature-card__description">{feature.description}</p>
              </div>
            ))}
          </div>
        </section>

        {/* CTA Section */}
        <section className="ct-cta">
          <div className="ct-cta__content">
            <h2 className="ct-cta__title">Ready to Transform Your Credential Management?</h2>
            <p className="ct-cta__description">
              Join thousands of organizations that trust ComsignTrust CMS 
              for their credential management needs.
            </p>
            <Button
              variant="primary"
              size="large"
              onClick={() => navigate('/pricing')}
            >
              Get Started Today
            </Button>
          </div>
        </section>

        {/* Footer */}
        <footer className="ct-footer">
          <p className="ct-footer__text">
            © 2026 ComsignTrust. All rights reserved. | 
            <span className="ct-footer__demo"> Demo Application for Azure Marketplace Billing</span>
          </p>
        </footer>
      </div>
    </Layout>
  );
}
