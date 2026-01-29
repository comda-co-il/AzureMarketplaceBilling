/**
 * Custom hook for Microsoft Entra ID authentication on the Azure Marketplace landing page
 * 
 * This hook provides:
 * 1. SSO authentication state
 * 2. User information from ID token claims
 * 3. Microsoft Graph API calls for additional user details
 * 
 * Based on: https://learn.microsoft.com/en-us/partner-center/marketplace-offers/azure-ad-transactable-saas-landing-page
 */

import { useState, useEffect, useCallback } from 'react';
import { useMsal, useIsAuthenticated } from '@azure/msal-react';
import { InteractionRequiredAuthError, InteractionStatus } from '@azure/msal-browser';
import type { AccountInfo, AuthenticationResult } from '@azure/msal-browser';
import { loginRequest, graphConfig, tokenRequest } from './authConfig';

/**
 * User information extracted from ID token and Microsoft Graph
 */
export interface EntraUserInfo {
  // From ID token claims
  objectId: string;           // oid - unique user identifier
  tenantId: string;           // tid - Azure AD tenant
  preferredUsername: string;  // preferred_username - usually email
  name: string;               // name - display name
  email: string;              // email claim
  
  // From Microsoft Graph API (additional details)
  givenName?: string;
  surname?: string;
  displayName?: string;
  jobTitle?: string;
  companyName?: string;
  mobilePhone?: string;
  country?: string;
  preferredLanguage?: string;
}

interface UseEntraAuthReturn {
  /** Whether the user is authenticated */
  isAuthenticated: boolean;
  /** Whether authentication is in progress */
  isLoading: boolean;
  /** User information from ID token and Graph API */
  userInfo: EntraUserInfo | null;
  /** Error message if authentication failed */
  error: string | null;
  /** The current account (raw MSAL account) */
  account: AccountInfo | null;
  /** Trigger login (usually automatic, but can be called manually) */
  login: () => Promise<void>;
  /** Logout the user */
  logout: () => Promise<void>;
  /** Refresh user info from Graph API */
  refreshUserInfo: () => Promise<void>;
}

/**
 * Extract user information from MSAL account (ID token claims)
 */
function extractUserInfoFromAccount(account: AccountInfo): Partial<EntraUserInfo> {
  const idTokenClaims = account.idTokenClaims || {};
  
  return {
    objectId: (idTokenClaims.oid as string) || account.localAccountId,
    tenantId: (idTokenClaims.tid as string) || account.tenantId,
    preferredUsername: (idTokenClaims.preferred_username as string) || account.username,
    name: (idTokenClaims.name as string) || '',
    email: (idTokenClaims.email as string) || account.username,
    givenName: idTokenClaims.given_name as string,
    surname: idTokenClaims.family_name as string,
  };
}

/**
 * Fetch additional user details from Microsoft Graph API
 */
