<script setup lang="ts">
import type { Chapter } from '~/types/book'

const route = useRoute()
const bookId = route.params.id as string
const chapterId = route.params.chapterId as string

// Fetch book and chapter data
const { bookData } = await useBook(bookId)
const { chapterData } = await useChapter(chapterId, bookId)

// Reading progress tracking
const { saveReadingProgress } = useReadingProgress()

// Save reading progress when chapter data is loaded
watch(() => chapterData.value, (newChapterData) => {
  if (newChapterData && import.meta.client) {
    saveReadingProgress(
      parseInt(bookId),
      chapterId,
      newChapterData.chapterNumber
    )
  }
}, { immediate: true })

// Chapters sidebar state
const isChaptersSidebarOpen = ref(false)

const openChaptersSidebar = () => {
  isChaptersSidebarOpen.value = true
}

const closeChaptersSidebar = () => {
  isChaptersSidebarOpen.value = false
}

// Navigation helpers
const sortedChapters = computed<Chapter[]>(() => {
  if (!bookData.value?.chapters) return []
  return [...bookData.value.chapters].sort((a, b) => a.chapterNumber - b.chapterNumber)
})

const currentChapterIndex = computed(() => {
  return sortedChapters.value.findIndex(ch => ch.uuid === chapterId)
})

const prevChapterId = computed(() => {
  if (currentChapterIndex.value <= 0) return null
  return sortedChapters.value[currentChapterIndex.value - 1]?.uuid || null
})

const nextChapterId = computed(() => {
  if (currentChapterIndex.value >= sortedChapters.value.length - 1) return null
  return sortedChapters.value[currentChapterIndex.value + 1]?.uuid || null
})

// Page meta
useHead({
  title: computed(() => 
    chapterData.value 
      ? `${chapterData.value.title} - ${chapterData.value.bookTitle}` 
      : 'Loading...'
  )
})
</script>

<template>
  <div class="reader-page">
    <template v-if="chapterData && bookData">
      <!-- Reader Header -->
      <ReaderHeader
        :book-title="chapterData.bookTitle"
        :book-cover-url="chapterData.bookCoverImageUrl"
        :chapter-number="chapterData.chapterNumber"
        :chapter-title="chapterData.title"
        :book-id="bookId"
      />

      <!-- Main Content -->
      <main class="reader-main">
        <ReaderContent
          :chapter-number="chapterData.chapterNumber"
          :chapter-title="chapterData.title"
          :html-content="chapterData.htmlContent"
        />

        <!-- Navigation -->
        <ReaderNavigation
          :book-id="bookId"
          :prev-chapter-id="prevChapterId"
          :next-chapter-id="nextChapterId"
          @open-chapters="openChaptersSidebar"
        />
      </main>

      <!-- Chapters Sidebar -->
      <ChaptersSidebar
        :is-open="isChaptersSidebarOpen"
        :book-id="bookId"
        :book-title="bookData.title"
        :book-cover-url="bookData.coverImageUrl"
        :chapters="bookData.chapters"
        :current-chapter-id="chapterId"
        @close="closeChaptersSidebar"
      />
    </template>

    <!-- Loading State -->
    <div v-else class="loading-state">
      <div class="loading-spinner" />
      <span>Loading chapter...</span>
    </div>
  </div>
</template>

<style scoped lang="scss">
.reader-page {
  min-height: 100vh;
  background: linear-gradient(180deg, #111827 0%, #0f172a 100%);
}

.reader-main {
  padding-bottom: 2rem;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  gap: 1rem;
  color: #9ca3af;
  font-size: 1rem;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(59, 130, 246, 0.2);
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
