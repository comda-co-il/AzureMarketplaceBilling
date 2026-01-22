import { useEffect, useState } from 'react';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardBody, Button, Modal } from '../components/Common';
import { azureWebhookApi } from '../services/api';
import type { AzureWebhookEvent, PaginatedResponse } from '../types';

/**
 * Azure Webhooks Development Page
 * 
 * This page displays all webhook events received from Azure Marketplace.
 * It's used for development and debugging of the Connection Webhook integration.
 * 
 * This page is for verifying webhook data from Azure Partner Center -> Technical Configuration -> Connection Webhook.
 */
export function AzureWebhooksPage() {
  const [events, setEvents] = useState<AzureWebhookEvent[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [total, setTotal] = useState(0);
  const [selectedEvent, setSelectedEvent] = useState<AzureWebhookEvent | null>(null);
  const [showClearConfirm, setShowClearConfirm] = useState(false);

  const fetchEvents = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await azureWebhookApi.getEvents(page, 20);
      setEvents(response.data);
      setTotalPages(response.totalPages);
      setTotal(response.total);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch webhook events');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEvents();
  }, [page]);

  const handleClearEvents = async () => {
    try {
      await azureWebhookApi.clearEvents();
      setShowClearConfirm(false);
      setPage(1);
      fetchEvents();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to clear events');
    }
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleString();
  };

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
  };

  const formatJson = (jsonStr: string) => {
    try {
      return JSON.stringify(JSON.parse(jsonStr), null, 2);
    } catch {
      return jsonStr;
    }
  };

  return (
    <Layout>
      <div className="ct-page-container">
        <div className="ct-admin-header">
          <h1 className="ct-admin-title">Azure Webhook Events</h1>
          <span className="ct-badge ct-badge--warning">Development Only</span>
        </div>

        <p className="ct-admin-description">
          This page displays all webhook events received from Azure Marketplace.
          Use this to debug and verify webhook integration during development.
        </p>

        {/* Actions Bar */}
        <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem', alignItems: 'center' }}>
          <Button onClick={fetchEvents} disabled={loading}>
            {loading ? 'Refreshing...' : 'Refresh'}
          </Button>
          <Button 
            variant="outline" 
            onClick={() => setShowClearConfirm(true)}
            disabled={events.length === 0}
          >
            Clear All Events
          </Button>
          <span className="ct-text-muted">
            Total events: {total}
          </span>
        </div>

        {error && (
          <div className="ct-alert ct-alert--error" style={{ marginBottom: '1rem' }}>
            {error}
          </div>
        )}

        {/* Events Table */}
        <Card>
          <CardHeader>
            <h3>Webhook Events</h3>
          </CardHeader>
          <CardBody>
            {loading && events.length === 0 ? (
              <div className="ct-loading">Loading events...</div>
            ) : events.length === 0 ? (
              <div className="ct-empty-state">
                <p>No webhook events received yet.</p>
                <p className="ct-text-muted" style={{ marginTop: '0.5rem' }}>
                  Events will appear here when Azure sends webhooks to your endpoint.
                </p>
              </div>
            ) : (
              <>
                <table className="ct-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Action</th>
                      <th>Subscription ID</th>
                      <th>Plan ID</th>
                      <th>Status</th>
                      <th>Received At</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {events.map((event) => (
                      <tr key={event.id}>
                        <td>{event.id}</td>
                        <td>
                          <span className={`ct-badge ct-badge--${
                            event.action?.toLowerCase() === 'subscribe' ? 'success' :
                            event.action?.toLowerCase() === 'unsubscribe' ? 'error' :
                            event.action?.toLowerCase() === 'suspend' ? 'warning' :
                            'default'
                          }`}>
                            {event.action || '-'}
                          </span>
                        </td>
                        <td>
                          <code style={{ fontSize: '0.75rem' }}>
                            {event.subscriptionId ? 
                              `${event.subscriptionId.substring(0, 8)}...` : 
                              '-'
                            }
                          </code>
                        </td>
                        <td>{event.planId || '-'}</td>
                        <td>{event.status || '-'}</td>
                        <td>{formatDate(event.receivedAt)}</td>
                        <td>
                          <Button 
                            variant="ghost" 
                            size="small"
                            onClick={() => setSelectedEvent(event)}
                          >
                            View Details
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>

                {/* Pagination */}
                {totalPages > 1 && (
                  <div style={{ 
                    display: 'flex', 
                    justifyContent: 'center', 
                    gap: '0.5rem', 
                    marginTop: '1rem' 
                  }}>
                    <Button
                      variant="outline"
                      size="small"
                      onClick={() => setPage(p => Math.max(1, p - 1))}
                      disabled={page === 1}
                    >
                      Previous
                    </Button>
                    <span style={{ 
                      display: 'flex', 
                      alignItems: 'center', 
                      padding: '0 1rem' 
                    }}>
                      Page {page} of {totalPages}
                    </span>
                    <Button
                      variant="outline"
                      size="small"
                      onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                      disabled={page === totalPages}
                    >
                      Next
                    </Button>
                  </div>
                )}
              </>
            )}
          </CardBody>
        </Card>

        {/* Webhook Endpoint Info */}
        <Card>
          <CardHeader>
            <h3>Webhook Endpoint Configuration</h3>
          </CardHeader>
          <CardBody>
            <p style={{ marginBottom: '1rem' }}>
              Configure this endpoint in Azure Partner Center → Technical Configuration → Connection Webhook:
            </p>
            <div style={{ 
              backgroundColor: 'var(--ct-bg-tertiary)', 
              padding: '1rem', 
              borderRadius: '4px',
              fontFamily: 'monospace',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}>
              <code>POST {window.location.origin}/api/webhook/azure</code>
              <Button 
                variant="ghost" 
                size="small"
                onClick={() => copyToClipboard(`${window.location.origin}/api/webhook/azure`)}
              >
                Copy
              </Button>
            </div>
          </CardBody>
        </Card>
      </div>

      {/* Event Details Modal */}
      {selectedEvent && (
        <Modal
          isOpen={true}
          onClose={() => setSelectedEvent(null)}
          title={`Webhook Event #${selectedEvent.id}`}
          size="large"
        >
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {/* Metadata */}
            <div>
              <h4 style={{ marginBottom: '0.5rem' }}>Event Metadata</h4>
              <table className="ct-table">
                <tbody>
                  <tr>
                    <td><strong>Action</strong></td>
                    <td>{selectedEvent.action || '-'}</td>
                  </tr>
                  <tr>
                    <td><strong>Subscription ID</strong></td>
                    <td>
                      <code>{selectedEvent.subscriptionId || '-'}</code>
                      {selectedEvent.subscriptionId && (
                        <Button 
                          variant="ghost" 
                          size="small"
                          onClick={() => copyToClipboard(selectedEvent.subscriptionId!)}
                          style={{ marginLeft: '0.5rem' }}
                        >
                          Copy
                        </Button>
                      )}
                    </td>
                  </tr>
                  <tr>
                    <td><strong>Publisher ID</strong></td>
                    <td>{selectedEvent.publisherId || '-'}</td>
                  </tr>
                  <tr>
                    <td><strong>Offer ID</strong></td>
                    <td>{selectedEvent.offerId || '-'}</td>
                  </tr>
                  <tr>
                    <td><strong>Plan ID</strong></td>
                    <td>{selectedEvent.planId || '-'}</td>
                  </tr>
                  <tr>
                    <td><strong>Operation ID</strong></td>
                    <td><code>{selectedEvent.operationId || '-'}</code></td>
                  </tr>
                  <tr>
                    <td><strong>Activity ID</strong></td>
                    <td><code>{selectedEvent.activityId || '-'}</code></td>
                  </tr>
                  <tr>
                    <td><strong>Status</strong></td>
                    <td>{selectedEvent.status || '-'}</td>
                  </tr>
                  <tr>
                    <td><strong>Azure Timestamp</strong></td>
                    <td>{selectedEvent.azureTimestamp ? formatDate(selectedEvent.azureTimestamp) : '-'}</td>
                  </tr>
                  <tr>
                    <td><strong>Received At</strong></td>
                    <td>{formatDate(selectedEvent.receivedAt)}</td>
                  </tr>
                  <tr>
                    <td><strong>Source IP</strong></td>
                    <td>{selectedEvent.sourceIp || '-'}</td>
                  </tr>
                </tbody>
              </table>
            </div>

            {/* Raw Payload */}
            <div>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                <h4>Raw Payload</h4>
                <Button 
                  variant="ghost" 
                  size="small"
                  onClick={() => copyToClipboard(selectedEvent.rawPayload)}
                >
                  Copy JSON
                </Button>
              </div>
              <pre style={{ 
                backgroundColor: 'var(--ct-bg-tertiary)', 
                padding: '1rem', 
                borderRadius: '4px',
                overflow: 'auto',
                maxHeight: '300px',
                fontSize: '0.75rem'
              }}>
                {formatJson(selectedEvent.rawPayload)}
              </pre>
            </div>

            {/* Headers */}
            {selectedEvent.headers && (
              <div>
                <h4 style={{ marginBottom: '0.5rem' }}>Request Headers</h4>
                <pre style={{ 
                  backgroundColor: 'var(--ct-bg-tertiary)', 
                  padding: '1rem', 
                  borderRadius: '4px',
                  overflow: 'auto',
                  maxHeight: '200px',
                  fontSize: '0.75rem',
                  whiteSpace: 'pre-wrap',
                  wordBreak: 'break-all'
                }}>
                  {selectedEvent.headers.split('; ').join('\n')}
                </pre>
              </div>
            )}
          </div>
        </Modal>
      )}

      {/* Clear Confirmation Modal */}
      <Modal
        isOpen={showClearConfirm}
        onClose={() => setShowClearConfirm(false)}
        title="Clear All Events?"
        size="small"
      >
        <p style={{ marginBottom: '1.5rem' }}>
          This will permanently delete all {total} webhook events. This action cannot be undone.
        </p>
        <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end' }}>
          <Button variant="outline" onClick={() => setShowClearConfirm(false)}>
            Cancel
          </Button>
          <Button variant="primary" onClick={handleClearEvents}>
            Clear All Events
          </Button>
        </div>
      </Modal>
    </Layout>
  );
}
