import { useEffect, useState } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardBody, Button, CountryAutocomplete } from '../components/Common';
import { marketplaceApi } from '../services/api';
import { useEntraAuth } from '../auth';
import type {
  ResolvedSubscriptionInfo,
  MarketplaceSubscriptionResponse,
  AvailableFeature,
  FeatureSelectionItem,
} from '../types';

type Step = 'sign-in' | 'resolving' | 'customer-info' | 'feature-selection' | 'thank-you';

interface FeatureState extends AvailableFeature {
  isEnabled: boolean;
  quantity: number;
}


export function AzureLandingPage() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get('token');

  // Microsoft Entra ID SSO authentication
  // Required by Azure Marketplace: https://learn.microsoft.com/en-us/partner-center/marketplace-offers/azure-ad-transactable-saas-landing-page
  const { 
    isAuthenticated, 
    isLoading: isAuthLoading, 
    userInfo, 
    error: authError, 
    login,
  } = useEntraAuth();

  // State - starts at sign-in if not authenticated
  const [currentStep, setCurrentStep] = useState<Step>('sign-in');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isFinalized, setIsFinalized] = useState(false); // Prevent multiple finalizations
  const [hasPrefilledFromSSO, setHasPrefilledFromSSO] = useState(false);

  // Data state
  const [resolvedInfo, setResolvedInfo] = useState<ResolvedSubscriptionInfo | null>(null);
  const [subscription, setSubscription] = useState<MarketplaceSubscriptionResponse | null>(null);
  const [availableFeatures, setAvailableFeatures] = useState<FeatureState[]>([]);

  // Form state - Customer Info
  const [customerName, setCustomerName] = useState('');
  const [customerEmail, setCustomerEmail] = useState('');
  const [companyName, setCompanyName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [jobTitle, setJobTitle] = useState('');
  const [countryCode, setCountryCode] = useState<string>('');
  const [countryOther, setCountryOther] = useState<string>('');
  const [countryError, setCountryError] = useState<string>('');
  const [countryOtherError, setCountryOtherError] = useState<string>('');
  const [comments, setComments] = useState('');

  // Form validation errors
  const [formErrors, setFormErrors] = useState<{
    customerName?: string;
    customerEmail?: string;
    companyName?: string;
    phoneNumber?: string;
  }>({});

  // Entra ID (Azure AD) Configuration - Customer's app registration for SaaS integration
  const [entraClientId, setEntraClientId] = useState('');
  const [entraClientSecret, setEntraClientSecret] = useState('');
  const [entraTenantId, setEntraTenantId] = useState('');
  const [entraErrors, setEntraErrors] = useState<{ clientId?: string; clientSecret?: string; tenantId?: string }>({});

  // Step 0: Handle authentication state
  // When user is authenticated, move to resolving step
  useEffect(() => {
    if (isAuthenticated && currentStep === 'sign-in') {
      setCurrentStep('resolving');
    }
  }, [isAuthenticated, currentStep]);

  // Step 1: Resolve token when authenticated
  useEffect(() => {
    // Only resolve token when authenticated and on resolving step
    if (!isAuthenticated || currentStep !== 'resolving') {
      return;
    }

    if (!token) {
      setError('No token provided. Please access this page from Azure Marketplace.');
      return;
    }

    const resolveToken = async () => {
      try {
        setIsLoading(true);
        const result = await marketplaceApi.resolveToken({ token });
        setResolvedInfo(result);

        // Load available features
        const features = await marketplaceApi.getAvailableFeatures();
        setAvailableFeatures(
          features.map((f) => ({
            ...f,
            isEnabled: false,
            quantity: f.minQuantity,
          }))
        );

        setCurrentStep('customer-info');
      } catch (err) {
        console.error('Failed to resolve token:', err);
        setError('Failed to resolve marketplace token. Please try again or contact support.');
      } finally {
        setIsLoading(false);
      }
    };

    resolveToken();
  }, [isAuthenticated, currentStep, token]);

  // Pre-fill form with SSO user information from Microsoft Entra ID and Graph API
  useEffect(() => {
    if (userInfo && !hasPrefilledFromSSO && currentStep === 'customer-info') {
      // Pre-fill from SSO (Graph API takes precedence over ID token claims)
      if (userInfo.displayName || userInfo.name) {
        setCustomerName(userInfo.displayName || userInfo.name);
      } else if (userInfo.givenName && userInfo.surname) {
        setCustomerName(`${userInfo.givenName} ${userInfo.surname}`);
      }
      
      if (userInfo.email) {
        setCustomerEmail(userInfo.email);
      }
      
      if (userInfo.companyName) {
        setCompanyName(userInfo.companyName);
      }
      
      if (userInfo.mobilePhone) {
        setPhoneNumber(userInfo.mobilePhone);
      }
      
      if (userInfo.jobTitle) {
        setJobTitle(userInfo.jobTitle);
      }

      // Try to set country from Graph API
      if (userInfo.country) {
        // Map common country names to ISO codes (basic mapping)
        const countryMap: Record<string, string> = {
          'United States': 'US',
          'USA': 'US',
          'Israel': 'IL',
          'United Kingdom': 'GB',
          'UK': 'GB',
          'Germany': 'DE',
          'France': 'FR',
          'Canada': 'CA',
          'Australia': 'AU',
          // Add more as needed
        };
        const mappedCode = countryMap[userInfo.country];
        if (mappedCode) {
          setCountryCode(mappedCode);
        } else {
          setCountryCode('XX'); // Other
          setCountryOther(userInfo.country);
        }
      }

      setHasPrefilledFromSSO(true);
    }
  }, [userInfo, hasPrefilledFromSSO, currentStep]);

  // Step 2: Submit customer info
  const handleSubmitCustomerInfo = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!resolvedInfo) return;

    // If no country is selected, default to "Other"
    const finalCountryCode = countryCode || 'XX';
    if (!countryCode) {
      setCountryCode('XX');
    }

    // Validate all form fields
    let isValid = true;
    const newFormErrors: typeof formErrors = {};
    setCountryError('');

    // Validate customer name
    if (!customerName.trim()) {
      newFormErrors.customerName = 'Full name is required';
      isValid = false;
    } else if (customerName.trim().length < 2) {
      newFormErrors.customerName = 'Full name must be at least 2 characters';
      isValid = false;
    }

    // Validate email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!customerEmail.trim()) {
      newFormErrors.customerEmail = 'Email address is required';
      isValid = false;
    } else if (!emailRegex.test(customerEmail.trim())) {
      newFormErrors.customerEmail = 'Please enter a valid email address';
      isValid = false;
    }

    // Validate company name
    if (!companyName.trim()) {
      newFormErrors.companyName = 'Company name is required';
      isValid = false;
    } else if (companyName.trim().length < 2) {
      newFormErrors.companyName = 'Company name must be at least 2 characters';
      isValid = false;
    }

    // Validate phone number (optional, but if provided must be valid format)
    if (phoneNumber.trim()) {
      // More permissive regex that accepts common international phone formats
      // Allows: +, digits, spaces, dashes, dots, parentheses anywhere
      const phoneRegex = /^[+]?[\d\s\-().]{7,20}$/;
      if (!phoneRegex.test(phoneNumber.trim())) {
        newFormErrors.phoneNumber = 'Please enter a valid phone number (7-20 characters)';
        isValid = false;
      }
    }

    setFormErrors(newFormErrors);

    // If "Other" is selected, validate that countryOther is filled
    if (finalCountryCode === 'XX' && !countryOther.trim()) {
      setCountryOtherError('Please specify your country');
      isValid = false;
    } else {
      setCountryOtherError('');
    }

    // Validate Entra ID fields (customer's app registration for SaaS integration)
    const newEntraErrors: { clientId?: string; clientSecret?: string; tenantId?: string } = {};
    
    if (!entraClientId.trim()) {
      newEntraErrors.clientId = 'Application (Client) ID is required';
      isValid = false;
    } else if (!/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(entraClientId.trim())) {
      newEntraErrors.clientId = 'Please enter a valid GUID format (e.g., 12345678-1234-1234-1234-123456789abc)';
      isValid = false;
    }
    
    if (!entraClientSecret.trim()) {
      newEntraErrors.clientSecret = 'Client Secret is required';
      isValid = false;
    } else if (entraClientSecret.trim().length < 10) {
      newEntraErrors.clientSecret = 'Client Secret appears too short. Please verify you copied the full value.';
      isValid = false;
    }
    
    // Use entered tenant ID or fall back to SSO tenant ID
    const finalTenantId = entraTenantId.trim() || userInfo?.tenantId || '';
    if (!finalTenantId) {
      newEntraErrors.tenantId = 'Directory (Tenant) ID is required';
      isValid = false;
    } else if (!/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(finalTenantId)) {
      newEntraErrors.tenantId = 'Please enter a valid GUID format (e.g., 12345678-1234-1234-1234-123456789abc)';
      isValid = false;
    }
    
    setEntraErrors(newEntraErrors);

    if (!isValid) {
      return;
    }

    try {
      setIsLoading(true);
      setError(null);

      const result = await marketplaceApi.submitCustomerInfo({
        marketplaceSubscriptionId: resolvedInfo.marketplaceSubscriptionId,
        customerName,
        customerEmail,
        companyName,
        phoneNumber,
        jobTitle,
        countryCode: finalCountryCode,
        countryOther: finalCountryCode === 'XX' ? countryOther : undefined,
        comments,
        // Customer's Entra ID app registration for SaaS integration
        entraClientId: entraClientId.trim(),
        entraClientSecret: entraClientSecret.trim(),
        entraTenantId: finalTenantId,
      });

      setSubscription(result);
      setCurrentStep('feature-selection');
    } catch (err) {
      console.error('Failed to submit customer info:', err);
      setError('Failed to submit customer information. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  // Toggle feature
  const handleToggleFeature = (featureId: string) => {
    setAvailableFeatures((prev) =>
      prev.map((f) =>
        f.featureId === featureId ? { ...f, isEnabled: !f.isEnabled } : f
      )
    );
  };

  // Update feature quantity
  const handleQuantityChange = (featureId: string, quantity: number) => {
    setAvailableFeatures((prev) =>
      prev.map((f) =>
        f.featureId === featureId
          ? { ...f, quantity: Math.max(f.minQuantity, Math.min(f.maxQuantity, quantity)) }
          : f
      )
    );
  };

  // Step 3: Submit feature selection
  const handleSubmitFeatures = async () => {
    if (!resolvedInfo) return;

    try {
      setIsLoading(true);
      setError(null);

      const features: FeatureSelectionItem[] = availableFeatures
        .filter((f) => f.isEnabled)
        .map((f) => ({
          featureId: f.featureId,
          featureName: f.featureName,
          isEnabled: true,
          quantity: f.quantity,
          pricePerUnit: f.pricePerUnit,
        }));

      const result = await marketplaceApi.submitFeatureSelection({
        marketplaceSubscriptionId: resolvedInfo.marketplaceSubscriptionId,
        features,
      });

      setSubscription(result);
    } catch (err) {
      console.error('Failed to submit feature selection:', err);
      setError('Failed to submit feature selection. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  // Step 4: Finalize subscription
  const handleFinalize = async () => {
    // Prevent multiple submissions
    if (!resolvedInfo || isFinalized || isLoading) return;

    // Set flag immediately to prevent double-clicks
    setIsFinalized(true);

    try {
      setIsLoading(true);
      setError(null);

      // First submit features if any are selected
      await handleSubmitFeatures();

      // Then finalize
      const result = await marketplaceApi.finalizeSubscription({
        marketplaceSubscriptionId: resolvedInfo.marketplaceSubscriptionId,
      });

      setSubscription(result);
      setCurrentStep('thank-you');
    } catch (err) {
      console.error('Failed to finalize subscription:', err);
      setError('Failed to finalize subscription. Please try again.');
      // Reset the flag on error so user can retry
      setIsFinalized(false);
    } finally {
      setIsLoading(false);
    }
  };

  // Calculate total for enabled features
  const calculateTotal = () => {
    return availableFeatures
      .filter((f) => f.isEnabled)
      .reduce((sum, f) => sum + f.quantity * f.pricePerUnit, 0);
  };

  // Get step number for display
  const getStepNumber = () => {
    switch (currentStep) {
      case 'sign-in':
        return 1;
      case 'resolving':
        return 2;
      case 'customer-info':
        return 3;
      case 'feature-selection':
        return 4;
      case 'thank-you':
        return 5;
      default:
        return 1;
    }
  };

  // Render step indicator
  const renderStepIndicator = () => {
    const steps = [
      { num: 1, label: 'Sign In' },
      { num: 2, label: 'Verification' },
      { num: 3, label: 'Your Details' },
      { num: 4, label: 'Choose Tokens' },
      { num: 5, label: 'Complete' },
    ];

    const currentNum = getStepNumber();

    return (
      <div className="ct-wizard-steps">
        {steps.map((step, index) => (
          <div
            key={step.num}
            className={`ct-wizard-step ${step.num < currentNum ? 'ct-wizard-step--completed' : ''} ${step.num === currentNum ? 'ct-wizard-step--active' : ''}`}
          >
            <div className="ct-wizard-step__number">
              {step.num < currentNum ? '✓' : step.num}
            </div>
            <div className="ct-wizard-step__label">{step.label}</div>
            {index < steps.length - 1 && <div className="ct-wizard-step__line" />}
          </div>
        ))}
      </div>
    );
  };

  // Render sign-in step (SSO required by Azure Marketplace)
  const renderSignInStep = () => (
    <Card>
      <CardBody>
        <div className="ct-sign-in">
          <div className="ct-sign-in__icon">🔐</div>
          <h2>Sign in to Continue</h2>
          <p className="ct-sign-in__message">
            To complete your Azure Marketplace subscription setup, please sign in with your 
            Microsoft account. This verifies your identity and allows us to securely configure 
            your subscription.
          </p>
          
          {authError && (
            <div className="ct-sign-in__error">
              <span>⚠️</span> {authError}
            </div>
          )}

          {!token && (
            <div className="ct-sign-in__warning">
              <span>⚠️</span>
              <p>No marketplace token detected. Please access this page from Azure Marketplace.</p>
            </div>
          )}

          <div className="ct-sign-in__actions">
            <Button 
              variant="primary" 
              size="large" 
              onClick={login}
              disabled={isAuthLoading || !token}
            >
              {isAuthLoading ? (
                <>
                  <span className="ct-spinner ct-spinner--small"></span>
                  Signing in...
                </>
              ) : (
                <>
                  <span className="ct-sign-in__ms-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="21" height="21" viewBox="0 0 21 21">
                      <rect x="1" y="1" width="9" height="9" fill="#f25022"/>
                      <rect x="11" y="1" width="9" height="9" fill="#7fba00"/>
                      <rect x="1" y="11" width="9" height="9" fill="#00a4ef"/>
                      <rect x="11" y="11" width="9" height="9" fill="#ffb900"/>
                    </svg>
                  </span>
                  Sign in with Microsoft
                </>
              )}
            </Button>
          </div>

          <p className="ct-sign-in__note">
            By signing in, you agree to our{' '}
            <Link to="/privacy-policy" target="_blank">Privacy Policy</Link>.
          </p>
        </div>
      </CardBody>
    </Card>
  );

  // Render resolving step
  const renderResolvingStep = () => (
    <Card>
      <CardBody>
        <div className="ct-resolving">
          <div className="ct-spinner ct-spinner--large"></div>
          <h2>Verifying your subscription...</h2>
          <p>Please wait while we verify your Azure Marketplace purchase.</p>
        </div>
      </CardBody>
    </Card>
  );

  // Render customer info form
  const renderCustomerInfoStep = () => (
    <Card>
      <CardHeader>
        <h2>Tell us about yourself</h2>
      </CardHeader>
      <CardBody>
        {/* SSO User Info Banner */}
        {userInfo && (
          <div className="ct-sso-info">
            <div className="ct-sso-info__header">
              <span className="ct-sso-info__icon">✓</span>
              <span>Signed in as <strong>{userInfo.email || userInfo.preferredUsername}</strong></span>
            </div>
            <p className="ct-sso-info__note">
              Your information has been pre-filled from your Microsoft account. Please review and update if needed.
            </p>
          </div>
        )}

        {resolvedInfo && (
          <div className="ct-resolved-info">
            <div className="ct-resolved-info__item">
              <span className="ct-resolved-info__label">Subscription:</span>
              <span className="ct-resolved-info__value">{resolvedInfo.subscriptionName}</span>
            </div>
            <div className="ct-resolved-info__item">
              <span className="ct-resolved-info__label">Plan:</span>
              <span className="ct-resolved-info__value">{resolvedInfo.planId}</span>
            </div>
            {userInfo?.tenantId && (
              <div className="ct-resolved-info__item">
                <span className="ct-resolved-info__label">Tenant ID:</span>
                <span className="ct-resolved-info__value ct-resolved-info__value--mono">{userInfo.tenantId}</span>
              </div>
            )}
          </div>
        )}

        <form onSubmit={handleSubmitCustomerInfo} className="ct-customer-form">
          <div className="ct-form-row">
            <div className="ct-input-group">
              <label className="ct-label--primary">
                Full Name <span className="ct-required">*</span>
              </label>
              <input
                type="text"
                className={`ct-input--primary ${formErrors.customerName ? 'ct-input--error' : ''}`}
                value={customerName}
                onChange={(e) => {
                  setCustomerName(e.target.value);
                  setFormErrors((prev) => ({ ...prev, customerName: undefined }));
                }}
                placeholder="John Smith"
              />
              {formErrors.customerName && <div className="ct-input-error">{formErrors.customerName}</div>}
            </div>
            <div className="ct-input-group">
              <label className="ct-label--primary">
                Email Address <span className="ct-required">*</span>
              </label>
              <input
                type="email"
                className={`ct-input--primary ${formErrors.customerEmail ? 'ct-input--error' : ''}`}
                value={customerEmail}
                onChange={(e) => {
                  setCustomerEmail(e.target.value);
                  setFormErrors((prev) => ({ ...prev, customerEmail: undefined }));
                }}
                placeholder="john@company.com"
              />
              {formErrors.customerEmail && <div className="ct-input-error">{formErrors.customerEmail}</div>}
            </div>
          </div>

          <div className="ct-form-row">
            <div className="ct-input-group">
              <label className="ct-label--primary">
                Company Name <span className="ct-required">*</span>
              </label>
              <input
                type="text"
                className={`ct-input--primary ${formErrors.companyName ? 'ct-input--error' : ''}`}
                value={companyName}
                onChange={(e) => {
                  setCompanyName(e.target.value);
                  setFormErrors((prev) => ({ ...prev, companyName: undefined }));
                }}
                placeholder="Acme Corporation"
              />
              {formErrors.companyName && <div className="ct-input-error">{formErrors.companyName}</div>}
            </div>
            <div className="ct-input-group">
              <label className="ct-label--primary">Phone Number</label>
              <input
                type="tel"
                className={`ct-input--primary ${formErrors.phoneNumber ? 'ct-input--error' : ''}`}
                value={phoneNumber}
                onChange={(e) => {
                  setPhoneNumber(e.target.value);
                  setFormErrors((prev) => ({ ...prev, phoneNumber: undefined }));
                }}
                placeholder="+1 (555) 123-4567"
              />
              {formErrors.phoneNumber && <div className="ct-input-error">{formErrors.phoneNumber}</div>}
            </div>
          </div>

          <div className="ct-form-row">
            <div className="ct-input-group">
              <label className="ct-label--primary">Job Title</label>
              <input
                type="text"
                className="ct-input--primary"
                value={jobTitle}
                onChange={(e) => setJobTitle(e.target.value)}
                placeholder="IT Manager"
              />
            </div>
            <div className="ct-input-group">
              <label className="ct-label--primary">
                Country <span className="ct-required">*</span>
              </label>
              <CountryAutocomplete
                countryCode={countryCode}
                countryOther={countryOther}
                onCountryCodeChange={setCountryCode}
                onCountryOtherChange={setCountryOther}
                countryError={countryError}
                countryOtherError={countryOtherError}
                onErrorClear={() => {
                  setCountryError('');
                  setCountryOtherError('');
                }}
              />
            </div>
          </div>

          {/* Entra ID (Azure AD) Configuration Section - Required for SaaS Integration */}
          <div className="ct-entra-section">
            <div className="ct-entra-section__header">
              <h3>Microsoft Entra ID Configuration</h3>
              <p className="ct-entra-section__description">
                To integrate ComsignTrust CMS with your organization, we need your Microsoft Entra ID 
                (formerly Azure AD) app registration details. This allows us to securely communicate 
                with your tenant.
              </p>
              <Link to="/entra-id-guide" className="ct-entra-section__guide-link" target="_blank">
                📖 How to configure Entra ID? (Step-by-step guide)
              </Link>
            </div>

            <div className="ct-entra-section__security-note">
              <span className="ct-entra-section__security-icon">🔒</span>
              <div>
                <strong>Security Note:</strong> Your credentials are transmitted securely over HTTPS 
                and encrypted at rest. We recommend using a dedicated app registration with minimal 
                required permissions for this integration.
              </div>
            </div>

            <div className="ct-form-row">
              <div className="ct-input-group">
                <label className="ct-label--primary">
                  Application (Client) ID <span className="ct-required">*</span>
                </label>
                <input
                  type="text"
                  className={`ct-input--primary ${entraErrors.clientId ? 'ct-input--error' : ''}`}
                  value={entraClientId}
                  onChange={(e) => {
                    setEntraClientId(e.target.value);
                    setEntraErrors((prev) => ({ ...prev, clientId: undefined }));
                  }}
                  placeholder="12345678-1234-1234-1234-123456789abc"
                />
                <span className="ct-input-hint">Found on the app's Overview page as "Application (client) ID"</span>
                {entraErrors.clientId && <div className="ct-input-error">{entraErrors.clientId}</div>}
              </div>
              <div className="ct-input-group">
                <label className="ct-label--primary">
                  Directory (Tenant) ID <span className="ct-required">*</span>
                </label>
                <input
                  type="text"
                  className={`ct-input--primary ${entraErrors.tenantId ? 'ct-input--error' : ''}`}
                  value={entraTenantId || userInfo?.tenantId || ''}
                  onChange={(e) => {
                    setEntraTenantId(e.target.value);
                    setEntraErrors((prev) => ({ ...prev, tenantId: undefined }));
                  }}
                  placeholder="87654321-4321-4321-4321-cba987654321"
                />
                <span className="ct-input-hint">
                  {userInfo?.tenantId 
                    ? 'Pre-filled from your sign-in. Change only if using a different tenant.'
                    : 'Found on the app\'s Overview page as "Directory (tenant) ID"'
                  }
                </span>
                {entraErrors.tenantId && <div className="ct-input-error">{entraErrors.tenantId}</div>}
              </div>
            </div>

            <div className="ct-input-group">
              <label className="ct-label--primary">
                Client Secret <span className="ct-required">*</span>
              </label>
              <input
                type="password"
                className={`ct-input--primary ${entraErrors.clientSecret ? 'ct-input--error' : ''}`}
                value={entraClientSecret}
                onChange={(e) => {
                  setEntraClientSecret(e.target.value);
                  setEntraErrors((prev) => ({ ...prev, clientSecret: undefined }));
                }}
                placeholder="Enter your client secret value"
                autoComplete="off"
              />
              <span className="ct-input-hint">
                Found in "Certificates & secrets" → "Client secrets". Use the secret <strong>Value</strong>, not the Secret ID.
              </span>
              {entraErrors.clientSecret && <div className="ct-input-error">{entraErrors.clientSecret}</div>}
            </div>
          </div>

          <div className="ct-input-group">
            <label className="ct-label--primary">Additional Comments</label>
            <textarea
              className="ct-input--primary ct-textarea"
              value={comments}
              onChange={(e) => setComments(e.target.value)}
              rows={3}
              placeholder="Any special requirements or questions?"
            />
          </div>

          <div className="ct-form-actions">
            <Button 
              type="button" 
              variant="outline" 
              size="large" 
              onClick={() => {
                // Fill with demo/test data
                setCustomerName('Gal Cohen');
                setCustomerEmail('galc@comda.co.il');
                setCompanyName('Comda');
                setPhoneNumber('+972-50-123-4567');
                setJobTitle('Software Developer');
                setCountryCode('IL'); // Israel
                setCountryOther('');
                setComments('Demo submission for testing');
                setEntraClientId('12345678-1234-1234-1234-123456789abc');
                setEntraClientSecret('demo-client-secret-value-12345');
                setEntraTenantId('87654321-4321-4321-4321-cba987654321');
                // Clear any errors
                setCountryError('');
                setCountryOtherError('');
                setEntraErrors({});
                setFormErrors({});
              }}
            >
              Fill Test Data
            </Button>
            <Button type="submit" variant="primary" size="large" disabled={isLoading}>
              {isLoading ? 'Saving...' : 'Continue to Token Selection'}
            </Button>
          </div>
        </form>
      </CardBody>
    </Card>
  );

  // Render feature selection step
  const renderFeatureSelectionStep = () => (
    <Card>
      <CardHeader>
        <h2>Choose Your Tokens</h2>
      </CardHeader>
      <CardBody>
        <p className="ct-feature-intro">
          Enable the credential types you need and specify the quantity of tokens you want to purchase.
          These will be added to your metered billing.
        </p>

        <div className="ct-feature-list">
          {availableFeatures.map((feature) => (
            <div
              key={feature.featureId}
              className={`ct-feature-card ${feature.isEnabled ? 'ct-feature-card--enabled' : ''}`}
            >
              <div className="ct-feature-card__header">
                <label className="ct-feature-toggle">
                  <input
                    type="checkbox"
                    checked={feature.isEnabled}
                    onChange={() => handleToggleFeature(feature.featureId)}
                  />
                  <span className="ct-feature-toggle__slider"></span>
                </label>
                <div className="ct-feature-card__info">
                  <h3>{feature.featureName}</h3>
                  <p>{feature.description}</p>
                </div>
                <div className="ct-feature-card__price">
                  ${feature.pricePerUnit.toFixed(2)}
                  <span>/token</span>
                </div>
              </div>

              {feature.isEnabled && (
                <div className="ct-feature-card__body">
                  <div className="ct-quantity-selector">
                    <label>Quantity:</label>
                    <div className="ct-quantity-input">
                      <button
                        type="button"
                        onClick={() => handleQuantityChange(feature.featureId, feature.quantity - 100)}
                        disabled={feature.quantity <= feature.minQuantity}
                      >
                        -
                      </button>
                      <input
                        type="number"
                        value={feature.quantity}
                        onChange={(e) =>
                          handleQuantityChange(feature.featureId, parseInt(e.target.value) || 0)
                        }
                        min={feature.minQuantity}
                        max={feature.maxQuantity}
                      />
                      <button
                        type="button"
                        onClick={() => handleQuantityChange(feature.featureId, feature.quantity + 100)}
                        disabled={feature.quantity >= feature.maxQuantity}
                      >
                        +
                      </button>
                    </div>
                    <span className="ct-quantity-range">
                      ({feature.minQuantity.toLocaleString()} - {feature.maxQuantity.toLocaleString()})
                    </span>
                  </div>
                  <div className="ct-feature-card__subtotal">
                    Subtotal: <strong>${(feature.quantity * feature.pricePerUnit).toFixed(2)}</strong>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>

        <div className="ct-feature-summary">
          <div className="ct-feature-summary__total">
            <span>Estimated Monthly Total:</span>
            <strong>${calculateTotal().toFixed(2)}</strong>
          </div>
          <p className="ct-feature-summary__note">
            * Actual charges will be based on usage and billed through Azure Marketplace.
          </p>
        </div>

        <div className="ct-form-actions">
          <Button
            variant="outline"
            size="large"
            onClick={() => setCurrentStep('customer-info')}
            disabled={isLoading}
          >
            Back
          </Button>
          <Button
            variant="primary"
            size="large"
            onClick={handleFinalize}
            disabled={isLoading || isFinalized}
          >
            {isLoading ? 'Processing...' : isFinalized ? 'Completed' : 'Finish Setup'}
          </Button>
        </div>
      </CardBody>
    </Card>
  );

  // Render thank you step
  const renderThankYouStep = () => (
    <Card>
      <CardBody>
        <div className="ct-thank-you">
          <div className="ct-thank-you__icon">✓</div>
          <h1>Thank You for Subscribing!</h1>
          <p className="ct-thank-you__message">
            Your subscription request has been submitted successfully.
            Our team will review your information and contact you shortly to complete the setup.
          </p>

          {subscription && (
            <div className="ct-thank-you__details">
              <h3>Subscription Details</h3>
              <div className="ct-thank-you__grid">
                <div className="ct-thank-you__item">
                  <span className="ct-thank-you__label">Company</span>
                  <span className="ct-thank-you__value">{subscription.companyName}</span>
                </div>
                <div className="ct-thank-you__item">
                  <span className="ct-thank-you__label">Contact</span>
                  <span className="ct-thank-you__value">{subscription.customerName}</span>
                </div>
                <div className="ct-thank-you__item">
                  <span className="ct-thank-you__label">Email</span>
                  <span className="ct-thank-you__value">{subscription.customerEmail}</span>
                </div>
                <div className="ct-thank-you__item">
                  <span className="ct-thank-you__label">Reference ID</span>
                  <span className="ct-thank-you__value ct-thank-you__value--mono">
                    {subscription.azureSubscriptionId}
                  </span>
                </div>
              </div>

              {subscription.featureSelections.length > 0 && (
                <>
                  <h3>Selected Features</h3>
                  <div className="ct-thank-you__features">
                    {subscription.featureSelections.map((f) => (
                      <div key={f.featureId} className="ct-thank-you__feature">
                        <span>{f.featureName}</span>
                        <span>{f.quantity.toLocaleString()} tokens</span>
                      </div>
                    ))}
                  </div>
                </>
              )}
            </div>
          )}

          <div className="ct-thank-you__next-steps">
            <h3>What happens next?</h3>
            <ol>
              <li>Our team will review your subscription request</li>
              <li>You will receive a confirmation email within 24 hours</li>
              <li>We will contact you to complete the technical setup</li>
              <li>Once activated, you can start using your credentials</li>
            </ol>
          </div>

          <div className="ct-thank-you__contact">
            <p>
              Questions? Contact us at{' '}
              <a href="mailto:support@comsigntrust.com">support@comsigntrust.com</a>
            </p>
          </div>
        </div>
      </CardBody>
    </Card>
  );

  // Render current step
  const renderCurrentStep = () => {
    switch (currentStep) {
      case 'sign-in':
        return renderSignInStep();
      case 'resolving':
        return renderResolvingStep();
      case 'customer-info':
        return renderCustomerInfoStep();
      case 'feature-selection':
        return renderFeatureSelectionStep();
      case 'thank-you':
        return renderThankYouStep();
      default:
        return renderSignInStep();
    }
  };

  return (
    <Layout showSidebar={false}>
      <div className="ct-azure-landing">
        <div className="ct-azure-landing__header">
          <img 
            src="/logo_medium.png" 
            alt="ComsignTrust" 
            className="ct-azure-landing__logo"
            onError={(e) => {
              e.currentTarget.style.display = 'none';
            }}
          />
          <h1>ComsignTrust CMS</h1>
          <p>Azure Marketplace Subscription Setup</p>
        </div>

        {currentStep !== 'thank-you' && renderStepIndicator()}

        {error && (
          <div className="ct-azure-landing__error">
            <span>⚠️</span>
            <p>{error}</p>
            <button onClick={() => setError(null)}>×</button>
          </div>
        )}

        <div className="ct-azure-landing__content">{renderCurrentStep()}</div>
      </div>
    </Layout>
  );
}
