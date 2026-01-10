<script setup lang="ts">
const route = useRoute()
const authStore = useAuthStore()

const bookId = computed(() => String(route.params.id))

const { bookData } = await useBook(bookId.value)

// Check if current user is an author of this book
const isAuthor = computed(() => {
  if (!bookData.value || !authStore.decodedToken) return false
  return bookData.value.authors.some(author => author.id == authStore.decodedToken.authorId)
})

const totalChapters = computed(() => bookData.value?.chapters?.length || 0)

// SEO Meta
if (bookData.value) {
  useSeoMeta({
    title: `${bookData.value.title} - DreamShelf`,
    description: bookData.value.description?.substring(0, 160),
  })
}
</script>


<template>
  <div class="book-page">
    <div v-if="bookData" class="section">
      <BookHero :book="bookData" :is-author="isAuthor" />

      <div class="content-grid">
        <div class="main-content">
          <BookDescription :description="bookData.description" />
          <BookAuthors :authors="bookData.authors" />
          <BookChapters :chapters="bookData.chapters" :book-id="bookData.id" />
        </div>

        <aside class="sidebar">
          <BookStats :total-chapters="totalChapters" :last-updated-at="bookData.lastUpdatedAt" />
        </aside>
      </div>
    </div>
    <div v-else class="loading-state">
      <p>Loading book details...</p>
    </div>
  </div>
</template>

<style scoped lang="scss">
.book-page {
  background-color: #f9fafb;
  min-height: 100vh;
  padding: 1.5rem 0;
}

.loading-state {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 50vh;

  p {
    font-size: 1.125rem;
    color: #6b7280;
  }
}

.content-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 2rem;
}

.main-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.sidebar {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

@media (min-width: 768px) {
  .book-page {
    padding: 2rem 0;
  }
}

@include media-breakpoint-up(lg) {
  .book-page {
    padding: 3rem 0;
  }

  .content-grid {
    grid-template-columns: 1fr 320px;
    gap: 2.5rem;
  }
}

@include media-breakpoint-up(xl) {
  .content-grid {
    grid-template-columns: 1fr 360px;
  }
}
</style>
