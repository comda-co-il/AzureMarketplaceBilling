# Azure Marketplace Billing Demo Application

A complete Azure Marketplace SaaS billing demo application that simulates the full subscription and metered billing flow. This is a **DEMO application** that demonstrates how Azure Marketplace billing works and can later be connected to real Azure Marketplace APIs.

## Features

- **Subscription Management**: Create, view, upgrade/downgrade, and cancel subscriptions
- **Metered Billing**: Track usage across 7 credential types (PKI, Print, DESFire, Prox, Biometric, Wallet, FIDO)
- **Usage Tracking**: Real-time usage monitoring with overage calculation
- **Admin Dashboard**: View all subscriptions, billing events, and revenue metrics
- **Demo Controls**: Simulate usage and billing scenarios

## Technology Stack

- **Frontend**: React 18+ with TypeScript, Vite
- **Backend**: ASP.NET Core 10 (NET 10)
- **Database**: SQLite (for demo purposes)
- **Styling**: Custom CSS based on ComsignTrust design system

## Project Structure

```
AzureMarketplaceBilling/
├── AzureMarketplaceBilling.sln
├── src/
│   ├── AzureMarketplaceBilling.Api/          # ASP.NET Core Backend
│   │   ├── Controllers/
│   │   │   ├── PlansController.cs
│   │   │   ├── SubscriptionsController.cs
│   │   │   ├── UsageController.cs
│   │   │   ├── AdminController.cs
│   │   │   └── WebhookController.cs
│   │   ├── Services/
│   │   │   ├── IMeteredBillingService.cs
│   │   │   ├── MeteredBillingService.cs
│   │   │   ├── ISubscriptionService.cs
│   │   │   ├── SubscriptionService.cs
│   │   │   ├── IAzureMarketplaceClient.cs
│   │   │   ├── DemoAzureMarketplaceClient.cs
│   │   │   └── AzureMarketplaceClient.cs
│   │   ├── Models/
│   │   │   ├── Plan.cs
│   │   │   ├── PlanQuota.cs
│   │   │   ├── Subscription.cs
│   │   │   ├── UsageRecord.cs
│   │   │   ├── UsageEvent.cs
│   │   │   ├── DTOs/
│   │   │   └── Enums/
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── DbInitializer.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── AzureMarketplaceBilling.Client/       # React Frontend
│       ├── src/
│       │   ├── components/
│       │   │   ├── Layout/
│       │   │   ├── Common/
│       │   │   └── Billing/
│       │   ├── pages/
│       │   ├── services/
│       │   ├── hooks/
│       │   ├── types/
│       │   └── styles/
│       ├── package.json
│       └── vite.config.ts
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/)
- npm or yarn

### Backend Setup

1. Navigate to the API project:
   ```bash
   cd src/AzureMarketplaceBilling.Api
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the API:
   ```bash
   dotnet run
   ```
   
   The API will be available at:
   - HTTPS: https://localhost:7001
   - HTTP: http://localhost:5001
   - Swagger UI: https://localhost:7001/swagger

### Frontend Setup

1. Navigate to the client project:
   ```bash
   cd src/AzureMarketplaceBilling.Client
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```
   
   The frontend will be available at: http://localhost:5173

## Application Pages

| Page | URL | Description |
|------|-----|-------------|
| Landing | `/` | Product overview and features |
| Pricing | `/pricing` | View plans and subscribe |
| Success | `/subscription-success/:id` | Subscription confirmation |
| Dashboard | `/dashboard/:id` | Usage monitoring and demo controls |
| History | `/dashboard/:id/history` | Usage history and export |
| Admin | `/admin` | Admin dashboard (password: `admin123`) |

## Pricing Plans

| Plan | Monthly Price | Best For |
|------|--------------|----------|
| Starter | $299 | Small organizations |
| Professional | $799 | Growing businesses |
| Enterprise | $1,999 | Large-scale deployments |

### Included Quotas per Plan

| Credential Type | Starter | Professional | Enterprise | Overage Price |
|----------------|---------|--------------|------------|---------------|
| PKI Certificates | 100 | 500 | 2,000 | $3.50/unit |
| Print Jobs | 200 | 1,000 | 5,000 | $0.75/unit |
| DESFire Encodings | 100 | 500 | 2,000 | $1.25/unit |
| Prox Encodings | 100 | 500 | 2,000 | $0.35/unit |
| Biometric Enrollments | 50 | 200 | 1,000 | $3.00/unit |
| Wallet Credentials | 50 | 200 | 1,000 | $1.75/unit |
| FIDO Enrollments | 100 | 500 | 2,000 | $2.50/unit |

## API Endpoints

### Plans
- `GET /api/plans` - Get all available plans
- `GET /api/plans/{id}` - Get specific plan details

### Subscriptions
- `POST /api/subscriptions` - Create new subscription
- `GET /api/subscriptions/{id}` - Get subscription details
- `GET /api/subscriptions/{id}/usage` - Get current usage
- `PUT /api/subscriptions/{id}/plan` - Change plan
- `DELETE /api/subscriptions/{id}` - Cancel subscription
- `POST /api/subscriptions/{id}/new-billing-period` - Start new billing period

### Usage
- `POST /api/usage/record` - Record usage event
- `GET /api/usage/{subscriptionId}` - Get usage summary
- `GET /api/usage/{subscriptionId}/history` - Get usage history

### Admin (requires X-Admin-Password header)
- `POST /api/admin/verify` - Verify admin password
- `GET /api/admin/subscriptions` - Get all subscriptions
- `GET /api/admin/usage-events` - Get billing events
- `GET /api/admin/dashboard` - Get dashboard stats

### Webhooks (for Azure simulation)
- `POST /api/webhook/subscription-changed` - Simulate subscription webhook
- `POST /api/webhook/usage-reported` - Confirm usage reported

## Demo Mode

The application runs in demo mode by default (`IsDemo: true` in appsettings.json). In demo mode:

- Metered billing events are logged locally instead of calling Azure APIs
- All Azure Marketplace API calls are simulated
- Events are stored in the local SQLite database

To connect to real Azure Marketplace APIs, set `IsDemo: false` and configure the Azure credentials in appsettings.json.

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=marketplace_billing.db"
  },
  "IsDemo": true,
  "AzureMarketplace": {
    "TenantId": "",
    "ClientId": "",
    "ClientSecret": "",
    "ApiVersion": "2018-08-31"
  }
}
```

## Demo Workflow

1. **Browse Plans**: Visit `/pricing` to see available subscription plans
2. **Subscribe**: Click "Subscribe" on a plan and fill out the signup form
3. **View Dashboard**: After subscribing, access your dashboard at `/dashboard/{subscriptionId}`
4. **Simulate Usage**: Use the "Demo Controls" section to simulate credential operations
5. **Monitor Usage**: Watch progress bars update as usage increases
6. **Observe Overage**: When usage exceeds included quota, overage charges appear
7. **Admin View**: Visit `/admin` (password: `admin123`) to see all subscriptions and billing events

## Azure Marketplace Integration (Future)

The application is designed to integrate with Azure Marketplace APIs:

1. **Metered Billing API**: Report overage usage to Azure
2. **SaaS Fulfillment API**: Handle subscription lifecycle
3. **Webhooks**: Receive notifications from Azure Marketplace

The `AzureMarketplaceClient.cs` contains stubs for these integrations.

## License

This is a demo application for Azure Marketplace billing integration.

## Support

For questions or issues, please contact the development team.
