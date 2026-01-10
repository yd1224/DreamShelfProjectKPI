<script setup lang="ts">
import type { Chapter } from '~/types/book'

interface Props {
  chapters: Chapter[]
  bookId: number
}

const props = defineProps<Props>()

const formatDate = (dateString: string | null | undefined) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return 'Invalid Date'
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const sortedChapters = computed(() => {
  return [...props.chapters].sort((a: Chapter, b: Chapter) => a.chapterNumber - b.chapterNumber)
})
</script>

<template>
  <section class="content-card chapters-section">
    <div class="section-header chapters-header">
      <h2 class="section-title">
        <SpriteSymbol name="text-justify" width="24" height="24" />
        Chapters    <span class="chapters-count-badge">{{ chapters.length }}</span>
      </h2>
      <div style="display: flex; align-items: center; gap: 0.5rem; flex-direction: row-reverse;">
        <NuxtLink :to="`/books/${bookId}/chapters/create`" class="create-chapter-button">
          <SpriteSymbol name="plus" width="24" height="24" />
          Create Chapter
        </NuxtLink>
     
      </div>
    </div>

    <div v-if="chapters.length === 0" class="empty-state">
      <svg width="48" height="48" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M9 12H15M9 16H15M17 21H7C5.89543 21 5 20.1046 5 19V5C5 3.89543 5.89543 3 7 3H12.5858C12.851 3 13.1054 3.10536 13.2929 3.29289L18.7071 8.70711C18.8946 8.89464 19 9.149 19 9.41421V19C19 20.1046 18.1046 21 17 21Z" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
      </svg>
      <p>No chapters available yet</p>
      <span class="empty-hint">Chapters will appear here once the author publishes them</span>
    </div>

    <div v-else class="chapters-list">
      <div
        v-for="chapter in sortedChapters" 
        :key="chapter.uuid"
        class="chapter-item">
        <div class="chapter-number">{{ chapter.chapterNumber }}</div>
        <div class="chapter-info">
          <NuxtLink :to="`/books/${bookId}/chapters/${chapter.uuid}`" class="chapter-title" :external="true">{{ chapter.title }}</NuxtLink>
          <div class="chapter-meta">
            <span v-if="chapter.lastUpdatedAt" class="meta-date">
              <SpriteSymbol name="edit" width="14" height="14" />
              last updated: {{ formatDate(chapter.lastUpdatedAt) }}
            </span>
          </div>
        </div>
        <slot name="actions" :chapter="chapter">
          <svg class="chapter-arrow" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M7.5 5L12.5 10L7.5 15" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </slot>
      </div>
    </div>
  </section>
</template>

<style scoped lang="scss">
.content-card {
  background-color: #ffffff;
  border-radius: 0.75rem;
  padding: 0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  border: 1px solid #e5e7eb;
  overflow: hidden;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem;
  border-bottom: 2px solid #f3f4f6;
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

.chapters-count-badge {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: white;
  padding: 0.375rem 0.875rem;
  border-radius: 1.25rem;
  font-size: 0.875rem;
  font-weight: 700;
  box-shadow: 0 2px 8px rgba(59, 130, 246, 0.3);
  min-width: 2rem;
  text-align: center;
}

.empty-state {
  padding: 4rem 2rem;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;

  svg {
    color: #d1d5db;
    margin-bottom: 0.5rem;
  }

  p {
    font-size: 1.0625rem;
    font-weight: 600;
    color: #6b7280;
    margin: 0;
  }

  .empty-hint {
    font-size: 0.875rem;
    color: #9ca3af;
    max-width: 300px;
  }
}

.chapters-list {
  display: flex;
  flex-direction: column;
}

.chapter-item {
  display: grid;
  grid-template-columns: auto 1fr auto;
  gap: 1rem;
  padding: 1.125rem 1.5rem;
  border-bottom: 1px solid #f3f4f6;
  text-decoration: none;
  transition: all 0.2s ease;
  align-items: center;
  position: relative;

  &:last-child {
    border-bottom: none;
  }

  &:hover {
    background-color: #f9fafb;
    padding-left: 1.75rem;

    .chapter-arrow {
      opacity: 1;
      transform: translateX(0);
    }

    .chapter-number {
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
      color: white;
      transform: scale(1.05);
    }
  }
}

.chapter-number {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background-color: #f3f4f6;
  color: #4b5563;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1rem;
  flex-shrink: 0;
  transition: all 0.2s ease;
  border: 2px solid #e5e7eb;
}

.chapter-info {
  min-width: 0;
}

.chapter-title {
  font-size: 1rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 0.375rem 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  line-height: 1.3;
}

.chapter-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.8125rem;
  color: #9ca3af;
  align-items: center;

  .meta-date {
    display: flex;
    align-items: center;
    gap: 0.375rem;

    svg {
      flex-shrink: 0;
      opacity: 0.7;
    }
  }
}

.chapter-arrow {
  color: #d1d5db;
  opacity: 0;
  transform: translateX(-8px);
  transition: all 0.2s ease;
  flex-shrink: 0;
}

@media (min-width: 768px) {
  .section-header {
    padding: 2rem;
  }

  .section-title {
    font-size: 1.5rem;
  }

  .chapter-item {
    padding: 1.25rem 2rem;

    &:hover {
      padding-left: 2.25rem;
    }
  }

  .chapter-number {
    width: 48px;
    height: 48px;
    font-size: 1.125rem;
  }

  .chapter-title {
    white-space: normal;
  }

  .chapter-meta {
    font-size: 0.875rem;
  }
}

@include media-breakpoint-up(lg) {
  .chapter-item {
    padding: 1.5rem 2rem;
  }
}

.create-chapter-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem; // spacing between icon and text
  padding: 0.6rem 1.2rem;
  border-radius: 0.75rem;
  font-weight: 600;
  font-size: 1rem;
  text-decoration: none;
  color:black;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: all 0.2s ease-in-out;
  
  // Icon size adjustment
  svg {
    width: 24px;
    height: 24px;
  }
}

</style>
