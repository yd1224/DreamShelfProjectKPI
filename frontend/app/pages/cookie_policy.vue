<script setup lang="ts">
useSeoMeta({
  title: 'Cookie Policy - DreamShelf',
  description: 'Learn about how DreamShelf uses cookies to enhance your browsing experience and provide personalized content.',
})
</script>

<template>
  <div class="section">
    <div>
      <header class="policy-header">
        <h1 class="policy-title">Cookie Policy</h1>
      </header>

      <div>
        <section class="policy-section">
          <h2>What Are Cookies</h2>
          <p>
            Cookies are small text files that are stored on your device when you visit our website. 
            They help us provide you with a better experience by remembering your preferences and 
            understanding how you use our service.
          </p>
        </section>

        <section class="policy-section">
          <h2>How We Use Cookies</h2>
          <p>DreamShelf uses cookies for the following purposes:</p>
          <ul>
            <li><strong>Essential Cookies:</strong> Required for the website to function properly</li>
            <li><strong>Performance Cookies:</strong> Help us understand how visitors interact with our website</li>
            <li><strong>Functionality Cookies:</strong> Remember your preferences and settings</li>
            <li><strong>Analytics Cookies:</strong> Provide insights into website usage and performance</li>
          </ul>
        </section>

        <section class="policy-section">
          <h2>Types of Cookies We Use</h2>
          
          <div class="cookie-category">
            <h3>Strictly Necessary Cookies</h3>
            <p>
              These cookies are essential for the website to function and cannot be switched off. 
              They are usually set in response to actions made by you, such as logging in or 
              filling in forms.
            </p>
          </div>

          <div class="cookie-category">
            <h3>Performance and Analytics Cookies</h3>
            <p>
              These cookies help us understand how visitors interact with our website by 
              collecting and reporting information anonymously. This helps us improve our 
              website's performance.
            </p>
          </div>

          <div class="cookie-category">
            <h3>Functional Cookies</h3>
            <p>
              These cookies enable enhanced functionality and personalization, such as 
              remembering your reading preferences and book recommendations.
            </p>
          </div>
        </section>

        <section class="policy-section">
          <h2>Managing Your Cookie Preferences</h2>
          <p>
            You can control and manage cookies in various ways. Most web browsers allow you to:
          </p>
          <ul>
            <li>View what cookies are stored on your device</li>
            <li>Delete cookies individually or all at once</li>
            <li>Block cookies from specific websites</li>
            <li>Block all cookies from being set</li>
            <li>Delete all cookies when you close your browser</li>
          </ul>
          <p>
            Please note that disabling certain cookies may affect the functionality of our website 
            and your user experience.
          </p>
        </section>

        <section class="policy-section">
          <h2>Third-Party Cookies</h2>
          <p>
            We may use third-party services that set their own cookies. These services help us 
            provide better functionality and analyze website usage. Third-party cookies are 
            subject to the respective privacy policies of these external services.
          </p>
        </section>

        <section class="policy-section">
          <h2>Updates to This Policy</h2>
          <p>
            We may update this Cookie Policy from time to time to reflect changes in our 
            practices or for other operational, legal, or regulatory reasons. We encourage 
            you to review this policy periodically.
          </p>
        </section>

        <section class="policy-section">
          <h2>Contact Us</h2>
          <p>
            If you have any questions about this Cookie Policy, please contact us at:
          </p>
          <div class="contact-info">
            <p><strong>Email:</strong> privacy@dreamshelf.com</p>
            <p><strong>Address:</strong> DreamShelf Privacy Team, 123 Book Street, Reading City, RC 12345</p>
            <p><strong>Phone:</strong> +48 539 376 324</p>
          </div>
        </section>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.policy-header {
  text-align: center;
  margin-bottom: 2rem;
}

.policy-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0 0 0.5rem 0;
  line-height: 1.2;
}


.policy-section {
  margin-bottom: 2rem;

  h2 {
    font-size: 1.25rem;
    font-weight: 600;
    color: #1f2937;
    margin: 0 0 1rem 0;
    line-height: 1.3;
  }

  h3 {
    font-size: 1.125rem;
    font-weight: 600;
    color: #374151;
    margin: 0 0 0.75rem 0;
  }

  p {
    color: #4b5563;
    line-height: 1.6;
    margin: 0 0 1rem 0;
    font-size: 0.875rem;

    &:last-child {
      margin-bottom: 0;
    }
  }

  ul {
    color: #4b5563;
    line-height: 1.6;
    margin: 0 0 1rem 0;
    padding-left: 1.25rem;
    font-size: 0.875rem;

    li {
      margin-bottom: 0.5rem;

      &:last-child {
        margin-bottom: 0;
      }

      strong {
        color: #1f2937;
        font-weight: 600;
      }
    }
  }
}

.cookie-category {
  background-color: #f3f4f6;
  border-radius: 6px;
  padding: 1rem;
  margin-bottom: 1rem;

  &:last-child {
    margin-bottom: 0;
  }
}

.contact-info {
  background-color: #f9fafb;
  border-left: 4px solid #1f2937;
  padding: 1rem;
  border-radius: 0 6px 6px 0;
  margin-top: 1rem;

  p {
    margin: 0 0 0.5rem 0;
    font-size: 0.875rem;

    &:last-child {
      margin-bottom: 0;
    }

    strong {
      color: #1f2937;
      font-weight: 600;
    }
  }
}

// Tablet styles (768px and up)
@include media-breakpoint-up(md) {
  .policy-title {
    font-size: 2.25rem;
  }

  .policy-section {
    margin-bottom: 2.5rem;

    h2 {
      font-size: 1.5rem;
      margin-bottom: 1.25rem;
    }

    h3 {
      font-size: 1.25rem;
      margin-bottom: 1rem;
    }

    p, ul {
      font-size: 1rem;
    }
  }

  .cookie-category {
    padding: 1.5rem;
  }

  .contact-info {
    padding: 1.5rem;
  }
}

// Desktop styles (1024px and up)
@include media-breakpoint-up(lg) {
  .policy-title {
    font-size: 2.5rem;
  }

  .policy-section {
    margin-bottom: 3rem;

    h2 {
      font-size: 1.75rem;
      margin-bottom: 1.5rem;
    }
  }
}
</style>