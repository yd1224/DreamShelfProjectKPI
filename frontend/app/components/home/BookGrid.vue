<script setup lang="ts">
// @ts-ignore
import { Splide, SplideSlide } from '@splidejs/vue-splide'
import '@splidejs/vue-splide/css'

interface Book {
  id: number
  title: string
  genres: string[]
  authors: string[]
  coverImageUrl: string | null
}

interface Props {
  title: string
  subtitle: string
  books: Book[]
  viewAllLink: string
  showNewBadges?: boolean
  backgroundClass?: string
}

defineProps<Props>()

const splideOptions = {
  type: 'slide' as const,
  autoplay: false,
  perPage: 5,
  perMove: 1,
  gap: '1.5rem',
  pagination: false,
  arrows: true,
  pauseOnHover: true,
  rewind: true,
  breakpoints: {
    480: {
      perPage: 2,
      gap: '1rem',
    },
    768: {
      perPage: 3,
      gap: '1.25rem',
    },
    1024: {
      perPage: 4,
      gap: '1.5rem',
    },
  },
}
</script>

<template>
  <section class="section-container" :class="backgroundClass">
    <div class="section">
      <div class="section-header">
        <h2 class="section-title">{{ title }}</h2>
        <p class="section-subtitle">{{ subtitle }}</p>
      </div>
      <div class="books-slider">
        <Splide :options="splideOptions" class="splide">
          <SplideSlide v-for="book in books" :key="book.id">
            <div class="book-card">
              <NuxtLink :to="`/books/${book.id}`" class="book-link-wrapper">
                <div class="book-cover">
                  <div v-if="showNewBadges" class="new-badge">NEW</div>
                  <img 
                    v-if="book.coverImageUrl" 
                    :src="`${useRuntimeConfig().public.apiBaseUrl}/images/${book.coverImageUrl}`" 
                    :alt="book.title" 
                    class="cover-image" 
                  />
                  <div v-else class="cover-image-placeholder">
                    📚
                  </div>
                </div>
                <div class="book-info">
                  <div class="book-content">
                    <h3 class="book-title">{{ book.title }}</h3>
                    <p class="book-author" v-if="book.authors.length > 0">by {{ book.authors.join(', ') }}</p>
                    <p class="book-author" v-else>by Unknown Author</p>
                  </div>
                  <div class="genres">
                    <div class="book-genre" v-for="genre in book.genres" :key="genre">{{ genre }}</div>
                  </div>
                </div>
              </NuxtLink>
            </div>
          </SplideSlide>
        </Splide>
      </div>
    </div>
  </section>
</template>

<style scoped lang="scss">

.section-container {
  padding: 3rem 0;

  @media (min-width: 768px) {
    padding: 4rem 0;
  }
}

.section-header {
  text-align: center;
  margin-bottom: 2rem;

  @media (min-width: 768px) {
    margin-bottom: 2.5rem;
  }
}

.section-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;

  @media (min-width: 768px) {
    font-size: 2rem;
  }
}

.section-subtitle {
  font-size: 0.95rem;
  color: #6b7280;
  margin-bottom: 0;
}

.view-all-link {
  color: #3b82f6;
  font-weight: 600;
  text-decoration: none;
  transition: color 0.3s ease;

  &:hover {
    color: #2563eb;
  }
}

.books-slider {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1rem;

  @media (min-width: 768px) {
    padding: 0 2rem;
  }
}

// Splide customization
.splide {
  &__slide {
    display: flex !important;
    height: auto;
  }

  &__arrow {
    background: white;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    width: 2.5rem;
    height: 2.5rem;
    
    &:hover {
      background: #f9fafb;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }

    svg {
      fill: #374151;
    }
  }

  &__arrow--prev {
    left: -0.5rem;

    @media (min-width: 768px) {
      left: 0.5rem;
    }
  }

  &__arrow--next {
    right: -0.5rem;

    @media (min-width: 768px) {
      right: 0.5rem;
    }
  }

  &__track {
    padding: 0.5rem 0;
  }

  &__list {
    align-items: stretch;
  }
}

.book-card {
  background: white;
  border-radius: 0.5rem;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  border: 1px solid #e5e7eb;
  transition: all 0.2s ease;
  height: 100%;
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 260px;
  margin: 0 auto;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  }
}

.book-cover {
  position: relative;
  width: 100%;
  height: 200px;
  overflow: hidden;
}

.cover-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
}

.cover-image-placeholder {
  width: 100%;
  height: 100%;
  background-color: #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #9ca3af;
  font-size: 2.5rem;
}

.book-link-wrapper {
  text-decoration: none;
  color: inherit;
  display: block;
  height: 100%;
}

.book-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.overlay-btn {
  background: white;
  color: #1f2937;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;

  &:hover {
    background: #3b82f6;
    color: white;
  }
}

.new-badge {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  background: #ef4444;
  color: white;
  padding: 0.3125rem 0.625rem;
  border-radius: 0.25rem;
  font-size: 0.6875rem;
  font-weight: 600;
  z-index: 10;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
}

.book-info {
  padding: 1rem;
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: space-between;

  @media (min-width: 768px) {
    padding: 1.25rem;
  }
}

.book-content {
  flex: 1;
}

.book-title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.375rem;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;

  @media (min-width: 768px) {
    font-size: 1rem;
  }
}

.book-author {
  color: #6b7280;
  margin-bottom: 0.75rem;
  font-size: 0.8125rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.book-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.rating {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.stars {
  font-size: 1rem;
}

.rating-value {
  font-weight: 600;
  color: #1f2937;
}

.price {
  font-weight: 700;
  color: #3b82f6;
  font-size: 1.125rem;
}

.book-genre {
  background: #f3f4f6;
  color: #374151;
  padding: 0.1875rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.6875rem;
  font-weight: 500;
  display: inline-block;
}

.genres {
  display: flex;
  gap: 0.375rem;
  flex-wrap: wrap;
}
</style>
