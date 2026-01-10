<script setup lang="ts">
import type { Book } from '~/types/book'

interface Props {
  book: Book
  isAuthor: boolean
}

const props = defineProps<Props>()

const { ages, bookStatus } = useConstants()

const getAgeRestrictionDisplay = (ageId: number) => {
  const age = ages[ageId]
  return age ? `${age.value} ${age.name}` : 'N/A'
}

const getBookStatusDisplay = (statusId: number) => {
  return bookStatus[statusId]?.name || 'Unknown'
}

const getBookStatusClass = (statusId: number) => {
  const statusNames = (bookStatus.map(status => status.name)).map(name => name.toLowerCase())
  return statusNames[statusId] || 'unknown'
}

const sortedChapters = computed(() => {
  return [...props.book.chapters].sort((a, b) => a.chapterNumber - b.chapterNumber)
})
</script>

<template>
  <div class="book-hero">
    <!-- Cover Image -->
    <div class="cover-container">
      <div class="cover-wrapper">
        <img v-if="book.coverImageUrl" :src="`http://localhost:5153/images/${book.coverImageUrl}`"
          :alt="book.title" class="cover-image" />
        <div v-else class="cover-placeholder">
          <span class="placeholder-icon">📖</span>
        </div>
      </div>
    </div>

    <!-- Book Info -->
    <div class="book-main-info">
      <h1 class="book-title">{{ book.title }}</h1>

      <!-- Meta Info -->
      <div class="meta-row">
        <span v-if="book.publicationYear" class="meta-item">
          <svg class="meta-icon" width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M8 14.5C11.5899 14.5 14.5 11.5899 14.5 8C14.5 4.41015 11.5899 1.5 8 1.5C4.41015 1.5 1.5 4.41015 1.5 8C1.5 11.5899 4.41015 14.5 8 14.5Z" stroke="currentColor" stroke-width="1.5"/>
            <path d="M8 4V8L10.5 10.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
          </svg>
          {{ book.publicationYear }}
        </span>
        <span v-if="book.ageRestriction !== null && book.ageRestriction !== undefined" class="meta-item">
          <svg class="meta-icon" width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M8 8C9.65685 8 11 6.65685 11 5C11 3.34315 9.65685 2 8 2C6.34315 2 5 3.34315 5 5C5 6.65685 6.34315 8 8 8Z" stroke="currentColor" stroke-width="1.5"/>
            <path d="M3 14C3 11.7909 5.23858 10 8 10C10.7614 10 13 11.7909 13 14" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
          </svg>
          {{ getAgeRestrictionDisplay(book.ageRestriction) }}
        </span>
        <span v-if="book.bookStatus !== null && book.bookStatus !== undefined" class="meta-item status" :class="getBookStatusClass(book.bookStatus)">
          {{ getBookStatusDisplay(book.bookStatus) }}
        </span>
      </div>

      <!-- Genres -->
      <div v-if="book.genres && book.genres.length > 0" class="genre-tags">
        <span v-for="genre in book.genres" :key="genre.id" class="genre-tag">
          {{ typeof genre === 'string' ? genre : genre.name }}
        </span>
      </div>

      <!-- Action Buttons -->
      <div class="action-buttons">
        <NuxtLink v-if="book.chapters.length > 0" :to="`/books/${book.id}/chapters/${sortedChapters[0]?.uuid}`" :external="true" class="read-button primary">
          <SpriteSymbol name="read" width="20" height="20" />
          Start Reading
        </NuxtLink>
        <NuxtLink v-if="isAuthor" :to="`/books/${book.id}/edit`" :external="true" class="read-button secondary">
          <SpriteSymbol name="edit" width="20" height="20" />
          Edit Book
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.book-hero {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.cover-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.cover-wrapper {
  width: 100%;
  max-width: 200px;
  aspect-ratio: 2/3;
  border-radius: 0.5rem;
  overflow: hidden;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
  background-color: #e5e7eb;
}

.cover-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.cover-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #d1d5db 0%, #9ca3af 100%);
}

.placeholder-icon {
  font-size: 4rem;
}

.book-main-info {
  text-align: center;
}

.book-title {
  font-size: 1.75rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.75rem;
  line-height: 1.2;
}

.meta-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  align-items: center;
  justify-content: center;
  margin-bottom: 1.5rem;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.875rem;
  color: #6b7280;
  font-weight: 500;

  &.status {
    padding: 0.375rem 0.875rem;
    border-radius: 1rem;
    font-weight: 600;
    font-size: 0.8125rem;
    text-transform: capitalize;

    &.announced {
      background-color: #fef3c7;
      color: #92400e;
    }

    &.ongoing {
      background-color: #dbeafe;
      color: #1e40af;
    }

    &.completed {
      background-color: #d1fae5;
      color: #065f46;
    }

    &.frozen {
      background-color: #f3f4f6;
      color: #4b5563;
    }
  }
}

.meta-icon {
  flex-shrink: 0;
  opacity: 0.7;
}

.genre-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  justify-content: center;
  margin-bottom: 1.5rem;
}

.genre-tag {
  padding: 0.375rem 0.75rem;
  background-color: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  color: #4b5563;
  transition: all 0.2s ease;

  &:hover {
    background-color: #f9fafb;
    border-color: #3b82f6;
    color: #3b82f6;
  }
}

.action-buttons {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
  justify-content: center;
}

.read-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.875rem 1.75rem;
  border-radius: 0.5rem;
  font-weight: 600;
  font-size: 0.9375rem;
  text-decoration: none;
  transition: all 0.2s ease;
  border: none;
  cursor: pointer;
  font-family: inherit;

  svg {
    flex-shrink: 0;
  }

  &.primary {
    background-color: #3b82f6;
    color: white;

    &:hover {
      background-color: #2563eb;
      transform: translateY(-2px);
      box-shadow: 0 8px 16px rgba(59, 130, 246, 0.3);
    }
  }

  &.secondary {
    background-color: white;
    color: #000000;
    border: 2px solid #000000;

    &:hover {
      background-color: #eff6ff;
      transform: translateY(-2px);
      box-shadow: 0 8px 16px rgba(59, 130, 246, 0.15);
    }
  }
}

@media (min-width: 768px) {
  .book-hero {
    grid-template-columns: auto 1fr;
    gap: 2.5rem;
    align-items: start;
  }

  .cover-container {
    align-items: flex-start;
  }

  .cover-wrapper {
    max-width: 240px;
  }

  .book-main-info {
    text-align: left;
  }

  .book-title {
    font-size: 2.25rem;
    margin-bottom: 1rem;
  }

  .meta-row {
    justify-content: flex-start;
    margin-bottom: 1.75rem;
  }

  .genre-tags {
    justify-content: flex-start;
    margin-bottom: 1.75rem;
  }

  .action-buttons {
    justify-content: flex-start;
  }
}

@include media-breakpoint-up(lg) {
  .book-hero {
    gap: 3rem;
    margin-bottom: 3rem;
  }

  .cover-wrapper {
    max-width: 280px;
  }

  .book-title {
    font-size: 2.5rem;
  }
}
</style>
