# CCMS Azure Marketplace Billing Portal

A comprehensive billing and subscription management portal for **CCMS (Credential Centralized Management System)** - deployed via Azure Marketplace with metered billing support.

## 🎯 About This Application

This is the **customer-facing billing portal** for CCMS, enabling organizations to:

- Subscribe to CCMS plans via Azure Marketplace
- Monitor credential usage across all 7 dimension types
- Track billing and overage charges in real-time
- Manage subscription lifecycle (upgrade, downgrade, cancel)

---

## 🔐 About CCMS

**CCMS** is a centralized platform for the issuance, management, and lifecycle control of digital credentials. It serves enterprise environments requiring secure identification, authentication, and digital signature capabilities.

### Credential Types Supported

| Type | Description |
|------|-------------|
| **PKI Certificates** | Digital certificates for authentication & signatures (RSA/ECC) |
| **Card Printing** | Physical ID cards with security features |
| **DESFire Encoding** | Contactless NFC smartcard encoding |
| **Prox Credentials** | Proximity card encoding (Wiegand 26-bit) |
| **Biometrics** | Fingerprint and facial recognition enrollment |
| **Mobile Wallet** | Apple/Google Wallet digital credentials |
| **FIDO Keys** | Passwordless authentication with FIDO2 |

### Core Capabilities

- **Credential Lifecycle**: Issue, revoke, suspend, renew, reset, unblock
- **Multi-CA Support**: Integration with enterprise and public CAs
- **HSM Integration**: Hardware Security Module for key protection
- **Active Directory Sync**: Automatic user provisioning
- **Kiosk Self-Service**: Autonomous credential issuance stations
- **RBAC**: Role-based access control for operators
- **Audit Logging**: Complete traceability for compliance

---

## 💰 Billing Model

CCMS uses **Azure Marketplace metered billing** with the following structure:

### Plans

| Plan | Monthly Base | Target |
|------|-------------|--------|
| **Starter** | $299 | Small organizations (up to 100 users) |
| **Professional** | $799 | Medium organizations (up to 500 users) |
| **Enterprise** | $1,999 | Large organizations (unlimited users) |

### Metered Dimensions

Each plan includes quotas for all 7 credential types. Usage beyond the included quota is billed at per-unit overage rates:

- Print credentials
- PKI certificates
- DESFire cards
- Prox cards
- Biometric enrollments
- Wallet passes
- FIDO keys

---

## 🚀 Getting Started

### Prerequisites

- Node.js 18+
- npm or yarn

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

The application will start at `http://localhost:5173`

### Production Build

```bash
npm run build
```

---

## 🌐 Internationalization

The portal supports multiple languages:

- **English** (default)
- **Hebrew** (עברית) with full RTL support

Language can be switched using the selector in the header.

---

## 🔗 Backend API

This frontend connects to the CCMS Billing API which handles:

- Subscription management
- Usage tracking and reporting
- Azure Marketplace webhook integration
- Metered billing event submission

API documentation available at `/swagger` when running the backend.

---

## 📁 Project Structure

```
src/
├── components/
│   ├── Billing/       # PlanCard, UsageBar, UsageHistory
│   ├── Common/        # Button, Card, Modal, Table, Alert
│   └── Layout/        # Header, Sidebar, Layout
├── hooks/             # Custom React hooks
├── i18n/              # Internationalization (en, he)
├── pages/             # Landing, Pricing, Dashboard, Admin
├── services/          # API client
├── styles/            # ComsignTrust design system CSS
└── types/             # TypeScript definitions
```

---

## 🛡️ Security & Compliance

- HTTPS encrypted communication
- Azure AD authentication support
- GDPR-compliant data handling
- ISO 27001 aligned access controls
- Complete audit trail logging

---

## 📞 Support

For technical support or billing inquiries:

- **Email**: support@comsigntrust.com
- **Documentation**: https://docs.comsigntrust.com/ccms

---

© 2026 ComsignTrust. All rights reserved.
