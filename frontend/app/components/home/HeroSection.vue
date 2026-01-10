<script setup lang="ts">

</script>

<template>
  <section class="hero-section">
    <div class="hero-content section">
      <div class="hero-text">
        <h1 class="hero-title">
          Discover Your Next
          <span class="highlight">Great Read</span>
        </h1>
        <p class="hero-description">
          Explore thousands of books, connect with fellow readers, and build your personal digital library. 
          Your literary journey starts here.
        </p>
        <div class="hero-actions">
          <NuxtLink to="/books/create" class="btn btn-secondary">
            Add Your Book
          </NuxtLink>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped lang="scss">
.hero-section {
  background-image: url('/images/hero.jpg');
  background-position: center;
  background-size: cover;
  background-repeat: no-repeat;
  color: white;
  padding: 4rem 0;
  position: relative;
  overflow: hidden;

  @media (min-width: 768px) {
    padding: 6rem 0;
  }

  // Additional darker overlay for better text readability
  &::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.5);
    z-index: 1;
  }
}

.hero-content {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  align-items: center;
  position: relative;
  z-index: 2;

  @media (min-width: 768px) {
    grid-template-columns: 1fr 1fr;
    gap: 4rem;
  }
}

.hero-title {
  font-size: 2.5rem;
  font-weight: 800;
  line-height: 1.2;
  margin-bottom: 1.5rem;

  @media (min-width: 768px) {
    font-size: 3.5rem;
  }
}

.hero-description {
  font-size: 1.125rem;
  line-height: 1.6;
  margin-bottom: 2rem;

  @media (min-width: 768px) {
    font-size: 1.25rem;
  }
}

.hero-actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;

  @media (min-width: 768px) {
    flex-direction: row;
    gap: 1.5rem;
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
  background-color: #3b82f6;
  color: white;
}

// Floating Books Animation
.hero-image {
  position: relative;
  height: 300px;
  display: flex;
  justify-content: center;
  align-items: center;

  @media (min-width: 768px) {
    height: 400px;
  }
}
</style>
