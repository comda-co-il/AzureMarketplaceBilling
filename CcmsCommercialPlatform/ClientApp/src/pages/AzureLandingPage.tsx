import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardBody, Button, CountryAutocomplete } from '../components/Common';
import { marketplaceApi } from '../services/api';
import type {
  ResolvedSubscriptionInfo,
  MarketplaceSubscriptionResponse,
  AvailableFeature,
  FeatureSelectionItem,
} from '../types';

type Step = 'resolving' | 'customer-info' | 'feature-selection' | 'thank-you';

interface FeatureState extends AvailableFeature {
  isEnabled: boolean;
  quantity: number;
}


export function AzureLandingPage() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get('token');

  // State
  const [currentStep, setCurrentStep] = useState<Step>('resolving');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

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

  // Step 1: Resolve token on mount
  useEffect(() => {
    if (!token) {
      setError('No token provided. Please access this page from Azure Marketplace.');
      return;
    }

    const resolveToken = async () => {
      try {
        setIsLoading(true);
        const result = await marketplaceApi.resolveToken({ token });
        setResolvedInfo(result);

        // Pre-fill email from purchaser
        if (result.purchaser?.emailId) {
          setCustomerEmail(result.purchaser.emailId);
        }

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
  }, [token]);

  // Step 2: Submit customer info
  const handleSubmitCustomerInfo = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!resolvedInfo) return;

    // If no country is selected, default to "Other"
    const finalCountryCode = countryCode || 'XX';
    if (!countryCode) {
      setCountryCode('XX');
    }

    // Validate country
    let isValid = true;
    setCountryError('');

    // If "Other" is selected, validate that countryOther is filled
    if (finalCountryCode === 'XX' && !countryOther.trim()) {
      setCountryOtherError('Please specify your country');
      isValid = false;
    } else {
      setCountryOtherError('');
    }

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
    if (!resolvedInfo) return;

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
      case 'resolving':
        return 1;
      case 'customer-info':
        return 2;
      case 'feature-selection':
        return 3;
      case 'thank-you':
        return 4;
      default:
        return 1;
    }
  };

  // Render step indicator
  const renderStepIndicator = () => {
    const steps = [
      { num: 1, label: 'Verification' },
      { num: 2, label: 'Your Details' },
      { num: 3, label: 'Choose Tokens' },
      { num: 4, label: 'Complete' },
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
                className="ct-input--primary"
                value={customerName}
                onChange={(e) => setCustomerName(e.target.value)}
                required
                placeholder="John Smith"
              />
            </div>
            <div className="ct-input-group">
              <label className="ct-label--primary">
                Email Address <span className="ct-required">*</span>
              </label>
              <input
                type="email"
                className="ct-input--primary"
                value={customerEmail}
                onChange={(e) => setCustomerEmail(e.target.value)}
                required
                placeholder="john@company.com"
              />
            </div>
          </div>

          <div className="ct-form-row">
            <div className="ct-input-group">
              <label className="ct-label--primary">
                Company Name <span className="ct-required">*</span>
              </label>
              <input
                type="text"
                className="ct-input--primary"
                value={companyName}
                onChange={(e) => setCompanyName(e.target.value)}
                required
                placeholder="Acme Corporation"
              />
            </div>
            <div className="ct-input-group">
              <label className="ct-label--primary">Phone Number</label>
              <input
                type="tel"
                className="ct-input--primary"
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value)}
                placeholder="+1 (555) 123-4567"
              />
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
                setCustomerName('Gal Cohen');
                setCustomerEmail('galc@comda.co.il');
                setCompanyName('Comda');
                setPhoneNumber('+972 (50) 123-4567');
                setJobTitle('Software Developer');
                setCountryCode('IL'); // Israel
                setCountryOther('');
                setComments('Demo submission');
                setCountryError('');
                setCountryOtherError('');
              }}
            >
              Fill All Fields
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
            disabled={isLoading}
          >
            {isLoading ? 'Processing...' : 'Finish Setup'}
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
      case 'resolving':
        return renderResolvingStep();
      case 'customer-info':
        return renderCustomerInfoStep();
      case 'feature-selection':
        return renderFeatureSelectionStep();
      case 'thank-you':
        return renderThankYouStep();
      default:
        return null;
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
