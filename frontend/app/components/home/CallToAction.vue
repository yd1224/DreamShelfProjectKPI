<script setup lang="ts">

</script>

<template>
  <section class="section-container cta-section">
    <div class="section">
      <div class="cta-content">
        <h2 class="cta-title">Ready to Start Your Reading Journey?</h2>
        <p class="cta-description">
          Join thousands of book lovers and discover your next favorite read today.
        </p>
        <div class="cta-actions">
          <NuxtLink to="/auth/register" class="btn btn-primary">
            Join DreamShelf
          </NuxtLink>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped lang="scss">
.cta-section {
  background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
  color: white;
}

.cta-content {
  text-align: center;
  max-width: 600px;
  margin: 0 auto;
}

.cta-title {
  font-size: 2.5rem;
  font-weight: 800;
  margin-bottom: 1.5rem;

  @media (min-width: 768px) {
    font-size: 3rem;
  }
}

.cta-description {
  font-size: 1.125rem;
  line-height: 1.6;
  margin-bottom: 2.5rem;
  opacity: 0.9;
  color: #d1d5db;

  @media (min-width: 768px) {
    font-size: 1.25rem;
  }
}

.cta-actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;

  @media (min-width: 768px) {
    flex-direction: row;
    gap: 1.5rem;
    justify-content: center;
  }
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0.75rem 2rem;
  border-radius: 0.5rem;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.3s ease;
  border: 2px solid transparent;

  &.btn-primary {
    background-color: #3b82f6;
    color: white;

    &:hover {
      background-color: #2563eb;
      transform: translateY(-2px);
      box-shadow: 0 10px 25px -5px rgba(59, 130, 246, 0.4);
    }
  }

  &.btn-outline {
    background-color: transparent;
    color: white;
    border-color: rgba(255, 255, 255, 0.3);

    &:hover {
      background-color: rgba(255, 255, 255, 0.1);
      border-color: white;
      transform: translateY(-2px);
    }
  }
}
</style>
