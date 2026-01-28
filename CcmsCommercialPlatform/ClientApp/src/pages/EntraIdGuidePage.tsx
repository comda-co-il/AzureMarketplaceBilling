import { Link } from 'react-router-dom';
import { Layout } from '../components/Layout';

/**
 * Entra ID Configuration Guide Page
 * 
 * CCMS Installation Tutorial on Azure: Step 1 - App Registration in Entra ID
 * This page provides step-by-step instructions for configuring
 * Microsoft Entra ID (formerly Azure AD) for CCMS integration.
 */
export function EntraIdGuidePage() {
  return (
    <Layout>
      <div className="ct-entra-guide">
        <div className="ct-entra-guide__header">
          <Link to="/azure-landing" className="ct-entra-guide__back-link">
            ← Back to Subscription Setup
          </Link>
          <h1>CCMS Installation Tutorial on Azure</h1>
          <p className="ct-entra-guide__subtitle">
            Step 1 - App Registration in Microsoft Entra ID
          </p>
        </div>

        <div className="ct-entra-guide__content">
          {/* Objective Section */}
          <section className="ct-entra-guide__section">
            <h2>Objective</h2>
            <p>
              Create Azure App Registrations to enable secure identity and access management 
              for the Comsign CCMS system via Microsoft Entra ID (Azure AD).
            </p>
            <div className="ct-entra-guide__info-box">
              <div className="ct-entra-guide__info-icon">ℹ️</div>
              <div>
                <strong>What you'll need:</strong>
                <ul>
                  <li>Azure portal access with permissions to create App Registrations</li>
                  <li>Administrator or Application Administrator role in your organization</li>
                  <li>Your CCMS domain URL (e.g., https://your-ccms-domain)</li>
                </ul>
              </div>
            </div>
          </section>

          {/* Step 1: Log in to Azure Portal */}
          <section className="ct-entra-guide__section">
            <div className="ct-entra-guide__step-header">
              <span className="ct-entra-guide__step-number">1</span>
              <h2>Log in to Azure Portal</h2>
            </div>
            <ol className="ct-entra-guide__steps">
              <li>
                Open{' '}
                <a 
                  href="https://portal.azure.com" 
                  target="_blank" 
                  rel="noopener noreferrer"
                  className="ct-entra-guide__link"
                >
                  https://portal.azure.com
                </a>
              </li>
              <li>In the left-hand menu, go to <strong>Microsoft Entra ID</strong></li>
              <li>Click on <strong>App registrations</strong></li>
            </ol>
            
            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image1.png" 
                alt="Azure Portal - App registrations in Azure services" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Click on "App registrations" in Azure services</p>
            </div>

            <ol className="ct-entra-guide__steps" start={4}>
              <li>Click <strong>+ New registration</strong></li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image2.png" 
                alt="App registrations page - New registration button" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Click "+ New registration" to create a new app</p>
            </div>
          </section>

          {/* Step 2: Register the CCMS Web App */}
          <section className="ct-entra-guide__section">
            <div className="ct-entra-guide__step-header">
              <span className="ct-entra-guide__step-number">2</span>
              <h2>Register the CCMS Web App</h2>
            </div>
            <ol className="ct-entra-guide__steps">
              <li>
                <strong>Name:</strong> Enter a name for your application
                <div className="ct-entra-guide__code-block">
                  CCMS-App
                </div>
              </li>
              <li>
                <strong>Supported account types:</strong> Choose
                <div className="ct-entra-guide__option-highlight">
                  Accounts in this organizational directory only (Single tenant)
                </div>
              </li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image3.png" 
                alt="Register an application - Name and account types" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Enter the app name and select "Accounts in this organizational directory only"</p>
            </div>

            <ol className="ct-entra-guide__steps" start={3}>
              <li>
                <strong>Redirect URI:</strong>
                <ul>
                  <li>Type: <strong>Web</strong></li>
                  <li>URI: <code>https://&lt;your-ccms-domain&gt;/login/callback</code></li>
                </ul>
                <div className="ct-entra-guide__tip">
                  <strong>Note:</strong> Replace <code>&lt;your-ccms-domain&gt;</code> with your actual CCMS URL.
                </div>
              </li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image4.png" 
                alt="Redirect URI configuration" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Configure the Redirect URI with your CCMS domain</p>
            </div>

            <ol className="ct-entra-guide__steps" start={4}>
              <li>Click <strong>Register</strong></li>
            </ol>
          </section>

          {/* Step 3: Generate Client Secret */}
          <section className="ct-entra-guide__section">
            <div className="ct-entra-guide__step-header">
              <span className="ct-entra-guide__step-number">3</span>
              <h2>Generate Client Secret</h2>
            </div>
            <ol className="ct-entra-guide__steps">
              <li>After registration, go to <strong>Certificates & secrets</strong> in the left sidebar</li>
              <li>Click <strong>+ New client secret</strong></li>
              <li>Add a description (e.g., <code>CCMS Web Secret</code>) and select an expiration period</li>
              <li>Click <strong>Add</strong></li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image5.png" 
                alt="Certificates & secrets - New client secret" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Go to "Certificates & secrets" and click "+ New client secret"</p>
            </div>

            <div className="ct-entra-guide__warning-box">
              <div className="ct-entra-guide__warning-icon">⚠️</div>
              <div>
                <strong>Important:</strong> Save the <strong>Value</strong> immediately – it will only be shown once! 
                You'll need this value for your CCMS app settings. Make sure to copy the <strong>Value</strong> column, 
                not the "Secret ID".
              </div>
            </div>

            <div className="ct-entra-guide__values-table">
              <div className="ct-entra-guide__value-row ct-entra-guide__value-row--highlight">
                <div className="ct-entra-guide__value-label">
                  <strong>Client Secret Value</strong>
                  <span className="ct-entra-guide__required-badge">Required</span>
                </div>
                <div className="ct-entra-guide__value-desc">
                  Copy and save this value securely - you won't be able to see it again after leaving this page.
                </div>
              </div>
            </div>
          </section>

          {/* Step 4: Configure API Permissions */}
          <section className="ct-entra-guide__section">
            <div className="ct-entra-guide__step-header">
              <span className="ct-entra-guide__step-number">4</span>
              <h2>Configure API Permissions</h2>
            </div>
            <ol className="ct-entra-guide__steps">
              <li>Go to <strong>API permissions</strong> in the left sidebar</li>
              <li>Click <strong>Add a permission</strong> → <strong>Microsoft Graph</strong></li>
              <li>Add the following permissions:</li>
            </ol>

            <div className="ct-entra-guide__permissions-table">
              <table>
                <thead>
                  <tr>
                    <th>Permission Name</th>
                    <th>Type</th>
                    <th>Description</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td><code>Group.Read.All</code></td>
                    <td>Application</td>
                    <td>Read all groups</td>
                  </tr>
                  <tr>
                    <td><code>GroupMember.Read.All</code></td>
                    <td>Application</td>
                    <td>Read all group memberships</td>
                  </tr>
                  <tr>
                    <td><code>User.Read.All</code></td>
                    <td>Application</td>
                    <td>Read all users' full profiles</td>
                  </tr>
                  <tr>
                    <td><code>User.Read</code></td>
                    <td>Delegated</td>
                    <td>Sign in and read user profile</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <ol className="ct-entra-guide__steps" start={4}>
              <li>Click <strong>Grant admin consent</strong> to approve the permissions for your organization</li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image6.png" 
                alt="API permissions configured" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">API permissions configured with admin consent granted</p>
            </div>

            <div className="ct-entra-guide__tip">
              <strong>Tip:</strong> After granting consent, you should see a green checkmark next to each permission 
              indicating "Granted for [your organization]".
            </div>
          </section>

          {/* Step 5: Add Optional Claim for Group Information */}
          <section className="ct-entra-guide__section">
            <div className="ct-entra-guide__step-header">
              <span className="ct-entra-guide__step-number">5</span>
              <h2>Add Optional Claim for Group Information</h2>
            </div>
            <p>
              To allow CCMS to receive group claims in the token, configure the application to include the groups claim.
            </p>
            <ol className="ct-entra-guide__steps">
              <li>In your App Registration, go to <strong>Token configuration</strong></li>
              <li>Click <strong>+ Add groups claim</strong></li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image7.png" 
                alt="Token configuration - Add groups claim" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Click "+ Add groups claim" in Token configuration</p>
            </div>

            <ol className="ct-entra-guide__steps" start={3}>
              <li>
                In the popup:
                <ul>
                  <li>Choose <strong>Security groups</strong> (default is fine)</li>
                  <li>Under <strong>Token types</strong>, check all relevant:
                    <ul>
                      <li>✅ ID</li>
                      <li>✅ Access</li>
                      <li>✅ SAML</li>
                    </ul>
                  </li>
                  <li>Leave the optional settings as Default</li>
                </ul>
              </li>
              <li>Click <strong>Add</strong></li>
            </ol>
          </section>

          {/* Step 6: Expose an API */}
          <section className="ct-entra-guide__section">
            <div className="ct-entra-guide__step-header">
              <span className="ct-entra-guide__step-number">6</span>
              <h2>Expose an API (Create Scope for Login)</h2>
            </div>
            <p>
              To allow other apps (like the CCMS client or backend) to request tokens to access your CCMS application, 
              you need to expose an API and define scopes.
            </p>
            <ol className="ct-entra-guide__steps">
              <li>In the App Registration, go to <strong>Expose an API</strong></li>
              <li>Set the <strong>Application ID URI</strong> if it's not already set (you'll see a default like <code>api://&lt;app-guid&gt;</code>)</li>
              <li>Click <strong>+ Add a scope</strong></li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image8.png" 
                alt="Expose an API - Add a scope" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Click "+ Add a scope" to create the login scope</p>
            </div>

            <ol className="ct-entra-guide__steps" start={4}>
              <li>
                In the <strong>Add a scope</strong> dialog, enter:
                <div className="ct-entra-guide__scope-config">
                  <div className="ct-entra-guide__config-item">
                    <span className="ct-entra-guide__config-label">Scope name:</span>
                    <code>login</code>
                  </div>
                  <div className="ct-entra-guide__config-item">
                    <span className="ct-entra-guide__config-label">Who can consent:</span>
                    <span>Admins and users</span>
                  </div>
                  <div className="ct-entra-guide__config-item">
                    <span className="ct-entra-guide__config-label">Admin consent display name:</span>
                    <span>Login</span>
                  </div>
                  <div className="ct-entra-guide__config-item">
                    <span className="ct-entra-guide__config-label">Admin consent description:</span>
                    <span>Allow client applications to authenticate and login to CCMS.</span>
                  </div>
                  <div className="ct-entra-guide__config-item">
                    <span className="ct-entra-guide__config-label">User consent display name:</span>
                    <span>Login</span>
                  </div>
                  <div className="ct-entra-guide__config-item">
                    <span className="ct-entra-guide__config-label">User consent description:</span>
                    <span>Login to CCMS system.</span>
                  </div>
                  <div className="ct-entra-guide__config-item">
                    <span className="ct-entra-guide__config-label">State:</span>
                    <span>Enabled</span>
                  </div>
                </div>
              </li>
              <li>Click <strong>Add scope</strong></li>
            </ol>
            <div className="ct-entra-guide__tip">
              Once added, it will appear under "Scopes defined by this API".
            </div>
          </section>

          {/* Step 7: Enable Redirect URI for Native Client */}
          <section className="ct-entra-guide__section">
            <div className="ct-entra-guide__step-header">
              <span className="ct-entra-guide__step-number">7</span>
              <h2>Enable Redirect URI for Native Client (Desktop/Mobile)</h2>
            </div>
            
            <h3 className="ct-entra-guide__subsection-title">A. Enable Native Client Redirect URI</h3>
            <p>
              This step is required to allow desktop or mobile CCMS clients to authenticate users through 
              Microsoft Entra ID using the MSAL authentication library.
            </p>
            <ol className="ct-entra-guide__steps">
              <li>In your App Registration, go to <strong>Authentication</strong></li>
              <li>Scroll to the section titled <strong>Mobile and desktop applications</strong></li>
              <li>
                Ensure the following URI is added and checked:
                <div className="ct-entra-guide__code-block">
                  https://login.microsoftonline.com/common/oauth2/nativeclient
                </div>
              </li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image9.png" 
                alt="Authentication - Mobile and desktop applications" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Enable the native client redirect URI for MSAL</p>
            </div>

            <div className="ct-entra-guide__info-box">
              <div className="ct-entra-guide__info-icon">✅</div>
              <div>
                This URI must be checked to allow token responses to desktop clients using MSAL.
              </div>
            </div>

            <h3 className="ct-entra-guide__subsection-title">B. Set Front-Channel Logout URL</h3>
            <p>
              To support single sign-out (SSO logout), configure a Front-channel logout URL:
            </p>
            <ol className="ct-entra-guide__steps">
              <li>Still in the <strong>Authentication</strong> section, scroll to <strong>Front-channel logout URL</strong></li>
              <li>
                Enter the logout endpoint used by your CCMS app:
                <div className="ct-entra-guide__code-block">
                  https://&lt;your-ccms-domain&gt;/logout
                </div>
              </li>
            </ol>

            <div className="ct-entra-guide__image-container">
              <img 
                src="/entra_id_tutorial/image10.png" 
                alt="Authentication - Front-channel logout URL" 
                className="ct-entra-guide__image"
              />
              <p className="ct-entra-guide__image-caption">Configure the Front-channel logout URL</p>
            </div>

            <div className="ct-entra-guide__info-box">
              <div className="ct-entra-guide__info-icon">ℹ️</div>
              <div>
                <strong>Requirements for this URL:</strong>
                <ul>
                  <li>Must match your CCMS application base address</li>
                  <li>Must accept GET requests to terminate sessions</li>
                </ul>
                This ensures that when a user signs out of Azure AD, the CCMS app will also be notified 
                to clear the user's session locally.
              </div>
            </div>
          </section>

          {/* Summary Section */}
          <section className="ct-entra-guide__section ct-entra-guide__section--summary">
            <h2>Summary: Values Needed for CCMS Setup</h2>
            <p>After completing the steps above, you should have the following values from the <strong>Overview</strong> page:</p>
            
            <div className="ct-entra-guide__summary-table">
              <div className="ct-entra-guide__summary-row">
                <div className="ct-entra-guide__summary-field">Application (Client) ID</div>
                <div className="ct-entra-guide__summary-location">App Registration → Overview</div>
              </div>
              <div className="ct-entra-guide__summary-row">
                <div className="ct-entra-guide__summary-field">Directory (Tenant) ID</div>
                <div className="ct-entra-guide__summary-location">App Registration → Overview</div>
              </div>
              <div className="ct-entra-guide__summary-row">
                <div className="ct-entra-guide__summary-field">Client Secret Value</div>
                <div className="ct-entra-guide__summary-location">App Registration → Certificates & secrets</div>
              </div>
            </div>

            <div className="ct-entra-guide__cta">
              <Link to="/azure-landing" className="ct-entra-guide__cta-button">
                ← Return to Subscription Setup
              </Link>
            </div>
          </section>

          {/* Troubleshooting Section */}
          <section className="ct-entra-guide__section">
            <h2>Troubleshooting</h2>
            
            <div className="ct-entra-guide__faq">
              <div className="ct-entra-guide__faq-item">
                <h3>I don't have permission to create App Registrations</h3>
                <p>
                  Contact your Azure administrator or IT department. They may need to grant you the 
                  "Application Developer" role or create the app registration on your behalf.
                </p>
              </div>

              <div className="ct-entra-guide__faq-item">
                <h3>I lost my client secret value</h3>
                <p>
                  Client secret values can only be viewed once when created. You'll need to create 
                  a new client secret and update your CCMS configuration with the new value.
                </p>
              </div>

              <div className="ct-entra-guide__faq-item">
                <h3>My client secret expired</h3>
                <p>
                  Create a new client secret following Step 3 above, then update your CCMS 
                  configuration with the new secret value.
                </p>
              </div>

              <div className="ct-entra-guide__faq-item">
                <h3>Admin consent is not available</h3>
                <p>
                  You may need Global Administrator or Privileged Role Administrator permissions 
                  to grant admin consent. Contact your Azure AD administrator.
                </p>
              </div>
            </div>
          </section>

          {/* Contact Section */}
          <section className="ct-entra-guide__section ct-entra-guide__contact">
            <h2>Need Help?</h2>
            <p>
              If you encounter any issues during the configuration process, our support team is here to help.
            </p>
            <div className="ct-entra-guide__contact-info">
              <p>
                <strong>Email:</strong>{' '}
                <a href="mailto:support@comsigntrust.com">support@comsigntrust.com</a>
              </p>
              <p>
                <strong>Phone:</strong> +972-3-6485255
              </p>
            </div>
          </section>
        </div>
      </div>
    </Layout>
  );
}
