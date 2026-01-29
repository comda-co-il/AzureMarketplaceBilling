/**
 * Microsoft Entra ID (Azure AD) Authentication Configuration
 * 
 * This configuration is for the SaaS landing page SSO as required by Azure Marketplace.
 * See: https://learn.microsoft.com/en-us/partner-center/marketplace-offers/azure-ad-transactable-saas-landing-page
 * 
 * Configuration is loaded from the server's appsettings.json via /api/config endpoint.
 */

import type { Configuration, PopupRequest, RedirectRequest } from '@azure/msal-browser';

/**
 * Server configuration response type
 */
export interface ServerConfig {
  entraId: {
    clientId: string;
    authority: string;
    redirectUri: string;
    postLogoutRedirectUri: string;
  };
  isDemo: boolean;
  applicationUrl: string;
}

/**
 * Fetch configuration from the server
 */
export async function fetchServerConfig(): Promise<ServerConfig> {
  const response = await fetch('/api/config');
  if (!response.ok) {
    throw new Error('Failed to fetch configuration from server');
  }
  return response.json();
}

/**
 * Create MSAL configuration from server config
 */
export function createMsalConfig(serverConfig: ServerConfig): Configuration {
  return {
    auth: {
      clientId: serverConfig.entraId.clientId,
      authority: serverConfig.entraId.authority,
      redirectUri: serverConfig.entraId.redirectUri,
      postLogoutRedirectUri: serverConfig.entraId.postLogoutRedirectUri,
    },
    cache: {
      cacheLocation: 'sessionStorage', // Use sessionStorage for better security
    },
    system: {
      loggerOptions: {
        logLevel: import.meta.env.DEV ? 3 : 0, // Verbose in dev, Error only in prod
        loggerCallback: (level, message, containsPii) => {
          if (containsPii) return; // Don't log PII
          
          switch (level) {
            case 0: // Error
              console.error('[MSAL]', message);
              break;
            case 1: // Warning
              console.warn('[MSAL]', message);
              break;
            case 2: // Info
              console.info('[MSAL]', message);
              break;
            case 3: // Verbose
              console.debug('[MSAL]', message);
              break;
          }
        },
        piiLoggingEnabled: false,
      },
    },
  };
}

/**
 * Default/fallback MSAL configuration (used during initial load or if server config fails)
 */
export const defaultMsalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_MSAL_CLIENT_ID || '5aa80cab-8812-4ea5-8052-2bf21e289e68',
    authority: 'https://login.microsoftonline.com/common',
    redirectUri: typeof window !== 'undefined' 
      ? `${window.location.origin}/azure-landing` 
      : 'http://localhost:5173/azure-landing',
    postLogoutRedirectUri: '/',
  },
  cache: {
    cacheLocation: 'sessionStorage',
  },
  system: {
    loggerOptions: {
      logLevel: import.meta.env.DEV ? 3 : 0,
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        switch (level) {
          case 0: console.error('[MSAL]', message); break;
          case 1: console.warn('[MSAL]', message); break;
          case 2: console.info('[MSAL]', message); break;
          case 3: console.debug('[MSAL]', message); break;
        }
      },
      piiLoggingEnabled: false,
    },
  },
};

// For backwards compatibility, export msalConfig as default config
export const msalConfig = defaultMsalConfig;

/**
 * Scopes for ID token claims
 * User.Read is the basic permission that gives us:
 * - displayName, givenName, surname
 * - mail, userPrincipalName
 * - jobTitle
 * - preferredLanguage
 * 
 * This does NOT require admin consent, so all users can sign in.
 */
export const loginRequest: PopupRequest | RedirectRequest = {
  scopes: ['User.Read', 'openid', 'profile', 'email'],
};

/**
 * Scopes for calling Microsoft Graph API
 */
export const graphConfig = {
  graphMeEndpoint: 'https://graph.microsoft.com/v1.0/me',
  graphMeOrgEndpoint: 'https://graph.microsoft.com/v1.0/me?$select=displayName,givenName,surname,mail,jobTitle,companyName,mobilePhone,country,preferredLanguage',
};

/**
 * Token request for Graph API calls
 */
export const tokenRequest = {
  scopes: ['User.Read'],
};
