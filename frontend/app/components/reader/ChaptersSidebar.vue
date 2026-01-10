<script setup lang="ts">
import type { Chapter } from '~/types/book'

interface Props {
  isOpen: boolean
  bookId: number | string
  bookTitle: string
  bookCoverUrl?: string
  chapters: Chapter[]
  currentChapterId: string
}

interface Emits {
  (e: 'close'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const closeSidebar = () => {
  emit('close')
}

const sortedChapters = computed(() => {
  return [...props.chapters].sort((a, b) => a.chapterNumber - b.chapterNumber)
})

// Prevent body scroll when sidebar is open
watch(() => props.isOpen, (isOpen) => {
  if (import.meta.client) {
    if (isOpen) {
      useScroll().lockScroll()
    } else {
      useScroll().unlockScroll()
    }
  }
})

// Clean up on unmount
onUnmounted(() => {
  if (import.meta.client) {
    useScroll().unlockScroll()
  }
})
</script>

<template>
  <div>
    <!-- Overlay -->
    <Transition name="overlay">
      <div 
        v-if="isOpen" 
        class="sidebar-overlay"
        @click="closeSidebar"
      />
    </Transition>

    <!-- Sidebar Panel -->
    <Transition name="sidebar">
      <div v-if="isOpen" class="chapters-sidebar">
        <!-- Header with close button -->
        <div class="sidebar-header">
          <div class="sidebar-title">Chapters</div>
          <button 
            class="close-button"
            @click="closeSidebar"
            aria-label="Close chapters"
          >
            <SpriteSymbol name="close" width="24" height="24" />
          </button>
        </div>

        <!-- Book Info -->
        <div class="book-info-section">
          <div class="book-cover">
            <img 
              v-if="bookCoverUrl" 
              :src="`http://localhost:5153/images/${bookCoverUrl}`" 
              :alt="bookTitle"
              class="cover-image"
            />
            <div v-else class="cover-placeholder">
              <span>📖</span>
            </div>
          </div>
          <div class="book-details">
            <h3 class="book-title">{{ bookTitle }}</h3>
            <span class="chapters-count">{{ chapters.length }} chapters</span>
          </div>
        </div>

        <!-- Chapters List -->
        <div class="chapters-list">
          <NuxtLink 
            v-for="chapter in sortedChapters" 
            :key="chapter.uuid"
            :to="`/books/${bookId}/chapters/${chapter.uuid}`"
            class="chapter-item"
            :class="{ active: chapter.uuid === currentChapterId }"
            @click="closeSidebar"
          >
            <div class="chapter-number">{{ chapter.chapterNumber }}</div>
            <div class="chapter-info">
              <h4 class="chapter-title">{{ chapter.title }}</h4>
            </div>
            <svg 
              v-if="chapter.uuid === currentChapterId" 
              class="current-indicator" 
              width="16" 
              height="16" 
              viewBox="0 0 16 16" 
              fill="none"
            >
              <circle cx="8" cy="8" r="4" fill="currentColor"/>
            </svg>
          </NuxtLink>
        </div>

        <!-- Back to Book Link -->
        <div class="sidebar-footer">
          <NuxtLink :to="`/books/${bookId}`" class="back-to-book" @click="closeSidebar">
            <svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M11.25 13.5L6.75 9L11.25 4.5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            Back to Book Details
          </NuxtLink>
        </div>
      </div>
    </Transition>
  </div>
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

.sidebar-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4));
  z-index: 998;
  backdrop-filter: blur(4px);
  -webkit-backdrop-filter: blur(4px);
}

.chapters-sidebar {
  position: fixed;
  top: 0;
  right: 0;
  height: 100vh;
  width: 100%;
  max-width: 380px;
  background: linear-gradient(180deg, #1f2937 0%, #111827 100%);
  box-shadow: -8px 0 32px rgba(0, 0, 0, 0.3), -2px 0 8px rgba(0, 0, 0, 0.2);
  z-index: 999;
  display: flex;
  flex-direction: column;
  border-left: 1px solid rgba(75, 85, 99, 0.3);

  @include mobile {
    max-width: 90vw;
  }

  @include tablet {
    max-width: 400px;
  }
}

.sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid rgba(75, 85, 99, 0.2);
  background: rgba(31, 41, 55, 0.8);
  backdrop-filter: blur(8px);
  position: relative;
  flex-shrink: 0;

  &::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 1.5rem;
    right: 1.5rem;
    height: 1px;
    background: linear-gradient(90deg, transparent, #3b82f6, transparent);
  }
}

