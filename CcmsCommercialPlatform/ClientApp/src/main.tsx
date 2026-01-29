import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { PublicClientApplication, EventType } from '@azure/msal-browser';
import type { AccountInfo } from '@azure/msal-browser';
import { MsalProvider } from '@azure/msal-react';
import App from './App.tsx'
import { fetchServerConfig, createMsalConfig, defaultMsalConfig } from './auth';

// Set document direction to LTR for English
document.documentElement.dir = 'ltr';
document.documentElement.lang = 'en';

/**
 * Initialize the application with MSAL
 * Configuration is fetched from the server's appsettings.json
 * Required for Azure Marketplace SaaS landing page SSO
 * https://learn.microsoft.com/en-us/partner-center/marketplace-offers/azure-ad-transactable-saas-landing-page
 */
async function initializeApp() {
  let msalConfig = defaultMsalConfig;
  
  // Try to fetch config from server
  try {
    const serverConfig = await fetchServerConfig();
    msalConfig = createMsalConfig(serverConfig);
    console.info('[App] Loaded configuration from server');
  } catch (error) {
    console.warn('[App] Failed to load server config, using defaults:', error);
  }

  // Create MSAL instance with the configuration
  const msalInstance = new PublicClientApplication(msalConfig);

  // Initialize MSAL
  await msalInstance.initialize();

  // Handle redirect promise for login/logout redirects
  try {
    const response = await msalInstance.handleRedirectPromise();
    if (response) {
      // Set the active account after redirect
      msalInstance.setActiveAccount(response.account);
    }
  } catch (error) {
    console.error('[MSAL] Redirect error:', error);
  }

  // Set active account on login success
  msalInstance.addEventCallback((event) => {
    if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
      const payload = event.payload as { account: AccountInfo };
      if (payload.account) {
        msalInstance.setActiveAccount(payload.account);
      }
    }
  });

  // Render the app
  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <MsalProvider instance={msalInstance}>
        <App />
      </MsalProvider>
    </StrictMode>,
  );
}

// Start the application
initializeApp().catch((error) => {
  console.error('[App] Failed to initialize:', error);
  // Render an error message if initialization fails
  createRoot(document.getElementById('root')!).render(
    <div style={{ padding: '20px', textAlign: 'center' }}>
      <h1>Application Error</h1>
      <p>Failed to initialize the application. Please refresh the page or contact support.</p>
    </div>
  );
});
