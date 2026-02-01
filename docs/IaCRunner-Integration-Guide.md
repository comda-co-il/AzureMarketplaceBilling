# IaCRunner Integration Guide

## Overview

The CCMS Commercial Platform uses a **polling architecture** for infrastructure provisioning. Your IaCRunner service will poll our Azure-hosted API for pending provisioning jobs, process them, and report results via webhook.

## Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                        Your Private Network                         │
│  ┌─────────────┐                                                    │
│  │  IaCRunner  │ ──── OUTBOUND HTTPS ────►  Azure Web App          │
│  └─────────────┘                            (Public Internet)       │
└─────────────────────────────────────────────────────────────────────┘
```

All connections are **outbound from your network** - no inbound firewall rules required.

---

## API Endpoints

**Base URL:** `https://ccms-commercial-platform.azurewebsites.net`

### Authentication

All requests require the `X-Api-Key` header:

```
X-Api-Key: <your-api-key>
```

---

### 1. Poll for Pending Jobs

**Endpoint:** `GET /api/iac/pending-jobs`

**Polling Interval:** Every 15 seconds (recommended)

**Response:**
```json
{
  "jobs": [
    {
      "subscriptionId": 123,
      "azureSubscriptionId": "abc-123-def",
      "companyName": "Acme Corp",
      "customerEmail": "admin@acme.com",
      "createdAt": "2026-02-01T10:30:00Z"
    }
  ],
  "count": 1
}
```

If `count` is 0, no jobs are pending. Continue polling.

---

### 2. Claim a Job

**Endpoint:** `POST /api/iac/claim-job/{subscriptionId}`

**Purpose:** Prevents multiple runners from processing the same job.

**Response (200 OK):**
```json
{
  "message": "Job claimed successfully",
  "deploymentId": "deploy-123-20260201103000",
  "job": {
    "subscriptionId": 123,
    "azureSubscriptionId": "abc-123-def",
    "offerId": "ccms-offer",
    "planId": "standard",
    "customer": {
      "name": "John Doe",
      "email": "john@acme.com",
      "company": "Acme Corp",
      "phone": "+1234567890",
      "jobTitle": "IT Manager",
      "countryCode": "US",
      "countryOther": null,
      "comments": "Need fast deployment"
    },
    "entraConfig": {
      "clientId": "guid-here",
      "clientSecret": "secret-here",
      "tenantId": "tenant-guid",
      "adminGroupObjectId": "group-guid"
    },
    "purchaser": {
      "email": "purchaser@acme.com",
      "tenantId": "tenant-guid",
      "objectId": "user-guid"
    },
    "features": [
      {
        "featureId": "feature-1",
        "featureName": "Advanced Analytics",
        "isEnabled": true,
        "quantity": 100,
        "pricePerUnit": 0.50
      }
    ],
    "whitelistIps": ["192.168.1.0/24", "10.0.0.5"],
    "webhookUrl": "https://ccms-commercial-platform.azurewebsites.net/api/webhook/ccms-provisioning",
    "timestamp": "2026-02-01T10:30:00Z"
  }
}
```

**Response (409 Conflict):** Job already claimed by another runner
```json
{
  "message": "Job 123 is not available for claiming",
  "currentStatus": "Provisioning"
}
```

---

### 3. Report Completion (Webhook)

**Endpoint:** `POST /api/webhook/ccms-provisioning`

**On Success:**
```json
{
  "id": "deploy-123-20260201103000",
  "success": true,
  "ccms_url": "https://acme.ccms.comsigntrust.com",
  "message": "Provisioning completed successfully"
}
```

**On Failure:**
```json
{
  "id": "deploy-123-20260201103000",
  "success": false,
  "error": "Failed to create Azure resources: quota exceeded"
}
```

**Important:** The `id` field must match the `deploymentId` returned from claim-job.

---

## Recommended Flow

```python
# Pseudocode for IaCRunner

API_KEY = "your-api-key"
BASE_URL = "https://ccms-commercial-platform.azurewebsites.net"
POLL_INTERVAL = 15  # seconds

while True:
    # 1. Poll for pending jobs
    response = GET(f"{BASE_URL}/api/iac/pending-jobs", headers={"X-Api-Key": API_KEY})
    
    if response.count == 0:
        sleep(POLL_INTERVAL)
        continue
    
    for job_summary in response.jobs:
        # 2. Claim the job
        claim_response = POST(
            f"{BASE_URL}/api/iac/claim-job/{job_summary.subscriptionId}",
            headers={"X-Api-Key": API_KEY}
        )
        
        if claim_response.status == 409:
            continue  # Already claimed by another runner
        
        deployment_id = claim_response.deploymentId
        job = claim_response.job
        
        # 3. Provision the infrastructure
        try:
            ccms_url = provision_ccms(job)
            
            # 4. Report success
            POST(job.webhookUrl, json={
                "id": deployment_id,
                "success": True,
                "ccms_url": ccms_url
            })
        except Exception as e:
            # 4. Report failure
            POST(job.webhookUrl, json={
                "id": deployment_id,
                "success": False,
                "error": str(e)
            })
    
    sleep(POLL_INTERVAL)
```

---

## Error Handling

| HTTP Code | Meaning | Action |
|-----------|---------|--------|
| 200 | Success | Process response |
| 401 | Invalid API key | Check X-Api-Key header |
| 404 | Job not found | Skip, poll again |
| 409 | Job already claimed | Skip, poll again |
| 500 | Server error | Retry with backoff |

**Recommended backoff:** On 5xx errors, wait 60 seconds before retrying.

---

## Job States

The subscription goes through these states:

| Status | Description |
|--------|-------------|
| `PendingProvisioning` | Job is waiting to be picked up (visible in pending-jobs) |
| `Provisioning` | Job has been claimed and is being processed |
| `Active` | Provisioning completed successfully |
| `ProvisioningFailed` | Provisioning failed |

---

## Configuration (Azure App Side)

The following settings are configured in `appsettings.json`:

```json
{
  "IaCRunner": {
    "ApiKey": "your-shared-secret-key",
    "JobClaimTimeoutMinutes": 30,
    "MaxRetryCount": 3
  }
}
```

---

## Testing

1. Create a test subscription through the UI (use token="test" for demo mode)
2. Verify it appears in `GET /api/iac/pending-jobs`
3. Claim it with `POST /api/iac/claim-job/{id}`
4. Call the webhook with test data to complete the flow

---

## Security Notes

- Always use HTTPS
- Store the API key securely (environment variable, secrets manager)
- The API key should be rotated periodically
- Consider IP whitelisting on the Azure App Service if your runner has a static IP
