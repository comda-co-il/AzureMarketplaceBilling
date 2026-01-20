import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Layout } from '../components/Layout';
import { PlanCard } from '../components/Billing';
import { Modal, Button, Alert } from '../components/Common';
import { plansApi, subscriptionsApi } from '../services/api';
import type { Plan, CreateSubscriptionRequest } from '../types';

export function PricingPage() {
  const navigate = useNavigate();
  const { t } = useTranslation();
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
      setError(t('errors.loadingPlans'));
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
      setFormError(t('errors.generic'));
      return;
    }

    try {
      setSubmitting(true);
      setFormError(null);
      const subscription = await subscriptionsApi.create(formData);
      navigate(`/subscription-success/${subscription.id}`);
    } catch (err) {
      setFormError(t('errors.creatingSubscription'));
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
          <p>{t('common.loading')}</p>
        </div>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <div className="ct-pricing ct-pricing--error">
          <Alert type="error" message={error} autoClose={false} />
          <Button onClick={loadPlans}>{t('common.submit')}</Button>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="ct-pricing">
        <header className="ct-pricing__header">
          <h1 className="ct-pricing__title">{t('pricing.title')}</h1>
          <p className="ct-pricing__subtitle">
            {t('pricing.subtitle')}
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

        <Modal
          isOpen={isModalOpen}
          onClose={closeModal}
          title={t('subscription.title', { plan: selectedPlan?.name })}
          size="medium"
        >
          <form onSubmit={handleSubmit} className="ct-signup-form">
            {formError && (
              <Alert type="error" message={formError} autoClose={false} />
            )}
            
            <div className="ct-signup-form__plan-summary">
              <h3>{selectedPlan?.name}</h3>
              <p className="ct-signup-form__price">
                ${selectedPlan?.monthlyPrice}/{t('common.month')}
              </p>
            </div>

            <div className="ct-input-group">
              <label className="ct-label--primary">{t('subscription.form.companyName')} *</label>
              <input
                type="text"
                className="ct-input--primary"
                value={formData.companyName}
                onChange={(e) =>
                  setFormData({ ...formData, companyName: e.target.value })
                }
                required
                placeholder={t('subscription.form.companyPlaceholder')}
              />
            </div>

            <div className="ct-input-group">
              <label className="ct-label--primary">{t('subscription.form.customerName')} *</label>
              <input
                type="text"
                className="ct-input--primary"
                value={formData.customerName}
                onChange={(e) =>
                  setFormData({ ...formData, customerName: e.target.value })
                }
                required
                placeholder={t('subscription.form.namePlaceholder')}
              />
            </div>

            <div className="ct-input-group">
              <label className="ct-label--primary">{t('subscription.form.customerEmail')} *</label>
              <input
                type="email"
                className="ct-input--primary"
                value={formData.customerEmail}
                onChange={(e) =>
                  setFormData({ ...formData, customerEmail: e.target.value })
                }
                required
                placeholder={t('subscription.form.emailPlaceholder')}
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
                  {t('common.yes')}
                </span>
              </label>
            </div>

            <div className="ct-signup-form__actions">
              <Button variant="outline" type="button" onClick={closeModal}>
                {t('common.cancel')}
              </Button>
              <Button variant="primary" type="submit" loading={submitting}>
                {t('common.submit')}
              </Button>
            </div>
          </form>
        </Modal>
      </div>
    </Layout>
  );
}
