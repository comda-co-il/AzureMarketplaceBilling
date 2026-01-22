import { useParams, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Layout } from '../components/Layout';
import { UsageHistory } from '../components/Billing';
import { Card, CardHeader, CardBody, Button } from '../components/Common';
import { useSubscription } from '../hooks/useSubscription';

export function UsagePage() {
  const { id } = useParams<{ id: string }>();
  const { t } = useTranslation();
  const { subscription, loading, error } = useSubscription(id);

  if (loading) {
    return (
      <Layout showSidebar subscriptionId={id}>
        <div className="ct-usage-page ct-usage-page--loading">
          <div className="ct-spinner ct-spinner--large"></div>
          <p>{t('common.loading')}</p>
        </div>
      </Layout>
    );
  }

  if (error || !subscription) {
    return (
      <Layout>
        <div className="ct-usage-page ct-usage-page--error">
          <h1>{t('usage.title')}</h1>
          <p>{error || t('errors.loadingSubscription')}</p>
          <Link to="/pricing">
            <Button>{t('nav.pricing')}</Button>
          </Link>
        </div>
      </Layout>
    );
  }

  return (
    <Layout showSidebar subscriptionId={id}>
      <div className="ct-usage-page">
        <header className="ct-usage-page__header">
          <div>
            <h1 className="ct-usage-page__title">{t('usage.title')}</h1>
            <p className="ct-usage-page__subtitle">
              {subscription.companyName} - {subscription.plan?.name}
            </p>
          </div>
          <Link to={`/dashboard/${id}`}>
            <Button variant="outline">{t('common.back')}</Button>
          </Link>
        </header>

        <Card>
          <CardHeader>
            <h2>{t('usage.title')}</h2>
          </CardHeader>
          <CardBody>
            <UsageHistory subscriptionId={id!} />
          </CardBody>
        </Card>
      </div>
    </Layout>
  );
}
