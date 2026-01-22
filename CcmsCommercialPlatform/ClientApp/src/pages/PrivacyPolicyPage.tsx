import { Layout } from '../components/Layout';

/**
 * Privacy Policy Page for Azure Marketplace
 * 
 * This page is required for Azure Marketplace SaaS offers.
 * Configure this URL in Azure Partner Center -> Offer Setup -> Privacy policy link
 */
export function PrivacyPolicyPage() {
  return (
    <Layout>
      <div className="ct-privacy-policy">
        <div className="ct-privacy-header">
          <h1>Privacy Policy</h1>
          <p className="ct-privacy-subtitle">ComsignTrust CCMS Commercial Platform</p>
        </div>

        <div className="ct-privacy-content">
          <section className="ct-privacy-section">
            <h2>In this Privacy Policy, we describe how we collect, store, and use your data</h2>
            <p>
              We use your personal data to provide you with the best service for you. We take our 
              responsibility seriously to handle your data and we are committed to protecting your 
              privacy. There are steps you can take in order to control what we do with your data 
              and we will explain the following steps in this privacy policy.
            </p>
            <p>
              When we talk about data and personal data in this privacy policy, we mean personal 
              data that identifies you or can be used to identify you, such as your name and contact 
              information. This may also include information on how you use our websites and applications.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>Responsibility for the information</h2>
            <p>
              ComsignTrust Ltd. is responsible for your information. Our address is Kiryat Atidim 
              Building 4, Tel Aviv, Israel, Zip Code: 61580. We are the controller of the personal 
              information we collect from you – which means that we control your personal data 
              collection methods and the goals for which they are collected and processed.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>Personal information we collect</h2>
            <p>Depending on your use of the CCMS Commercial Platform, here is the information we collect:</p>
            
            <div className="ct-privacy-table-wrapper">
              <table className="ct-privacy-table">
                <thead>
                  <tr>
                    <th>When we collect</th>
                    <th>What we collect</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>When you subscribe through Azure Marketplace</td>
                    <td>Contact details (name, email address, company name), Azure subscription information</td>
                  </tr>
                  <tr>
                    <td>When you use our services</td>
                    <td>Usage data, service interaction logs, feature utilization metrics</td>
                  </tr>
                  <tr>
                    <td>When you contact us for support</td>
                    <td>Communication records (emails, support tickets, chat messages)</td>
                  </tr>
                  <tr>
                    <td>When you use our platform</td>
                    <td>Information about your activity, preferences, and service configuration</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </section>

          <section className="ct-privacy-section">
            <h2>Sensitive personal information</h2>
            <p>
              Certain types of personal information such as medical, biometric, racial, religious, 
              etc. are sensitive information that requires separate attention and protection because 
              of its importance. We do not require such information for the CCMS Commercial Platform services.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>How we use personal information</h2>
            <p>
              We may use personal information only when we have a reasonable reason to do so. 
              According to the GDPR, we may only use the information in one or more of the following cases:
            </p>
            <ol>
              <li>The information is necessary to fulfill a contract between us</li>
              <li>The information is necessary for compliance with the law</li>
              <li>When you agree to it (give us your consent)</li>
              <li>When it is part of our legitimate interest as an organization</li>
            </ol>
            <p>
              Legitimate interests are our business or marketing reasons for using your data, 
              but we will not place our legitimate interests above your best interests.
            </p>

            <div className="ct-privacy-table-wrapper">
              <table className="ct-privacy-table">
                <thead>
                  <tr>
                    <th>What we use your data for</th>
                    <th>Lawful basis</th>
                    <th>Our legitimate interest</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>Provide CCMS services and manage your subscription</td>
                    <td>Fulfilling a contract between us</td>
                    <td>Be effective about how we comply with our contracts and provide our services</td>
                  </tr>
                  <tr>
                    <td>Communicate with you about your subscription and usage</td>
                    <td>Fulfilling a contract, Our legitimate interest</td>
                    <td>Keeping our records updated and providing relevant service information</td>
                  </tr>
                  <tr>
                    <td>Process billing and usage-based charges through Azure</td>
                    <td>Fulfilling a contract, Legal duty</td>
                    <td>Manage our business efficiently including accounting and billing</td>
                  </tr>
                  <tr>
                    <td>Improve our services and develop new features</td>
                    <td>Our legitimate interest, Your consent</td>
                    <td>Development of services and products</td>
                  </tr>
                  <tr>
                    <td>Respond to support requests and complaints</td>
                    <td>Fulfilling a contract, Our legitimate interest</td>
                    <td>Identifying customer needs and improving service quality</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </section>

          <section className="ct-privacy-section">
            <h2>Marketing communications</h2>
            <p>
              We may send you email marketing communications if you indicate that you wish to 
              receive such emails. Our marketing communications include information about our 
              new and existing services, special offers we believe you may like, and other 
              products that we believe may be useful to you.
            </p>
            <p>
              If you've previously opted in to receive emails from us, you can opt out of 
              receiving marketing emails by clicking the link to unsubscribe from all of our 
              marketing emails.
            </p>
            <p>
              Please note that if you let us know that you do not wish to receive marketing 
              emails, you will still receive service emails related to your subscription, 
              usage alerts, and important service updates.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>Data retention</h2>
            <p>
              We save your data only as long as we need it. The amount of time we need your 
              data depends on our use of that data, whether it is to provide you with services, 
              for our legitimate interests (described above), or to comply with the law.
            </p>
            <p>
              We will actively review the information we hold and when there is no longer a 
              need for a customer, legal need or business need to hold the information, we 
              will delete it securely or in some cases make it anonymous.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>How we protect your information</h2>
            <p>
              We protect your personal data from unauthorized access, illegal use, loss, 
              manipulation, or destruction. We use technical means such as encryption and 
              password protection to protect the data and the systems in which they are held.
            </p>
            <p>
              We also use operational data protection measures, for example by limiting the 
              number of people who have access to the databases in which the information is held.
            </p>
            <p>
              We maintain these security measures in review and address industry security 
              standards to keep up to date on best practices.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>Sharing your data</h2>
            <p>We share some of your personal data with or receive personal data from the following categories of third parties:</p>
            <ol>
              <li>
                <strong>Microsoft Azure:</strong> Your subscription and usage information is 
                shared with Microsoft Azure for billing and marketplace integration purposes.
              </li>
              <li>
                <strong>Payment processors:</strong> To process payments, prevent, and identify 
                fraud, we process payment information through Microsoft Azure Marketplace billing.
              </li>
              <li>
                <strong>Service providers:</strong> We may share your data with companies that 
                provide us with products and/or services that are relevant to delivering our 
                services to you. We will ensure that our suppliers respect your personal data 
                and comply with data protection laws.
              </li>
            </ol>
          </section>

          <section className="ct-privacy-section">
            <h2>Your rights</h2>
            <ol>
              <li>
                You are entitled to see copies of all of your personal data that we hold, 
                to request correction or deletion of such data. You can also restrict or 
                resist processing your data.
              </li>
              <li>
                If you've given us your consent to use your data, such as to be able to 
                send you marketing emails, you can opt out at any time.
              </li>
              <li>
                You may object to our use of your data in the event that we rely on our 
                legitimate interests to do so.
              </li>
              <li>
                To raise any objection, remove consent or exercise your rights, you may 
                email us at <a href="mailto:support@comda.co.il">support@comda.co.il</a>.
              </li>
              <li>
                You may also contact the Company's Data Protection Officer (DPO) directly 
                at <a href="mailto:yaire@comda.co.il">yaire@comda.co.il</a>.
              </li>
            </ol>
            <p>
              When you contact us, we will get back to you as soon as possible and when 
              possible within less than a month. If your request is more complicated, we 
              may respond to you with a slight delay, but not later than two months since 
              the request.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>Complaints</h2>
            <p>
              If you have complaints regarding the processing of your personal data by 
              ComsignTrust, please email us at <a href="mailto:support@comda.co.il">support@comda.co.il</a> or 
              by mail to the company's offices at Kiryat Atidim 4, Tel Aviv, Israel 61580.
            </p>
            <p>
              In addition, you can contact the Company's Data Protection Officer (DPO) 
              directly at <a href="mailto:yaire@comda.co.il">yaire@comda.co.il</a>.
            </p>
            <p>
              Please note that you have the right to file a complaint with the regulatory 
              authority responsible for protecting personal data in the country in which 
              you live or work, in case you believe that data protection laws are in violation.
            </p>
          </section>

          <section className="ct-privacy-section">
            <h2>Contact us</h2>
            <p>
              For any matter relating to the protection of privacy and the use of your 
              personal data, please send an email to <a href="mailto:support@comda.co.il">support@comda.co.il</a> or 
              by mail to the company's offices at:
            </p>
            <address className="ct-privacy-address">
              <strong>ComsignTrust Ltd.</strong><br />
              Atidim Tech Park, Building #4<br />
              P.O.Box 58007<br />
              Tel-Aviv 61580, Israel<br />
              <br />
              <strong>Email:</strong> <a href="mailto:info@comsigntrust.com">info@comsigntrust.com</a><br />
              <strong>Phone:</strong> +972-3-6485255<br />
              <strong>Fax:</strong> +972-3-6474206
            </address>
            <p>
              You can also contact the Company's Data Protection Officer (DPO) directly 
              at <a href="mailto:yaire@comda.co.il">yaire@comda.co.il</a>.
            </p>
          </section>

          <section className="ct-privacy-section ct-privacy-footer">
            <p>
              <strong>Last updated:</strong> January 2026
            </p>
            <p className="ct-privacy-copyright">
              ComsignTrust™ – The Most Secured Digital Signature Solution. ComsignTrust™, 
              SIGNER-1™, CCMS™, and other service names are the trademarks and copyright 
              of ComsignTrust™ Ltd.
            </p>
          </section>
        </div>
      </div>
    </Layout>
  );
}
