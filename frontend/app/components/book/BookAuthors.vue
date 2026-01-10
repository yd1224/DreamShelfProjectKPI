<script setup lang="ts">
import type { Author } from '~/types/book'

interface Props {
  authors: Author[]
}

defineProps<Props>()
</script>

<template>
  <section class="content-card authors-card">
    <div class="section-header">
      <h2 class="section-title">
        <SpriteSymbol name="user" width="24" height="24" />
        {{ authors.length > 1 ? 'Authors' : 'Author' }}
      </h2>
    </div>
    <div class="authors-list">
      <div v-for="author in authors" :key="author.id" class="author-item">
        <div v-if="author.displayName">
          <div class="author-info">
            <h3 class="author-name">{{ author.displayName }}</h3>
          </div>
        </div>
      </div>
    </div>
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

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.25rem;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  font-size: 1.25rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
}

.authors-list {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.author-item {
  display: flex;
  gap: 1rem;
  align-items: start;
  padding: 1rem;
  border-radius: 0.5rem;
  background-color: #f9fafb;
  transition: all 0.2s ease;

  &:hover {
    background-color: #f3f4f6;
    transform: translateX(4px);
  }
}

.author-info {
  flex: 1;
  min-width: 0;
}

.author-name {
  font-size: 1.0625rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 0.375rem 0;
}

@media (min-width: 768px) {
  .content-card {
    padding: 2rem;
  }

  .section-title {
    font-size: 1.5rem;
  }
}
</style>
