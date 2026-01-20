import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardBody, Button, Table, Modal, Alert } from '../components/Common';
import { adminApi } from '../services/api';
import type { AdminDashboardStats, Subscription, UsageEvent, PaginatedResponse } from '../types';
import { SubscriptionStatus } from '../types';

export function AdminPage() {
  const navigate = useNavigate();
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [password, setPassword] = useState('');
  const [authError, setAuthError] = useState<string | null>(null);
  const [authenticating, setAuthenticating] = useState(false);

  // Dashboard data
  const [stats, setStats] = useState<AdminDashboardStats | null>(null);
  const [subscriptions, setSubscriptions] = useState<PaginatedResponse<Subscription> | null>(null);
  const [usageEvents, setUsageEvents] = useState<PaginatedResponse<UsageEvent> | null>(null);
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<'overview' | 'subscriptions' | 'events'>('overview');

  // Selected event for modal
  const [selectedEvent, setSelectedEvent] = useState<UsageEvent | null>(null);

  useEffect(() => {
    if (isAuthenticated) {
      loadData();
    }
  }, [isAuthenticated]);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setAuthenticating(true);
    setAuthError(null);

    const valid = await adminApi.verifyPassword(password);
    if (valid) {
      setIsAuthenticated(true);
    } else {
      setAuthError('Invalid password. Hint: admin123');
    }
    setAuthenticating(false);
  };

  const loadData = async () => {
    try {
      setLoading(true);
      const [statsData, subsData, eventsData] = await Promise.all([
        adminApi.getDashboardStats(password),
        adminApi.getSubscriptions(password),
        adminApi.getUsageEvents(password),
      ]);
      setStats(statsData);
      setSubscriptions(subsData);
      setUsageEvents(eventsData);
    } catch (error) {
      console.error('Failed to load admin data:', error);
    } finally {
      setLoading(false);
    }
  };

  const statusLabel = {
    [SubscriptionStatus.Active]: 'Active',
    [SubscriptionStatus.Suspended]: 'Suspended',
    [SubscriptionStatus.Cancelled]: 'Cancelled',
  };

  const subscriptionColumns = [
    { key: 'companyName', header: 'Company' },
    { 
      key: 'plan', 
      header: 'Plan',
      render: (sub: Subscription) => sub.plan?.name || sub.planId,
    },
    { 
      key: 'status', 
      header: 'Status',
      render: (sub: Subscription) => (
        <span className={`ct-status-badge ct-status-badge--${SubscriptionStatus[sub.status].toLowerCase()}`}>
          {statusLabel[sub.status]}
        </span>
      ),
    },
    { 
      key: 'startDate', 
      header: 'Start Date',
      render: (sub: Subscription) => new Date(sub.startDate).toLocaleDateString(),
    },
    { 
      key: 'customerEmail', 
      header: 'Contact',
      render: (sub: Subscription) => sub.customerEmail,
    },
  ];

  const eventColumns = [
    { 
      key: 'createdAt', 
      header: 'Timestamp',
      render: (event: UsageEvent) => new Date(event.createdAt).toLocaleString(),
    },
    { 
      key: 'resourceId', 
      header: 'Subscription',
      render: (event: UsageEvent) => event.resourceId.substring(0, 8) + '...',
    },
    { key: 'dimension', header: 'Dimension' },
    { key: 'quantity', header: 'Quantity' },
    { 
      key: 'amount', 
      header: 'Amount',
      render: (event: UsageEvent) => `$${event.amount.toFixed(2)}`,
    },
    { 
      key: 'status', 
      header: 'Status',
      render: (event: UsageEvent) => (
        <span className={`ct-status-badge ct-status-badge--${event.status.toLowerCase()}`}>
          {event.status}
        </span>
      ),
    },
  ];

  if (!isAuthenticated) {
    return (
      <Layout>
        <div className="ct-admin-login">
          <Card className="ct-admin-login__card">
            <CardHeader>
              <h1>Admin Access</h1>
            </CardHeader>
            <CardBody>
              <form onSubmit={handleLogin} className="ct-admin-login__form">
                {authError && (
                  <Alert type="error" message={authError} autoClose={false} />
                )}
                <div className="ct-input-group">
                  <label className="ct-label--primary">Password</label>
                  <input
                    type="password"
                    className="ct-input--primary"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="Enter admin password"
                    required
                  />
                </div>
                <Button variant="primary" type="submit" loading={authenticating}>
                  Login
                </Button>
              </form>
            </CardBody>
          </Card>
        </div>
      </Layout>
    );
  }

  if (loading && !stats) {
    return (
      <Layout>
        <div className="ct-admin ct-admin--loading">
          <div className="ct-spinner ct-spinner--large"></div>
          <p>Loading admin dashboard...</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="ct-admin">
        <header className="ct-admin__header">
          <h1 className="ct-admin__title">Admin Dashboard</h1>
          <Button variant="outline" onClick={loadData} disabled={loading}>
            {loading ? 'Refreshing...' : 'Refresh Data'}
          </Button>
        </header>

        {/* Stats Cards */}
        <div className="ct-admin__stats">
          <Card className="ct-stat-card">
            <CardBody>
              <div className="ct-stat-card__icon">📊</div>
              <div className="ct-stat-card__value">{stats?.totalActiveSubscriptions || 0}</div>
              <div className="ct-stat-card__label">Active Subscriptions</div>
            </CardBody>
          </Card>
          <Card className="ct-stat-card">
            <CardBody>
              <div className="ct-stat-card__icon">💰</div>
              <div className="ct-stat-card__value">
                ${stats?.monthlyRecurringRevenue.toFixed(2) || '0.00'}
              </div>
              <div className="ct-stat-card__label">Monthly Recurring Revenue</div>
            </CardBody>
          </Card>
          <Card className="ct-stat-card">
            <CardBody>
              <div className="ct-stat-card__icon">📈</div>
              <div className="ct-stat-card__value">
                ${stats?.totalMeteredRevenue.toFixed(2) || '0.00'}
              </div>
              <div className="ct-stat-card__label">Metered Revenue (This Month)</div>
            </CardBody>
          </Card>
          <Card className="ct-stat-card">
            <CardBody>
              <div className="ct-stat-card__icon">⚡</div>
              <div className="ct-stat-card__value">{stats?.usageEventsToday || 0}</div>
              <div className="ct-stat-card__label">Usage Events Today</div>
            </CardBody>
          </Card>
        </div>

        {/* Tabs */}
        <div className="ct-admin__tabs">
          <button
            className={`ct-admin__tab ${activeTab === 'overview' ? 'ct-admin__tab--active' : ''}`}
            onClick={() => setActiveTab('overview')}
          >
            Overview
          </button>
          <button
            className={`ct-admin__tab ${activeTab === 'subscriptions' ? 'ct-admin__tab--active' : ''}`}
            onClick={() => setActiveTab('subscriptions')}
          >
            Subscriptions
          </button>
          <button
            className={`ct-admin__tab ${activeTab === 'events' ? 'ct-admin__tab--active' : ''}`}
            onClick={() => setActiveTab('events')}
          >
            Billing Events
          </button>
        </div>

        {/* Tab Content */}
        {activeTab === 'overview' && (
          <div className="ct-admin__overview">
            <Card>
              <CardHeader>
                <h2>Subscriptions by Plan</h2>
              </CardHeader>
              <CardBody>
                <div className="ct-plan-distribution">
                  {stats?.subscriptionsByPlan && Object.entries(stats.subscriptionsByPlan).map(([plan, count]) => (
                    <div key={plan} className="ct-plan-distribution__item">
                      <span className="ct-plan-distribution__plan">{plan}</span>
                      <div className="ct-plan-distribution__bar">
                        <div 
                          className="ct-plan-distribution__fill"
                          style={{ 
                            width: `${(count / (stats.totalActiveSubscriptions || 1)) * 100}%` 
                          }}
                        />
                      </div>
                      <span className="ct-plan-distribution__count">{count}</span>
                    </div>
                  ))}
                  {(!stats?.subscriptionsByPlan || Object.keys(stats.subscriptionsByPlan).length === 0) && (
                    <p className="ct-admin__empty">No subscriptions yet</p>
                  )}
                </div>
              </CardBody>
            </Card>
          </div>
        )}

        {activeTab === 'subscriptions' && (
          <Card>
            <CardHeader>
              <h2>All Subscriptions</h2>
            </CardHeader>
            <CardBody>
              <Table
                columns={subscriptionColumns}
                data={subscriptions?.data || []}
                keyExtractor={(sub) => sub.id}
                onRowClick={(sub) => navigate(`/dashboard/${sub.id}`)}
                emptyMessage="No subscriptions found"
              />
            </CardBody>
          </Card>
        )}

        {activeTab === 'events' && (
          <Card>
            <CardHeader>
              <h2>Metered Billing Events</h2>
              <p className="ct-admin__events-description">
                These events represent usage reports that would be sent to Azure Marketplace Metered Billing API
              </p>
            </CardHeader>
            <CardBody>
              <Table
                columns={eventColumns}
                data={usageEvents?.data || []}
                keyExtractor={(event) => event.id}
                onRowClick={(event) => setSelectedEvent(event)}
                emptyMessage="No billing events recorded"
              />
            </CardBody>
          </Card>
        )}

        {/* Event Detail Modal */}
        <Modal
          isOpen={!!selectedEvent}
          onClose={() => setSelectedEvent(null)}
          title="Billing Event Details"
          size="medium"
        >
          {selectedEvent && (
            <div className="ct-event-detail">
              <h3>Azure Metered Billing API Payload</h3>
              <pre className="ct-event-detail__json">
{JSON.stringify({
  resourceId: selectedEvent.resourceId,
  planId: selectedEvent.planId,
  dimension: selectedEvent.dimension,
  quantity: selectedEvent.quantity,
  effectiveStartTime: selectedEvent.effectiveStartTime,
}, null, 2)}
              </pre>
              <div className="ct-event-detail__info">
                <div className="ct-event-detail__row">
                  <span>Status:</span>
                  <span className={`ct-status-badge ct-status-badge--${selectedEvent.status.toLowerCase()}`}>
                    {selectedEvent.status}
                  </span>
                </div>
                <div className="ct-event-detail__row">
                  <span>Amount:</span>
                  <span>${selectedEvent.amount.toFixed(2)}</span>
                </div>
                <div className="ct-event-detail__row">
                  <span>Created:</span>
                  <span>{new Date(selectedEvent.createdAt).toLocaleString()}</span>
                </div>
              </div>
            </div>
          )}
        </Modal>
      </div>
    </Layout>
  );
}
