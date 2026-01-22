import { Layout } from '../components/Layout';
import { Card, CardHeader, CardBody } from '../components/Common';

/**
 * Support Page for Azure Marketplace
 * 
 * This page is required for Azure Marketplace SaaS offers.
 * Configure this URL in Azure Partner Center -> Contact information -> Support link
 */
export function SupportPage() {
  return (
    <Layout>
      <div className="ct-support-page">
        <div className="ct-support-header">
          <h1>Support</h1>
          <p className="ct-support-subtitle">ComsignTrust CCMS Commercial Platform</p>
        </div>

        <div className="ct-support-content">
          {/* Contact Card */}
          <Card>
            <CardHeader>
              <h2>Contact Support</h2>
            </CardHeader>
            <CardBody>
              <div className="ct-support-contact-grid">
                <div className="ct-support-contact-item">
                  <div className="ct-support-contact-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
                      <polyline points="22,6 12,13 2,6"></polyline>
                    </svg>
                  </div>
                  <div className="ct-support-contact-details">
                    <h3>Email</h3>
                    <a href="mailto:support@comsigntrust.com">support@comsigntrust.com</a>
                  </div>
                </div>

                <div className="ct-support-contact-item">
                  <div className="ct-support-contact-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path>
                    </svg>
                  </div>
                  <div className="ct-support-contact-details">
                    <h3>Phone</h3>
                    <a href="tel:+972-3-6485255">+972-3-6485255</a>
                  </div>
                </div>

                <div className="ct-support-contact-item">
                  <div className="ct-support-contact-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
                      <circle cx="12" cy="10" r="3"></circle>
                    </svg>
                  </div>
                  <div className="ct-support-contact-details">
                    <h3>Address</h3>
                    <address>
                      Atidim Tech Park, Building #4<br />
                      Tel-Aviv 61580, Israel
                    </address>
                  </div>
                </div>

                <div className="ct-support-contact-item">
                  <div className="ct-support-contact-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <circle cx="12" cy="12" r="10"></circle>
                      <polyline points="12 6 12 12 16 14"></polyline>
                    </svg>
                  </div>
                  <div className="ct-support-contact-details">
                    <h3>Business Hours</h3>
                    <p>Sunday - Thursday<br />09:00 - 17:00 (IST)</p>
                  </div>
                </div>
              </div>
            </CardBody>
          </Card>

          {/* Support Options */}
          <Card>
            <CardHeader>
              <h2>How Can We Help?</h2>
            </CardHeader>
            <CardBody>
              <div className="ct-support-options">
                <div className="ct-support-option">
                  <h3>Technical Support</h3>
                  <p>
                    For technical issues, API questions, or integration support with the 
                    CCMS Commercial Platform, please contact our technical team.
                  </p>
                  <a href="mailto:support@comsigntrust.com?subject=Technical Support - CCMS Commercial Platform" className="ct-support-link">
                    Contact Technical Support
                  </a>
                </div>

                <div className="ct-support-option">
                  <h3>Billing & Subscriptions</h3>
                  <p>
                    For questions about your Azure Marketplace subscription, billing, 
                    usage, or plan changes.
                  </p>
                  <a href="mailto:support@comsigntrust.com?subject=Billing Inquiry - CCMS Commercial Platform" className="ct-support-link">
                    Contact Billing Support
                  </a>
                </div>

                <div className="ct-support-option">
                  <h3>Sales & General Inquiries</h3>
                  <p>
                    For sales questions, partnership opportunities, or general information 
                    about ComsignTrust products.
                  </p>
                  <a href="mailto:info@comsigntrust.com?subject=Sales Inquiry - CCMS Commercial Platform" className="ct-support-link">
                    Contact Sales
                  </a>
                </div>
              </div>
            </CardBody>
          </Card>

          {/* Documentation */}
          <Card>
            <CardHeader>
              <h2>Resources</h2>
            </CardHeader>
            <CardBody>
              <div className="ct-support-resources">
                <a href="https://www.comsigntrust.com" target="_blank" rel="noopener noreferrer" className="ct-support-resource">
                  <span className="ct-support-resource-title">ComsignTrust Website</span>
                  <span className="ct-support-resource-desc">Learn more about our products and solutions</span>
                </a>
                <a href="/privacy-policy" className="ct-support-resource">
                  <span className="ct-support-resource-title">Privacy Policy</span>
                  <span className="ct-support-resource-desc">How we handle your data</span>
                </a>
              </div>
            </CardBody>
          </Card>

          {/* Company Info */}
          <div className="ct-support-footer">
            <p>
              <strong>ComsignTrust Ltd.</strong> - The Most Secured Digital Signature Solution
            </p>
            <p className="ct-support-copyright">
              © {new Date().getFullYear()} ComsignTrust Ltd. All Rights Reserved.
            </p>
          </div>
        </div>
      </div>
    </Layout>
  );
}
