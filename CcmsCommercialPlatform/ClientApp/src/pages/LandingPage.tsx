import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Layout } from '../components/Layout';
import { Button } from '../components/Common';

export function LandingPage() {
  const navigate = useNavigate();
  const { t } = useTranslation();

  const features = [
    { icon: '🔐', titleKey: 'pki', title: t('dimensions.pki'), description: t('landing.features.subscription.description') },
    { icon: '🖨️', titleKey: 'print', title: t('dimensions.print'), description: t('landing.features.metered.description') },
    { icon: '💳', titleKey: 'desfire', title: t('dimensions.desfire'), description: t('landing.features.realtime.description') },
    { icon: '📡', titleKey: 'prox', title: t('dimensions.prox'), description: t('landing.features.azure.description') },
    { icon: '👆', titleKey: 'biometric', title: t('dimensions.biometric'), description: t('landing.features.subscription.description') },
    { icon: '📱', titleKey: 'wallet', title: t('dimensions.wallet'), description: t('landing.features.metered.description') },
    { icon: '🔑', titleKey: 'fido', title: t('dimensions.fido'), description: t('landing.features.realtime.description') },
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
              {t('landing.hero.title')} - {t('landing.hero.subtitle')}
            </h2>
            <p className="ct-hero__description">
              {t('landing.hero.description')}
            </p>
            <div className="ct-hero__actions">
              <Button
                variant="primary"
                size="large"
                onClick={() => navigate('/pricing')}
              >
                {t('landing.hero.viewPricing')}
              </Button>
              <Button
                variant="outline"
                size="large"
                onClick={() => navigate('/pricing')}
              >
                {t('landing.hero.learnMore')}
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
            {t('landing.features.title')}
          </h2>
          <p className="ct-features__subtitle">
            {t('landing.demo.description')}
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
            <h2 className="ct-cta__title">{t('landing.demo.title')}</h2>
            <p className="ct-cta__description">
              {t('landing.demo.description')}
            </p>
            <Button
              variant="primary"
              size="large"
              onClick={() => navigate('/pricing')}
            >
              {t('landing.hero.viewPricing')}
            </Button>
          </div>
        </section>

        {/* Footer */}
        <footer className="ct-footer">
          <p className="ct-footer__text">
            © 2026 ComsignTrust. | 
            <span className="ct-footer__demo"> {t('landing.demo.title')}</span>
          </p>
        </footer>
      </div>
    </Layout>
  );
}
