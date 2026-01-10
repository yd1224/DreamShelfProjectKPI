<script setup lang="ts">
  const currentYear = new Date().getFullYear()
</script>

<template>
  <footer class="footer">
    <div class="section">
      <!-- Main Footer Content -->
      <div class="footer-content">
        <!-- Brand Section -->
        <div class="footer-section brand-section">
          <div class="footer-logo">
            <img class="footer-logo-img" src="/images/logo.png" alt="DreamShelf" width="60" height="40"/>
            <span class="brand-name">DreamShelf</span>
          </div>
          <p class="brand-description">
            Your personal digital library for organizing and discovering your next great read.
          </p>
        </div>
      </div>

      <!-- Footer Bottom -->
      <div class="footer-bottom">
        <div class="footer-bottom-content">
          <p class="copyright">
            © {{ currentYear }} DreamShelf. All rights reserved.
          </p>
          <div class="footer-bottom-links">
            <NuxtLink to="/privacy_policy" class="bottom-link">Privacy</NuxtLink>
            <NuxtLink to="/terms_conditions" class="bottom-link">Terms</NuxtLink>
            <NuxtLink to="/cookie_policy" class="bottom-link">Cookies</NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<style scoped lang="scss">
.footer {
  background-color: #1f2937;
  color: #f9fafb;
  margin-top: auto;
}

.footer-content {
  display: grid;
  grid-template-columns: 1fr;
  gap: 2rem;
  padding: 2rem 0 1.5rem;
  text-align: center;
}

.footer-section {
  &.brand-section {
    order: -1; // Ensure brand section appears first on mobile
  }
}

.footer-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
  justify-content: center;

  &-img {
    filter: invert(1);
  }
}

.brand-name {
  font-size: 1.5rem;
  font-weight: 700;
  color: #ffffff;
}

.brand-description {
  color: #d1d5db;
  line-height: 1.6;
  margin-bottom: 1.5rem;
  max-width: 100%;
}


.footer-bottom {
  border-top: 1px solid #374151;
  padding: 1.5rem 0;
}

.footer-bottom-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.copyright {
  color: #9ca3af;
  font-size: 0.875rem;
  margin: 0;
}

.footer-bottom-links {
  display: flex;
  gap: 1.5rem;
}

.bottom-link {
  color: #9ca3af;
  text-decoration: none;
  font-size: 0.875rem;
  transition: color 0.2s ease;

  &:hover {
    color: #d1d5db;
  }
}

// Tablet styles (768px and up)
@include media-breakpoint-up(md) {
  .footer-content {
    grid-template-columns: repeat(2, 1fr);
    gap: 2.5rem;
    padding: 2.5rem 0 2rem;
    text-align: left;
  }

  .footer-section {
    &.brand-section {
      order: 0;
    }
  }

  .footer-logo {
    justify-content: flex-start;
  }

  .brand-description {
    max-width: 280px;
  }


  .footer-bottom-content {
    flex-direction: row;
    justify-content: space-between;
    gap: 0;
  }
}

// Desktop styles (1024px and up)
@include media-breakpoint-up(lg) {
  .brand-description {
    max-width: 320px;
  }
}
</style>