.sidebar-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #f9fafb;
  letter-spacing: -0.025em;
}

.close-button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border: none;
  background: rgba(75, 85, 99, 0.3);
  border-radius: 8px;
  cursor: pointer;
  color: #d1d5db;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid rgba(75, 85, 99, 0.4);

  &:hover {
    background: rgba(239, 68, 68, 0.2);
    color: #fca5a5;
    border-color: rgba(239, 68, 68, 0.3);
    transform: translateY(-1px);
  }

  svg {
    filter: brightness(0) invert(1);
    opacity: 0.8;
  }
}

.book-info-section {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.25rem 1.5rem;
  background: rgba(17, 24, 39, 0.5);
  border-bottom: 1px solid rgba(75, 85, 99, 0.2);
  flex-shrink: 0;
}

.book-cover {
  width: 56px;
  height: 84px;
  border-radius: 6px;
  overflow: hidden;
  flex-shrink: 0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
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
  background: linear-gradient(135deg, #374151 0%, #1f2937 100%);
  font-size: 1.5rem;
}

.book-details {
  min-width: 0;
  flex: 1;
}

.book-title {
  font-size: 1rem;
  font-weight: 700;
  color: #f9fafb;
  margin: 0 0 0.375rem 0;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  line-height: 1.3;
}

.chapters-count {
  font-size: 0.8125rem;
  color: #9ca3af;
  font-weight: 500;
}

.chapters-list {
  flex: 1;
  overflow-y: auto;
  padding: 0.75rem 0;

  // Custom scrollbar
  &::-webkit-scrollbar {
    width: 4px;
  }

  &::-webkit-scrollbar-track {
    background: rgba(75, 85, 99, 0.1);
  }

  &::-webkit-scrollbar-thumb {
    background: rgba(75, 85, 99, 0.4);
    border-radius: 2px;

    &:hover {
      background: rgba(75, 85, 99, 0.6);
    }
  }
}

.chapter-item {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  padding: 0.875rem 1.5rem;
  text-decoration: none;
  transition: all 0.2s ease;
  border-left: 3px solid transparent;
  position: relative;

  &:hover {
    background: rgba(59, 130, 246, 0.08);
    border-left-color: rgba(59, 130, 246, 0.5);

    .chapter-number {
      background: rgba(59, 130, 246, 0.2);
      border-color: rgba(59, 130, 246, 0.4);
      color: #93c5fd;
    }
  }

  &.active {
    background: linear-gradient(90deg, rgba(59, 130, 246, 0.15), rgba(59, 130, 246, 0.05));
    border-left-color: #3b82f6;

    .chapter-number {
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
      border-color: transparent;
      color: white;
    }

    .chapter-title {
      color: #93c5fd;
    }

    .current-indicator {
      color: #3b82f6;
    }
  }
}

.chapter-number {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: rgba(75, 85, 99, 0.3);
  border: 1px solid rgba(75, 85, 99, 0.4);
  color: #9ca3af;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 0.875rem;
  flex-shrink: 0;
  transition: all 0.2s ease;
}

.chapter-info {
  flex: 1;
  min-width: 0;
}

.chapter-title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #d1d5db;
  margin: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  transition: color 0.2s ease;
}

.current-indicator {
  flex-shrink: 0;
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.sidebar-footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid rgba(75, 85, 99, 0.2);
  background: rgba(31, 41, 55, 0.5);
  flex-shrink: 0;
}

.back-to-book {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #9ca3af;
  text-decoration: none;
  font-size: 0.875rem;
  font-weight: 500;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  transition: all 0.2s ease;
  background: rgba(75, 85, 99, 0.2);
  border: 1px solid rgba(75, 85, 99, 0.3);

  &:hover {
    background: rgba(59, 130, 246, 0.1);
    border-color: rgba(59, 130, 246, 0.3);
    color: #93c5fd;

    svg {
      transform: translateX(-3px);
    }
  }

  svg {
    flex-shrink: 0;
    transition: transform 0.2s ease;
  }
}

// Animations
.overlay-enter-active,
.overlay-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.overlay-enter-from,
.overlay-leave-to {
  opacity: 0;
  backdrop-filter: blur(0px);
}

.sidebar-enter-active,
.sidebar-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.sidebar-enter-from,
.sidebar-leave-to {
  transform: translateX(100%);
  opacity: 0.8;
}
</style>
