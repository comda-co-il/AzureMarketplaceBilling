import axios from 'axios';
import type {
  Plan,
  Subscription,
  UsageSummary,
  UsageRecord,
  UsageEvent,
  CreateSubscriptionRequest,
  RecordUsageRequest,
  AdminDashboardStats,
  PaginatedResponse,
  AzureWebhookEvent,
} from '../types';

// Use relative URL so it works in both dev and production
const API_BASE_URL = '/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Plans API
export const plansApi = {
  getAll: async (): Promise<Plan[]> => {
    const response = await api.get<Plan[]>('/plans');
    return response.data;
  },

  getById: async (id: string): Promise<Plan> => {
    const response = await api.get<Plan>(`/plans/${id}`);
    return response.data;
  },
};

// Subscriptions API
export const subscriptionsApi = {
  create: async (request: CreateSubscriptionRequest): Promise<Subscription> => {
    const response = await api.post<Subscription>('/subscriptions', request);
    return response.data;
  },

  getById: async (id: string): Promise<Subscription> => {
    const response = await api.get<Subscription>(`/subscriptions/${id}`);
    return response.data;
  },

  getUsage: async (id: string): Promise<UsageSummary> => {
    const response = await api.get<UsageSummary>(`/subscriptions/${id}/usage`);
    return response.data;
  },

  changePlan: async (id: string, newPlanId: string): Promise<Subscription> => {
    const response = await api.put<Subscription>(`/subscriptions/${id}/plan`, {
      newPlanId,
    });
    return response.data;
  },

  cancel: async (id: string): Promise<void> => {
    await api.delete(`/subscriptions/${id}`);
  },

  startNewBillingPeriod: async (id: string): Promise<Subscription> => {
    const response = await api.post<Subscription>(
      `/subscriptions/${id}/new-billing-period`
    );
    return response.data;
  },
};

// Usage API
export const usageApi = {
  record: async (request: RecordUsageRequest): Promise<void> => {
    await api.post('/usage/record', request);
  },

  getSummary: async (subscriptionId: string): Promise<UsageSummary> => {
    const response = await api.get<UsageSummary>(`/usage/${subscriptionId}`);
    return response.data;
  },

  getHistory: async (
    subscriptionId: string,
    params?: {
      startDate?: string;
      endDate?: string;
      dimensionType?: number;
    }
  ): Promise<UsageRecord[]> => {
    const response = await api.get<UsageRecord[]>(
      `/usage/${subscriptionId}/history`,
      { params }
    );
    return response.data;
  },
};

// Admin API
export const adminApi = {
  verifyPassword: async (password: string): Promise<boolean> => {
    try {
      const response = await api.post<{ valid: boolean }>('/admin/verify', {
        password,
      });
      return response.data.valid;
    } catch {
      return false;
    }
  },

  getSubscriptions: async (
    password: string,
    page = 1,
    pageSize = 20,
    status?: string
  ): Promise<PaginatedResponse<Subscription>> => {
    const response = await api.get<PaginatedResponse<Subscription>>(
      '/admin/subscriptions',
      {
        params: { page, pageSize, status },
        headers: { 'X-Admin-Password': password },
      }
    );
    return response.data;
  },

  getUsageEvents: async (
    password: string,
    page = 1,
    pageSize = 50,
    subscriptionId?: string
  ): Promise<PaginatedResponse<UsageEvent>> => {
    const response = await api.get<PaginatedResponse<UsageEvent>>(
      '/admin/usage-events',
      {
        params: { page, pageSize, subscriptionId },
        headers: { 'X-Admin-Password': password },
      }
    );
    return response.data;
  },

  getDashboardStats: async (password: string): Promise<AdminDashboardStats> => {
    const response = await api.get<AdminDashboardStats>('/admin/dashboard', {
      headers: { 'X-Admin-Password': password },
    });
    return response.data;
  },
};

// Azure Webhook Events API (Development)
export const azureWebhookApi = {
  getEvents: async (
    page = 1,
    pageSize = 50
  ): Promise<PaginatedResponse<AzureWebhookEvent>> => {
    const response = await api.get<PaginatedResponse<AzureWebhookEvent>>(
      '/webhook/azure/events',
      { params: { page, pageSize } }
    );
    return response.data;
  },

  getEvent: async (id: number): Promise<AzureWebhookEvent> => {
    const response = await api.get<AzureWebhookEvent>(
      `/webhook/azure/events/${id}`
    );
    return response.data;
  },

  clearEvents: async (): Promise<{ message: string }> => {
    const response = await api.delete<{ message: string }>(
      '/webhook/azure/events'
    );
    return response.data;
  },
};

export default api;
