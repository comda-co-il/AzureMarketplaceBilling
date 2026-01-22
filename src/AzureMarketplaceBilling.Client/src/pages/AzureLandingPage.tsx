import { useEffect, useState } from 'react';
import { useSearchParams, useLocation } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardBody } from '../components/Common';

/**
 * Azure Marketplace Landing Page (Development)
 * 
 * This page receives query parameters from Azure Marketplace when a customer
 * subscribes to the SaaS offer. It extracts and displays all parameters for
 * development and debugging purposes.
 * 
 * Expected Azure parameters:
 * - token: Marketplace identification token (used to resolve subscription details)
 * - subscription: Subscription ID (for direct API calls)
 * - ms-correlationid: Microsoft correlation ID for tracing
 * - offer: Offer ID
 * - plan: Plan ID
 * 
 * This page is for Technical Configuration -> Landing page URL in Azure Partner Center.
 */
export function AzureLandingPage() {
  const [searchParams] = useSearchParams();
  const location = useLocation();
  const [params, setParams] = useState<{ key: string; value: string }[]>([]);
  const [rawUrl, setRawUrl] = useState('');

  useEffect(() => {
    // Extract all query parameters
    const paramList: { key: string; value: string }[] = [];
    searchParams.forEach((value, key) => {
      paramList.push({ key, value });
    });
    setParams(paramList);

    // Store the raw URL for reference
    setRawUrl(window.location.href);
  }, [searchParams]);

  // Known Azure Marketplace parameters
  const knownParams = [
    { key: 'token', description: 'Marketplace identification token (resolve via API to get subscription details)' },
    { key: 'subscription', description: 'Azure subscription ID' },
    { key: 'ms-correlationid', description: 'Microsoft correlation ID for support and tracing' },
    { key: 'offer', description: 'Offer ID from Azure Marketplace' },
    { key: 'plan', description: 'Selected plan ID' },
  ];

  const getParamDescription = (key: string): string | undefined => {
    return knownParams.find(p => p.key.toLowerCase() === key.toLowerCase())?.description;
  };

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
  };

  return (
    <Layout>
      <div className="ct-page-container">
        <div className="ct-admin-header">
          <h1 className="ct-admin-title">Azure Marketplace Landing Page</h1>
          <span className="ct-badge ct-badge--warning">Development Only</span>
        </div>
        
        <p className="ct-admin-description">
          This page receives query parameters from Azure Marketplace when customers access
          your SaaS offer. Use this for development and testing of your subscription flow.
        </p>

        {/* Raw URL Display */}
        <Card>
          <CardHeader>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <h3>Raw URL</h3>
              <button 
                className="ct-button ct-button--small ct-button--outline"
                onClick={() => copyToClipboard(rawUrl)}
              >
                Copy URL
              </button>
            </div>
          </CardHeader>
          <CardBody>
            <div className="ct-code-block" style={{ 
              backgroundColor: 'var(--ct-bg-tertiary)', 
              padding: '1rem', 
              borderRadius: '4px',
              wordBreak: 'break-all',
              fontFamily: 'monospace',
              fontSize: '0.875rem'
            }}>
              {rawUrl}
            </div>
          </CardBody>
        </Card>

        {/* Parameters Table */}
        <Card>
          <CardHeader>
            <h3>Query Parameters ({params.length})</h3>
          </CardHeader>
          <CardBody>
            {params.length === 0 ? (
              <div className="ct-empty-state">
                <p>No query parameters received.</p>
                <p className="ct-text-muted" style={{ marginTop: '0.5rem' }}>
                  When Azure redirects users here, parameters will appear in this table.
                </p>
              </div>
            ) : (
              <table className="ct-table">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                    <th>Description</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {params.map((param, index) => (
                    <tr key={index}>
                      <td>
                        <code style={{ 
                          backgroundColor: 'var(--ct-bg-tertiary)', 
                          padding: '0.25rem 0.5rem', 
                          borderRadius: '4px' 
                        }}>
                          {param.key}
                        </code>
                      </td>
                      <td>
                        <div style={{ 
                          maxWidth: '400px', 
                          overflow: 'hidden', 
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap'
                        }} title={param.value}>
                          {param.value}
                        </div>
                      </td>
                      <td className="ct-text-muted">
                        {getParamDescription(param.key) || '-'}
                      </td>
                      <td>
                        <button 
                          className="ct-button ct-button--small ct-button--ghost"
                          onClick={() => copyToClipboard(param.value)}
                        >
                          Copy
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </CardBody>
        </Card>

        {/* Token Instructions */}
        {searchParams.has('token') && (
          <Card>
            <CardHeader>
              <h3>Next Steps: Resolve Token</h3>
            </CardHeader>
            <CardBody>
              <p style={{ marginBottom: '1rem' }}>
                The <code>token</code> parameter needs to be resolved using the Azure Marketplace
                SaaS Fulfillment API to get subscription details:
              </p>
              <div className="ct-code-block" style={{ 
                backgroundColor: 'var(--ct-bg-tertiary)', 
                padding: '1rem', 
                borderRadius: '4px',
                fontFamily: 'monospace',
                fontSize: '0.875rem',
                marginBottom: '1rem'
              }}>
                <div style={{ color: 'var(--ct-text-muted)' }}>// POST request to Azure API</div>
                <div>POST https://marketplaceapi.microsoft.com/api/saas/subscriptions/resolve</div>
                <div>?api-version=2018-08-31</div>
                <div style={{ marginTop: '0.5rem' }}>x-ms-marketplace-token: {searchParams.get('token')?.substring(0, 50)}...</div>
              </div>
              <p className="ct-text-muted">
                This will return the full subscription details including purchaser info, plan, and subscription ID.
              </p>
            </CardBody>
          </Card>
        )}

        {/* Expected Parameters Reference */}
        <Card>
          <CardHeader>
            <h3>Expected Azure Parameters Reference</h3>
          </CardHeader>
          <CardBody>
            <table className="ct-table">
              <thead>
                <tr>
                  <th>Parameter</th>
                  <th>Description</th>
                  <th>Required</th>
                </tr>
              </thead>
              <tbody>
                {knownParams.map((param, index) => (
                  <tr key={index}>
                    <td><code>{param.key}</code></td>
                    <td>{param.description}</td>
                    <td>{param.key === 'token' ? 'Yes' : 'Optional'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </CardBody>
        </Card>

        {/* Path Info */}
        <Card>
          <CardHeader>
            <h3>Route Information</h3>
          </CardHeader>
          <CardBody>
            <table className="ct-table">
              <tbody>
                <tr>
                  <td><strong>Pathname</strong></td>
                  <td><code>{location.pathname}</code></td>
                </tr>
                <tr>
                  <td><strong>Search</strong></td>
                  <td><code>{location.search || '(empty)'}</code></td>
                </tr>
                <tr>
                  <td><strong>Hash</strong></td>
                  <td><code>{location.hash || '(empty)'}</code></td>
                </tr>
                <tr>
                  <td><strong>Full URL</strong></td>
                  <td><code>{window.location.href}</code></td>
                </tr>
              </tbody>
            </table>
          </CardBody>
        </Card>
      </div>
    </Layout>
  );
}
