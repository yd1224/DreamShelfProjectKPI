<script setup lang="ts">
interface Props {
  description: string
}

defineProps<Props>()
</script>

<template>
  <section class="content-card">
    <h2 class="section-title">Description</h2>
    <div class="description-text" v-html="description"></div>
  </section>
</template>

<style scoped lang="scss">
.content-card {
  background-color: #ffffff;
  border-radius: 0.75rem;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  border: 1px solid #e5e7eb;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0 0 1rem 0;
}

.description-text {
  color: #4b5563;
  line-height: 1.8;
  font-size: 0.9375rem;

  :deep(p) {
    margin-bottom: 1rem;
    
    &:last-child {
      margin-bottom: 0;
    }
  }

  :deep(strong) {
    font-weight: 600;
    color: #374151;
  }

  :deep(em) {
    font-style: italic;
  }

  :deep(u) {
    text-decoration: underline;
  }

  :deep(ul), :deep(ol) {
    margin: 0.75rem 0;
    padding-left: 1.5rem;
  }

  :deep(li) {
    margin-bottom: 0.5rem;
  }

  :deep(a) {
    color: #3b82f6;
    text-decoration: underline;
    
    &:hover {
      color: #2563eb;
    }
  }

  :deep(h1), :deep(h2), :deep(h3), :deep(h4), :deep(h5), :deep(h6) {
    font-weight: 700;
    color: #1f2937;
    margin: 1.5rem 0 0.75rem 0;
    line-height: 1.3;
  }

  :deep(h1) { font-size: 1.5rem; }
  :deep(h2) { font-size: 1.25rem; }
  :deep(h3) { font-size: 1.125rem; }
  :deep(h4) { font-size: 1rem; }
}

@media (min-width: 768px) {
  .content-card {
    padding: 2rem;
  }

  .section-title {
    font-size: 1.5rem;
    margin-bottom: 1.5rem;
  }

  .description-text {
    font-size: 1rem;
  }
}
</style>
