import { useState, useEffect, useCallback } from 'react';
import { subscriptionsApi, usageApi } from '../services/api';
import type { Subscription, UsageSummary } from '../types';

interface UseSubscriptionResult {
  subscription: Subscription | null;
  usage: UsageSummary | null;
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
  recordUsage: (dimensionType: number, quantity: number) => Promise<void>;
  startNewBillingPeriod: () => Promise<void>;
}

export function useSubscription(subscriptionId: string | undefined): UseSubscriptionResult {
  const [subscription, setSubscription] = useState<Subscription | null>(null);
  const [usage, setUsage] = useState<UsageSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!subscriptionId) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const [subscriptionData, usageData] = await Promise.all([
        subscriptionsApi.getById(subscriptionId),
        subscriptionsApi.getUsage(subscriptionId),
      ]);

      setSubscription(subscriptionData);
      setUsage(usageData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load subscription data');
    } finally {
      setLoading(false);
    }
  }, [subscriptionId]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const recordUsage = useCallback(
    async (dimensionType: number, quantity: number) => {
      if (!subscriptionId) return;

      await usageApi.record({
        subscriptionId,
        dimensionType,
        quantity,
      });

      // Refresh usage data after recording
      const usageData = await subscriptionsApi.getUsage(subscriptionId);
      setUsage(usageData);
    },
    [subscriptionId]
  );

  const startNewBillingPeriod = useCallback(async () => {
    if (!subscriptionId) return;

    const updatedSubscription = await subscriptionsApi.startNewBillingPeriod(subscriptionId);
    setSubscription(updatedSubscription);

    // Refresh usage data
    const usageData = await subscriptionsApi.getUsage(subscriptionId);
    setUsage(usageData);
  }, [subscriptionId]);

  return {
    subscription,
    usage,
    loading,
    error,
    refresh: fetchData,
    recordUsage,
    startNewBillingPeriod,
  };
}
