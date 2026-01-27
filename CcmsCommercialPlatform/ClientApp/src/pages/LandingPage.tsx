import { useNavigate } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { Button } from '../components/Common';

export function LandingPage() {
  const navigate = useNavigate();

  const features = [
    { icon: '🔐', titleKey: 'pki', title: 'PKI', description: 'Complete subscription lifecycle management with Azure Marketplace integration.' },
    { icon: '🖨️', titleKey: 'print', title: 'Print', description: 'Track usage across 7 credential dimensions with automatic overage calculation.' },
    { icon: '💳', titleKey: 'desfire', title: 'DESFire', description: 'Monitor usage in real-time with visual progress indicators and alerts.' },
    { icon: '📡', titleKey: 'prox', title: 'Prox', description: 'Seamless integration with Azure Marketplace Metered Billing APIs.' },
    { icon: '👆', titleKey: 'biometric', title: 'Biometric', description: 'Complete subscription lifecycle management with Azure Marketplace integration.' },
    { icon: '📱', titleKey: 'wallet', title: 'Wallet', description: 'Track usage across 7 credential dimensions with automatic overage calculation.' },
    { icon: '🔑', titleKey: 'fido', title: 'FIDO', description: 'Monitor usage in real-time with visual progress indicators and alerts.' },
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
              Azure Marketplace - Billing Demo
            </h2>
            <p className="ct-hero__description">
              Experience the complete Azure Marketplace SaaS billing flow with metered usage tracking, overage calculations, and real-time billing simulation.
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
                Learn More
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
            Platform Features
          </h2>
          <p className="ct-features__subtitle">
            This application runs in demo mode, simulating Azure Marketplace billing without requiring actual Azure credentials. Perfect for testing and demonstration purposes.
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
            <h2 className="ct-cta__title">Demo Mode</h2>
            <p className="ct-cta__description">
              This application runs in demo mode, simulating Azure Marketplace billing without requiring actual Azure credentials. Perfect for testing and demonstration purposes.
            </p>
            <Button
              variant="primary"
              size="large"
              onClick={() => navigate('/pricing')}
            >
              View Pricing
            </Button>
          </div>
        </section>

        {/* Footer */}
        <footer className="ct-footer">
          <p className="ct-footer__text">
            © 2026 ComsignTrust. | 
            <span className="ct-footer__demo"> Demo Mode</span>
          </p>
        </footer>
      </div>
    </Layout>
  );
}
