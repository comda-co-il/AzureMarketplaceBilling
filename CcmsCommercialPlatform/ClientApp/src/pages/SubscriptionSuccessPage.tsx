import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Layout } from '../components/Layout';
import { Button, Card, CardBody } from '../components/Common';
import { subscriptionsApi } from '../services/api';
import type { Subscription } from '../types';

export function SubscriptionSuccessPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { t, i18n } = useTranslation();
  const [subscription, setSubscription] = useState<Subscription | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      loadSubscription();
    }
  }, [id]);

  const loadSubscription = async () => {
    try {
      setLoading(true);
      const data = await subscriptionsApi.getById(id!);
      setSubscription(data);
    } catch (err) {
      setError(t('errors.loadingSubscription'));
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString(i18n.language === 'he' ? 'he-IL' : 'en-US');
  };

  if (loading) {
    return (
      <Layout>
        <div className="ct-success ct-success--loading">
          <div className="ct-spinner ct-spinner--large"></div>
          <p>{t('common.loading')}</p>
        </div>
      </Layout>
    );
  }

  if (error || !subscription) {
    return (
      <Layout>
        <div className="ct-success ct-success--error">
          <h1>{t('common.error')}</h1>
          <p>{error || t('errors.loadingSubscription')}</p>
          <Button onClick={() => navigate('/pricing')}>
            {t('nav.pricing')}
          </Button>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="ct-success">
        <div className="ct-success__icon">✓</div>
        <h1 className="ct-success__title">{t('subscription.success.title')}</h1>
        <p className="ct-success__subtitle">
          {t('subscription.success.message')}
        </p>

        <Card className="ct-success__details">
          <CardBody>
            <h2>{t('common.details')}</h2>
            <div className="ct-success__info-grid">
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('subscription.success.subscriptionId')}</span>
                <span className="ct-success__info-value ct-success__info-value--mono">
                  {subscription.id}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('subscription.form.companyName')}</span>
                <span className="ct-success__info-value">
                  {subscription.companyName}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('subscription.form.customerName')}</span>
                <span className="ct-success__info-value">
                  {subscription.customerName}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('subscription.form.customerEmail')}</span>
                <span className="ct-success__info-value">
                  {subscription.customerEmail}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('subscription.success.plan')}</span>
                <span className="ct-success__info-value ct-success__info-value--highlight">
                  {subscription.plan?.name}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('common.price')}</span>
                <span className="ct-success__info-value">
                  ${subscription.plan?.monthlyPrice}/{t('common.month')}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('common.date')}</span>
                <span className="ct-success__info-value">
                  {formatDate(subscription.startDate)}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">{t('subscription.success.billingPeriod')}</span>
                <span className="ct-success__info-value">
                  {formatDate(subscription.billingPeriodStart)} - {formatDate(subscription.billingPeriodEnd)}
                </span>
              </div>
            </div>
          </CardBody>
        </Card>

        <div className="ct-success__actions">
          <Button
            variant="primary"
            size="large"
            onClick={() => navigate(`/dashboard/${subscription.id}`)}
          >
            {t('subscription.success.goToDashboard')}
          </Button>
          <Button
            variant="outline"
            size="large"
            onClick={() => navigate('/pricing')}
          >
            {t('nav.pricing')}
          </Button>
        </div>

        <p className="ct-success__note">
          💡 {t('subscription.success.subscriptionId')}
        </p>
      </div>
    </Layout>
  );
}
