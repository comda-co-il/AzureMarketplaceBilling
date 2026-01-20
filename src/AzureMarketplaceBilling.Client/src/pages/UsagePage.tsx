import { useParams, Link } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { UsageHistory } from '../components/Billing';
import { Card, CardHeader, CardBody, Button } from '../components/Common';
import { useSubscription } from '../hooks/useSubscription';

export function UsagePage() {
  const { id } = useParams<{ id: string }>();
  const { subscription, loading, error } = useSubscription(id);

  if (loading) {
    return (
      <Layout showSidebar subscriptionId={id}>
        <div className="ct-usage-page ct-usage-page--loading">
          <div className="ct-spinner ct-spinner--large"></div>
          <p>Loading usage history...</p>
        </div>
      </Layout>
    );
  }

  if (error || !subscription) {
    return (
      <Layout>
        <div className="ct-usage-page ct-usage-page--error">
          <h1>Usage History Not Found</h1>
          <p>{error || 'Could not load usage history.'}</p>
          <Link to="/pricing">
            <Button>View Pricing Plans</Button>
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
            <h1 className="ct-usage-page__title">Usage History</h1>
            <p className="ct-usage-page__subtitle">
              {subscription.companyName} - {subscription.plan?.name} Plan
            </p>
          </div>
          <Link to={`/dashboard/${id}`}>
            <Button variant="outline">Back to Dashboard</Button>
          </Link>
        </header>

        <Card>
          <CardHeader>
            <h2>Usage Records</h2>
          </CardHeader>
          <CardBody>
            <UsageHistory subscriptionId={id!} />
          </CardBody>
        </Card>
      </div>
    </Layout>
  );
}
