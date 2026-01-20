export enum TokenUsageType {
  Print = 0,
  Pki = 1,
  Desfire = 2,
  Prox = 3,
  Biometric = 4,
  Wallet = 5,
  Fido = 6,
}

export enum SubscriptionStatus {
  Active = 0,
  Suspended = 1,
  Cancelled = 2,
}

export interface PlanQuota {
  id: number;
  planId: string;
  dimensionType: TokenUsageType;
  dimensionId: string;
  displayName: string;
  includedQuantity: number;
  overagePrice: number;
}

export interface Plan {
  id: string;
  name: string;
  description: string;
  monthlyPrice: number;
  isPopular: boolean;
  quotas: PlanQuota[];
}

export interface Subscription {
  id: string;
  customerName: string;
  customerEmail: string;
  companyName: string;
  planId: string;
  plan?: Plan;
  startDate: string;
  endDate?: string;
  status: SubscriptionStatus;
  createdAt: string;
  billingPeriodStart: string;
  billingPeriodEnd: string;
}

export interface DimensionUsage {
  dimensionType: TokenUsageType;
  dimensionId: string;
  displayName: string;
  includedQuantity: number;
  usedQuantity: number;
  overageQuantity: number;
  overagePrice: number;
  overageCharges: number;
  usagePercentage: number;
  status: 'Normal' | 'Warning' | 'Critical';
}

export interface UsageSummary {
  subscriptionId: string;
  planId: string;
  planName: string;
  billingPeriodStart: string;
  billingPeriodEnd: string;
  dimensions: DimensionUsage[];
  totalOverageCharges: number;
}

export interface UsageRecord {
  id: number;
  subscriptionId: string;
  dimensionType: TokenUsageType;
  dimensionId: string;
  billingPeriodStart: string;
  usedQuantity: number;
  reportedOverage: number;
  lastUpdated: string;
}

export interface UsageEvent {
  id: number;
  resourceId: string;
  planId: string;
  dimension: string;
  quantity: number;
  amount: number;
  effectiveStartTime: string;
  createdAt: string;
  status: string;
}

export interface CreateSubscriptionRequest {
  customerName: string;
  customerEmail: string;
  companyName: string;
  planId: string;
}

export interface RecordUsageRequest {
  subscriptionId: string;
  dimensionType: TokenUsageType;
  quantity: number;
}

export interface AdminDashboardStats {
  totalActiveSubscriptions: number;
  monthlyRecurringRevenue: number;
  totalMeteredRevenue: number;
  usageEventsToday: number;
  subscriptionsByPlan: Record<string, number>;
}

export interface PaginatedResponse<T> {
  data: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export const TokenUsageTypeNames: Record<TokenUsageType, string> = {
  [TokenUsageType.Print]: 'Print Jobs',
  [TokenUsageType.Pki]: 'PKI Certificates',
  [TokenUsageType.Desfire]: 'DESFire Encodings',
  [TokenUsageType.Prox]: 'Prox Encodings',
  [TokenUsageType.Biometric]: 'Biometric Enrollments',
  [TokenUsageType.Wallet]: 'Wallet Credentials',
  [TokenUsageType.Fido]: 'FIDO Enrollments',
};
