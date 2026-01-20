import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { UsageBar } from '../components/Billing';
import { Card, CardBody, CardHeader, Button, Alert } from '../components/Common';
import { useSubscription } from '../hooks/useSubscription';
import { TokenUsageType, TokenUsageTypeNames, SubscriptionStatus } from '../types';

export function DashboardPage() {
  const { id } = useParams<{ id: string }>();
  const { subscription, usage, loading, error, recordUsage, startNewBillingPeriod } = useSubscription(id);
  
  const [selectedDimension, setSelectedDimension] = useState<TokenUsageType>(TokenUsageType.Pki);
  const [quantity, setQuantity] = useState(10);
  const [recording, setRecording] = useState(false);
  const [alertMessage, setAlertMessage] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [resettingPeriod, setResettingPeriod] = useState(false);

  const handleRecordUsage = async () => {
    try {
      setRecording(true);
      setAlertMessage(null);
      await recordUsage(selectedDimension, quantity);
      setAlertMessage({ type: 'success', message: `Successfully recorded ${quantity} ${TokenUsageTypeNames[selectedDimension]}` });
    } catch (err) {
      setAlertMessage({ type: 'error', message: 'Failed to record usage. Please try again.' });
      console.error(err);
    } finally {
      setRecording(false);
    }
  };

  const handleNewBillingPeriod = async () => {
    if (!window.confirm('Are you sure you want to start a new billing period? This will reset all usage counters.')) {
      return;
    }
    
    try {
      setResettingPeriod(true);
      setAlertMessage(null);
      await startNewBillingPeriod();
      setAlertMessage({ type: 'success', message: 'New billing period started successfully!' });
    } catch (err) {
      setAlertMessage({ type: 'error', message: 'Failed to start new billing period.' });
      console.error(err);
    } finally {
      setResettingPeriod(false);
    }
  };

  if (loading) {
    return (
      <Layout showSidebar subscriptionId={id}>
        <div className="ct-dashboard ct-dashboard--loading">
          <div className="ct-spinner ct-spinner--large"></div>
          <p>Loading dashboard...</p>
        </div>
      </Layout>
    );
  }

  if (error || !subscription || !usage) {
    return (
      <Layout>
        <div className="ct-dashboard ct-dashboard--error">
          <h1>Dashboard Not Found</h1>
          <p>{error || 'Could not load subscription data.'}</p>
          <p className="ct-dashboard__hint">
            Make sure you have a valid subscription ID. 
            <Link to="/pricing"> Create a new subscription</Link>
          </p>
        </div>
      </Layout>
    );
  }

  const statusLabel = {
    [SubscriptionStatus.Active]: 'Active',
    [SubscriptionStatus.Suspended]: 'Suspended',
    [SubscriptionStatus.Cancelled]: 'Cancelled',
  };

  const statusClass = {
    [SubscriptionStatus.Active]: 'ct-status--active',
    [SubscriptionStatus.Suspended]: 'ct-status--suspended',
    [SubscriptionStatus.Cancelled]: 'ct-status--cancelled',
  };

  return (
    <Layout showSidebar subscriptionId={id}>
      <div className="ct-dashboard">
        {alertMessage && (
          <Alert
            type={alertMessage.type}
            message={alertMessage.message}
            onClose={() => setAlertMessage(null)}
          />
        )}

        {/* Header */}
        <header className="ct-dashboard__header">
          <div className="ct-dashboard__header-info">
            <h1 className="ct-dashboard__title">{subscription.companyName}</h1>
            <div className="ct-dashboard__meta">
              <span className={`ct-status ${statusClass[subscription.status]}`}>
                {statusLabel[subscription.status]}
              </span>
              <span className="ct-dashboard__plan">
                {subscription.plan?.name} Plan - ${subscription.plan?.monthlyPrice}/month
              </span>
            </div>
          </div>
          <div className="ct-dashboard__header-actions">
            <Link to={`/dashboard/${id}/history`}>
              <Button variant="outline">View History</Button>
            </Link>
          </div>
        </header>

        {/* Billing Period Info */}
        <Card className="ct-dashboard__billing-period">
          <CardBody>
            <div className="ct-billing-period">
              <div className="ct-billing-period__info">
                <h3>Current Billing Period</h3>
                <p>
                  {new Date(usage.billingPeriodStart).toLocaleDateString()} - {' '}
                  {new Date(usage.billingPeriodEnd).toLocaleDateString()}
                </p>
              </div>
              <div className="ct-billing-period__summary">
                <div className="ct-billing-period__item">
                  <span className="ct-billing-period__label">Base Fee</span>
                  <span className="ct-billing-period__value">
                    ${subscription.plan?.monthlyPrice.toFixed(2)}
                  </span>
                </div>
                <div className="ct-billing-period__item">
                  <span className="ct-billing-period__label">Overage Charges</span>
                  <span className="ct-billing-period__value ct-billing-period__value--overage">
                    ${usage.totalOverageCharges.toFixed(2)}
                  </span>
                </div>
                <div className="ct-billing-period__item ct-billing-period__item--total">
                  <span className="ct-billing-period__label">Estimated Total</span>
                  <span className="ct-billing-period__value">
                    ${((subscription.plan?.monthlyPrice || 0) + usage.totalOverageCharges).toFixed(2)}
                  </span>
                </div>
              </div>
            </div>
          </CardBody>
        </Card>

        {/* Usage Overview */}
        <Card className="ct-dashboard__usage">
          <CardHeader>
            <h2>Usage Overview</h2>
          </CardHeader>
          <CardBody>
            <div className="ct-usage-grid">
              {usage.dimensions.map((dimension) => (
                <UsageBar key={dimension.dimensionId} dimension={dimension} />
              ))}
            </div>
          </CardBody>
        </Card>

        {/* Demo Controls */}
        <Card className="ct-dashboard__demo-controls">
          <CardHeader>
            <h2>🧪 Demo Controls</h2>
            <span className="ct-demo-badge">Simulation Mode</span>
          </CardHeader>
          <CardBody>
            <p className="ct-demo-controls__description">
              Use these controls to simulate credential operations and see how usage tracking and overage billing works.
            </p>
            
            <div className="ct-demo-controls__form">
              <div className="ct-input-group">
                <label className="ct-label--primary">Credential Type</label>
                <select
                  className="ct-input--primary"
                  value={selectedDimension}
                  onChange={(e) => setSelectedDimension(Number(e.target.value) as TokenUsageType)}
                >
                  {Object.entries(TokenUsageTypeNames).map(([value, name]) => (
                    <option key={value} value={value}>
                      {name}
                    </option>
                  ))}
                </select>
              </div>
              
              <div className="ct-input-group">
                <label className="ct-label--primary">Quantity</label>
                <input
                  type="number"
                  className="ct-input--primary"
                  value={quantity}
                  onChange={(e) => setQuantity(Math.max(1, parseInt(e.target.value) || 1))}
                  min="1"
                  max="1000"
                />
              </div>
              
              <Button
                variant="primary"
                onClick={handleRecordUsage}
                loading={recording}
              >
                Simulate Usage
              </Button>
            </div>

            <div className="ct-demo-controls__divider"></div>

            <div className="ct-demo-controls__period">
              <h3>Billing Period Controls</h3>
              <p>Start a new billing period to reset all usage counters and test the billing cycle.</p>
              <Button
                variant="outline"
                onClick={handleNewBillingPeriod}
                loading={resettingPeriod}
              >
                Start New Billing Period
              </Button>
            </div>
          </CardBody>
        </Card>
      </div>
    </Layout>
  );
}