async function fetchGraphUserInfo(accessToken: string): Promise<Partial<EntraUserInfo>> {
  try {
    const response = await fetch(graphConfig.graphMeOrgEndpoint, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    if (!response.ok) {
      console.warn('[EntraAuth] Graph API returned non-OK status:', response.status);
      return {};
    }

    const data = await response.json();
    
    return {
      displayName: data.displayName,
      givenName: data.givenName,
      surname: data.surname,
      email: data.mail || data.userPrincipalName,
      jobTitle: data.jobTitle,
      companyName: data.companyName,
      mobilePhone: data.mobilePhone,
      country: data.country,
      preferredLanguage: data.preferredLanguage,
    };
  } catch (err) {
    console.warn('[EntraAuth] Failed to fetch Graph user info:', err);
    return {};
  }
}

/**
 * Custom hook for Entra ID authentication
 */
export function useEntraAuth(): UseEntraAuthReturn {
  const { instance, accounts, inProgress } = useMsal();
  const isAuthenticated = useIsAuthenticated();
  
  const [userInfo, setUserInfo] = useState<EntraUserInfo | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoadingUserInfo, setIsLoadingUserInfo] = useState(false);

  const account = accounts[0] || null;

  /**
   * Login using redirect (better UX for landing pages)
   */
  const login = useCallback(async () => {
    try {
      setError(null);
      // Use redirect for better UX (popup can be blocked)
      await instance.loginRedirect(loginRequest);
    } catch (err) {
      console.error('[EntraAuth] Login failed:', err);
      setError('Failed to sign in. Please try again.');
    }
  }, [instance]);

  /**
   * Logout
   */
  const logout = useCallback(async () => {
    try {
      await instance.logoutRedirect({
        postLogoutRedirectUri: '/',
      });
    } catch (err) {
      console.error('[EntraAuth] Logout failed:', err);
    }
  }, [instance]);

  /**
   * Acquire token and fetch user info from Graph API
   */
  const refreshUserInfo = useCallback(async () => {
    if (!account) return;

    setIsLoadingUserInfo(true);
    setError(null);

    try {
      // First, extract info from ID token
      const tokenClaimsInfo = extractUserInfoFromAccount(account);

      // Try to get access token for Graph API
      let graphInfo: Partial<EntraUserInfo> = {};
      
      try {
        const tokenResponse: AuthenticationResult = await instance.acquireTokenSilent({
          ...tokenRequest,
          account,
        });
        
        if (tokenResponse.accessToken) {
          graphInfo = await fetchGraphUserInfo(tokenResponse.accessToken);
        }
      } catch (tokenErr) {
        if (tokenErr instanceof InteractionRequiredAuthError) {
          // Need to login again to get consent
          console.info('[EntraAuth] Token acquisition requires interaction');
          // For MSA accounts (personal Microsoft accounts), Graph API may not return company info
          // This is expected behavior per Microsoft docs
        } else {
          console.warn('[EntraAuth] Silent token acquisition failed:', tokenErr);
        }
      }

      // Merge token claims and Graph info (Graph takes precedence)
      const mergedInfo: EntraUserInfo = {
        objectId: tokenClaimsInfo.objectId || '',
        tenantId: tokenClaimsInfo.tenantId || '',
        preferredUsername: tokenClaimsInfo.preferredUsername || '',
        name: graphInfo.displayName || tokenClaimsInfo.name || '',
        email: graphInfo.email || tokenClaimsInfo.email || tokenClaimsInfo.preferredUsername || '',
        givenName: graphInfo.givenName || tokenClaimsInfo.givenName,
        surname: graphInfo.surname || tokenClaimsInfo.surname,
        displayName: graphInfo.displayName || tokenClaimsInfo.name,
        jobTitle: graphInfo.jobTitle,
        companyName: graphInfo.companyName,
        mobilePhone: graphInfo.mobilePhone,
        country: graphInfo.country,
        preferredLanguage: graphInfo.preferredLanguage,
      };

      setUserInfo(mergedInfo);
    } catch (err) {
      console.error('[EntraAuth] Failed to refresh user info:', err);
      setError('Failed to load user information.');
    } finally {
      setIsLoadingUserInfo(false);
    }
  }, [account, instance]);

  /**
   * Auto-fetch user info when authenticated
   */
  useEffect(() => {
    if (isAuthenticated && account && !userInfo && inProgress === InteractionStatus.None) {
      refreshUserInfo();
    }
  }, [isAuthenticated, account, userInfo, inProgress, refreshUserInfo]);

  /**
   * Auto-login if not authenticated (for landing page)
   * This triggers the SSO flow automatically when users arrive from Azure Marketplace
   */
  useEffect(() => {
    if (!isAuthenticated && inProgress === InteractionStatus.None && accounts.length === 0) {
      // Small delay to allow MSAL to initialize
      const timer = setTimeout(() => {
        // Don't auto-login - let the landing page component decide when to trigger login
        // This allows showing a "Sign in to continue" message first
      }, 100);
      return () => clearTimeout(timer);
    }
  }, [isAuthenticated, inProgress, accounts.length]);

  const isLoading = inProgress !== InteractionStatus.None || isLoadingUserInfo;

  return {
    isAuthenticated,
    isLoading,
    userInfo,
    error,
    account,
    login,
    logout,
    refreshUserInfo,
  };
}
