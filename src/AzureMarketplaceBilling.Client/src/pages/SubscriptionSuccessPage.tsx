import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { Button, Card, CardBody } from '../components/Common';
import { subscriptionsApi } from '../services/api';
import type { Subscription } from '../types';

export function SubscriptionSuccessPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
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
      setError('Failed to load subscription details');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="ct-success ct-success--loading">
          <div className="ct-spinner ct-spinner--large"></div>
          <p>Loading subscription details...</p>
        </div>
      </Layout>
    );
  }

  if (error || !subscription) {
    return (
      <Layout>
        <div className="ct-success ct-success--error">
          <h1>Oops!</h1>
          <p>{error || 'Subscription not found'}</p>
          <Button onClick={() => navigate('/pricing')}>
            View Pricing Plans
          </Button>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="ct-success">
        <div className="ct-success__icon">✓</div>
        <h1 className="ct-success__title">Subscription Created Successfully!</h1>
        <p className="ct-success__subtitle">
          Welcome to ComsignTrust CMS, {subscription.companyName}!
        </p>

        <Card className="ct-success__details">
          <CardBody>
            <h2>Subscription Details</h2>
            <div className="ct-success__info-grid">
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Subscription ID</span>
                <span className="ct-success__info-value ct-success__info-value--mono">
                  {subscription.id}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Company</span>
                <span className="ct-success__info-value">
                  {subscription.companyName}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Contact</span>
                <span className="ct-success__info-value">
                  {subscription.customerName}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Email</span>
                <span className="ct-success__info-value">
                  {subscription.customerEmail}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Plan</span>
                <span className="ct-success__info-value ct-success__info-value--highlight">
                  {subscription.plan?.name}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Monthly Price</span>
                <span className="ct-success__info-value">
                  ${subscription.plan?.monthlyPrice}/month
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Start Date</span>
                <span className="ct-success__info-value">
                  {new Date(subscription.startDate).toLocaleDateString()}
                </span>
              </div>
              <div className="ct-success__info-item">
                <span className="ct-success__info-label">Billing Period</span>
                <span className="ct-success__info-value">
                  {new Date(subscription.billingPeriodStart).toLocaleDateString()} - {' '}
                  {new Date(subscription.billingPeriodEnd).toLocaleDateString()}
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
            Go to Dashboard
          </Button>
          <Button
            variant="outline"
            size="large"
            onClick={() => navigate('/pricing')}
          >
            View All Plans
          </Button>
        </div>

        <p className="ct-success__note">
          💡 Save your Subscription ID - you'll need it to access your dashboard.
        </p>
      </div>
    </Layout>
  );
}
