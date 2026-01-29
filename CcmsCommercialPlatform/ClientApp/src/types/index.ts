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

// Azure Webhook Event for development/debugging
export interface AzureWebhookEvent {
  id: number;
  rawPayload: string;
  action?: string;
  activityId?: string;
  subscriptionId?: string;
  publisherId?: string;
  offerId?: string;
  planId?: string;
  operationId?: string;
  status?: string;
  azureTimestamp?: string;
  receivedAt: string;
  headers?: string;
  sourceIp?: string;
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

// Marketplace Subscription Types

export enum MarketplaceSubscriptionStatus {
  PendingCustomerInfo = 0,
  PendingFeatureSelection = 1,
  PendingSubmission = 2,
  SubmittedToExternalSystem = 3,
  Active = 4,
  Cancelled = 5,
  Error = 6,
}

export interface ResolveTokenRequest {
  token: string;
}

export interface PurchaserInfo {
  emailId: string;
  tenantId: string;
  objectId: string;
}

export interface BeneficiaryInfo {
  emailId: string;
  tenantId: string;
  objectId: string;
}

export interface ResolvedSubscriptionInfo {
  subscriptionId: string;
  subscriptionName: string;
  offerId: string;
  planId: string;
  purchaser: PurchaserInfo;
  beneficiary: BeneficiaryInfo;
  marketplaceSubscriptionId: number;
}

export interface SubmitCustomerInfoRequest {
  marketplaceSubscriptionId: number;
  customerName: string;
  customerEmail: string;
  companyName: string;
  phoneNumber?: string;
  jobTitle?: string;
  countryCode?: string;
  countryOther?: string;
  comments?: string;
  // Entra ID (Azure AD) Configuration
  entraClientId: string;
  entraClientSecret: string;
  entraTenantId: string;
  // IP Whitelist for CCMS instance
  whitelistIps?: string[];
}

export interface FeatureSelectionItem {
  featureId: string;
  featureName: string;
  isEnabled: boolean;
  quantity: number;
  pricePerUnit: number;
  notes?: string;
}

export interface SubmitFeatureSelectionRequest {
  marketplaceSubscriptionId: number;
  features: FeatureSelectionItem[];
}

export interface FinalizeSubscriptionRequest {
  marketplaceSubscriptionId: number;
}

export interface FeatureSelectionResponse {
  id: number;
  featureId: string;
  featureName: string;
  isEnabled: boolean;
  quantity: number;
  pricePerUnit: number;
  notes: string;
}

export interface MarketplaceSubscriptionResponse {
  id: number;
  azureSubscriptionId: string;
  offerId: string;
  planId: string;
  subscriptionName: string;
  purchaserEmail: string;
  purchaserTenantId: string;
  customerName: string;
  customerEmail: string;
  companyName: string;
  phoneNumber: string;
  jobTitle: string;
  country: string;
  comments: string;
  status: MarketplaceSubscriptionStatus;
  statusDisplay: string;
  featureSelections: FeatureSelectionResponse[];
  createdAt: string;
  customerInfoSubmittedAt?: string;
  featureSelectionCompletedAt?: string;
  submittedToExternalSystemAt?: string;
}

export interface AvailableFeature {
  featureId: string;
  featureName: string;
  description: string;
  pricePerUnit: number;
  minQuantity: number;
  maxQuantity: number;
}
