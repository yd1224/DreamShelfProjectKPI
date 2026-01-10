<script setup lang="ts">
interface Props {
  bookId: number | string
  prevChapterId?: string | null
  nextChapterId?: string | null
}

interface Emits {
  (e: 'openChapters'): void
}

defineProps<Props>()
defineEmits<Emits>()
</script>

<template>
  <nav class="reader-navigation">
    <NuxtLink 
      v-if="prevChapterId" 
      :to="`/books/${bookId}/chapters/${prevChapterId}`"
      class="nav-button prev"
    >
      <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M12.5 15L7.5 10L12.5 5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
      <span class="nav-text">Previous</span>
    </NuxtLink>
    <div v-else class="nav-button prev disabled">
      <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M12.5 15L7.5 10L12.5 5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
      <span class="nav-text">Previous</span>
    </div>

    <button class="nav-button chapters" @click="$emit('openChapters')">
      <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M3 5H17M3 10H17M3 15H12" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
      </svg>
      <span class="nav-text">Chapters</span>
    </button>

    <NuxtLink 
      v-if="nextChapterId" 
      :to="`/books/${bookId}/chapters/${nextChapterId}`"
      class="nav-button next"
    >
      <span class="nav-text">Next</span>
      <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M7.5 5L12.5 10L7.5 15" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    </NuxtLink>
    <div v-else class="nav-button next disabled">
      <span class="nav-text">Next</span>
      <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M7.5 5L12.5 10L7.5 15" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    </div>
  </nav>
</template>

<style scoped lang="scss">
@mixin mobile {
  @media (max-width: 767px) {
    @content;
  }
}

@mixin tablet {
  @media (min-width: 768px) {
    @content;
  }
}

@mixin desktop {
  @media (min-width: 992px) {
    @content;
  }
}

.reader-navigation {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 2rem 1rem;
  margin-top: 2rem;
  border-top: 1px solid rgba(75, 85, 99, 0.3);
  max-width: 720px;
  margin-left: auto;
  margin-right: auto;

  @include tablet {
    gap: 1rem;
    padding: 2.5rem 1.5rem;
  }

  @include desktop {
    gap: 1.25rem;
    padding: 3rem 2rem;
  }
}

.nav-button {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.875rem 1.25rem;
  border-radius: 12px;
  font-weight: 600;
  font-size: 0.9375rem;
  text-decoration: none;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid rgba(75, 85, 99, 0.4);
  background: rgba(31, 41, 55, 0.6);
  color: #d1d5db;
  cursor: pointer;
  font-family: inherit;
  min-width: 100px;

  @include mobile {
    padding: 0.75rem 1rem;
    font-size: 0.875rem;
    min-width: auto;
    flex: 1;
  }

  @include tablet {
    padding: 1rem 1.5rem;
    min-width: 120px;
  }

  svg {
    flex-shrink: 0;
    transition: transform 0.3s ease;
  }

  &:not(.disabled):hover {
    background: rgba(59, 130, 246, 0.15);
    border-color: rgba(59, 130, 246, 0.4);
    color: #93c5fd;
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(59, 130, 246, 0.2);
  }

  &.prev:not(.disabled):hover svg {
    transform: translateX(-3px);
  }

  &.next:not(.disabled):hover svg {
    transform: translateX(3px);
  }

  &.disabled {
    opacity: 0.4;
    cursor: not-allowed;
    pointer-events: none;
  }

  &.chapters {
    background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
    border-color: transparent;
    color: white;
    box-shadow: 0 4px 16px rgba(59, 130, 246, 0.3);

    &:hover {
      background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(59, 130, 246, 0.4);
      color: white;
    }
  }
}

.nav-text {
  @include mobile {
    display: none;
  }

  @include tablet {
    display: inline;
  }
}

// Show text on chapters button always
.chapters .nav-text {
  display: inline;
}
</style>
