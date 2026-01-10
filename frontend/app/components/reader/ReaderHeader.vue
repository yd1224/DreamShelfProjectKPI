<script setup lang="ts">
interface Props {
  bookTitle: string
  bookCoverUrl?: string
  chapterNumber: number
  chapterTitle: string
  bookId: string
}

defineProps<Props>()
</script>

<template>
  <header class="reader-header">
    <NuxtLink :to="`/books/${bookId}`" class="back-link" aria-label="Back to home">
      <SpriteSymbol name="arrow-left" width="20" height="20" />
    </NuxtLink>
    
    <div class="book-info">
      <div class="cover-mini">
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
        <h1 class="book-title">{{ bookTitle }}</h1>
        <span class="chapter-indicator">Chapter {{ chapterNumber }}</span>
      </div>
    </div>
  </header>
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

.reader-header {
  position: sticky;
  top: 0;
  z-index: 100;
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem 1rem;
  background: linear-gradient(180deg, rgba(17, 24, 39, 0.98) 0%, rgba(17, 24, 39, 0.95) 100%);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-bottom: 1px solid rgba(75, 85, 99, 0.3);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);

  @include tablet {
    padding: 1rem 1.5rem;
    gap: 1.25rem;
  }

  @include desktop {
    padding: 1rem 2rem;
  }
}

.back-link {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 10px;
  background: rgba(75, 85, 99, 0.3);
  border: 1px solid rgba(75, 85, 99, 0.4);
  color: #d1d5db;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  flex-shrink: 0;

  &:hover {
    background: rgba(59, 130, 246, 0.2);
    border-color: rgba(59, 130, 246, 0.4);
    color: #93c5fd;
    transform: translateX(-2px);
  }

  svg {
    filter: brightness(0) invert(1);
    opacity: 0.9;
  }
}

.book-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  min-width: 0;
  flex: 1;

  @include tablet {
    gap: 1rem;
  }
}

.cover-mini {
  width: 36px;
  height: 54px;
  border-radius: 4px;
  overflow: hidden;
  flex-shrink: 0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);

  @include tablet {
    width: 44px;
    height: 66px;
    border-radius: 6px;
  }
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
  font-size: 1rem;

  @include tablet {
    font-size: 1.25rem;
  }
}

.book-details {
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.book-title {
  font-size: 0.9375rem;
  font-weight: 700;
  color: #f9fafb;
  margin: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  line-height: 1.3;

  @include tablet {
    font-size: 1.125rem;
  }

  @include desktop {
    font-size: 1.25rem;
  }
}

.chapter-indicator {
  font-size: 0.75rem;
  font-weight: 500;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 0.05em;

  @include tablet {
    font-size: 0.8125rem;
  }
}
</style>
