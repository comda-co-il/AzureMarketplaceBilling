import { BrowserRouter, Routes, Route } from 'react-router-dom';
import {
  LandingPage,
  PricingPage,
  SubscriptionSuccessPage,
  DashboardPage,
  UsagePage,
  AdminPage,
  PrivacyPolicyPage,
  AzureLandingPage,
  AzureWebhooksPage,
} from './pages';
import './styles/styles.css';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/pricing" element={<PricingPage />} />
        <Route path="/subscription-success/:id" element={<SubscriptionSuccessPage />} />
        <Route path="/dashboard/:id" element={<DashboardPage />} />
        <Route path="/dashboard/:id/history" element={<UsagePage />} />
        <Route path="/admin" element={<AdminPage />} />
        <Route path="/privacy-policy" element={<PrivacyPolicyPage />} />
        {/* Azure Development Pages (for Technical Configuration in Marketplace offers) */}
        <Route path="/azure-landing" element={<AzureLandingPage />} />
        <Route path="/azure-webhooks" element={<AzureWebhooksPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
