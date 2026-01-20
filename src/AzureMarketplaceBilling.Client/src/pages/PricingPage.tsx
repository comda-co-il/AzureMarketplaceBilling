import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { PlanCard } from '../components/Billing';
import { Modal, Button, Alert } from '../components/Common';
import { plansApi, subscriptionsApi } from '../services/api';
import type { Plan, CreateSubscriptionRequest } from '../types';

export function PricingPage() {
  const navigate = useNavigate();
  const [plans, setPlans] = useState<Plan[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedPlan, setSelectedPlan] = useState<Plan | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [formData, setFormData] = useState<CreateSubscriptionRequest>({
    customerName: '',
    customerEmail: '',
    companyName: '',
    planId: '',
  });
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [termsAccepted, setTermsAccepted] = useState(false);

  useEffect(() => {
    loadPlans();
  }, []);

  const loadPlans = async () => {
    try {
      setLoading(true);
      const data = await plansApi.getAll();
      setPlans(data);
    } catch (err) {
      setError('Failed to load pricing plans. Please try again later.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSelectPlan = (planId: string) => {
    const plan = plans.find((p) => p.id === planId);
    if (plan) {
      setSelectedPlan(plan);
      setFormData({ ...formData, planId });
      setIsModalOpen(true);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!termsAccepted) {
      setFormError('Please accept the terms and conditions');
      return;
    }

    try {
      setSubmitting(true);
      setFormError(null);
      const subscription = await subscriptionsApi.create(formData);
      navigate(`/subscription-success/${subscription.id}`);
    } catch (err) {
      setFormError('Failed to create subscription. Please try again.');
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setSelectedPlan(null);
    setFormData({
      customerName: '',
      customerEmail: '',
      companyName: '',
      planId: '',
    });
    setFormError(null);
    setTermsAccepted(false);
  };

  if (loading) {
    return (
      <Layout>
        <div className="ct-pricing ct-pricing--loading">
          <div className="ct-spinner ct-spinner--large"></div>
          <p>Loading pricing plans...</p>
        </div>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <div className="ct-pricing ct-pricing--error">
          <Alert type="error" message={error} autoClose={false} />
          <Button onClick={loadPlans}>Try Again</Button>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="ct-pricing">
        <header className="ct-pricing__header">
          <h1 className="ct-pricing__title">Simple, Transparent Pricing</h1>
          <p className="ct-pricing__subtitle">
            Choose the plan that fits your organization's needs. 
            All plans include access to all seven credential types.
          </p>
        </header>

        <div className="ct-pricing__plans">
          {plans.map((plan) => (
            <PlanCard
              key={plan.id}
              plan={plan}
              onSelect={handleSelectPlan}
            />
          ))}
        </div>

        <section className="ct-pricing__faq">
          <h2 className="ct-pricing__faq-title">Frequently Asked Questions</h2>
          <div className="ct-pricing__faq-grid">
            <div className="ct-pricing__faq-item">
              <h3>What happens when I exceed my quota?</h3>
              <p>
                Usage beyond your included quota is billed at the overage rate shown 
                for each dimension. You'll never be cut off - we simply bill for the extra usage.
              </p>
            </div>
            <div className="ct-pricing__faq-item">
              <h3>Can I change plans at any time?</h3>
              <p>
                Yes! You can upgrade or downgrade your plan at any time. 
                Changes take effect immediately.
              </p>
            </div>
            <div className="ct-pricing__faq-item">
              <h3>Is there a free trial?</h3>
              <p>
                Yes, all plans come with a 14-day free trial. No credit card required to start.
              </p>
            </div>
            <div className="ct-pricing__faq-item">
              <h3>How is billing handled?</h3>
              <p>
                Monthly base fee is charged at the start of each billing period. 
                Overage charges are calculated at the end of the period.
              </p>
            </div>
          </div>
        </section>

        <Modal
          isOpen={isModalOpen}
          onClose={closeModal}
          title="Complete Your Subscription"
          size="medium"
        >
          <form onSubmit={handleSubmit} className="ct-signup-form">
            {formError && (
              <Alert type="error" message={formError} autoClose={false} />
            )}
            
            <div className="ct-signup-form__plan-summary">
              <h3>Selected Plan: {selectedPlan?.name}</h3>
              <p className="ct-signup-form__price">
                ${selectedPlan?.monthlyPrice}/month
              </p>
            </div>

            <div className="ct-input-group">
              <label className="ct-label--primary">Company Name *</label>
              <input
                type="text"
                className="ct-input--primary"
                value={formData.companyName}
                onChange={(e) =>
                  setFormData({ ...formData, companyName: e.target.value })
                }
                required
                placeholder="Enter your company name"
              />
            </div>

            <div className="ct-input-group">
              <label className="ct-label--primary">Contact Name *</label>
              <input
                type="text"
                className="ct-input--primary"
                value={formData.customerName}
                onChange={(e) =>
                  setFormData({ ...formData, customerName: e.target.value })
                }
                required
                placeholder="Enter your full name"
              />
            </div>

            <div className="ct-input-group">
              <label className="ct-label--primary">Email Address *</label>
              <input
                type="email"
                className="ct-input--primary"
                value={formData.customerEmail}
                onChange={(e) =>
                  setFormData({ ...formData, customerEmail: e.target.value })
                }
                required
                placeholder="Enter your email address"
              />
            </div>

            <div className="ct-input-group ct-input-group--checkbox">
              <label className="ct-checkbox">
                <input
                  type="checkbox"
                  checked={termsAccepted}
                  onChange={(e) => setTermsAccepted(e.target.checked)}
                />
                <span className="ct-checkbox__label">
                  I accept the Terms of Service and Privacy Policy
                </span>
              </label>
            </div>

            <div className="ct-signup-form__actions">
              <Button variant="outline" type="button" onClick={closeModal}>
                Cancel
              </Button>
              <Button variant="primary" type="submit" loading={submitting}>
                Complete Subscription
              </Button>
            </div>
          </form>
        </Modal>
      </div>
    </Layout>
  );
}
