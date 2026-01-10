<script setup lang="ts">
const $api = useNuxtApp().$api
const { getReadingProgress } = useReadingProgress()

const { data: newBooks } = await useAsyncData('newBooks', async () => await $api.getNewBooks())

// Fetch continue reading books
const continueReadingBooks = ref<any[]>([])
const readingProgressData = ref<any[]>([])

if (import.meta.client) {
  const progress = getReadingProgress()
  readingProgressData.value = progress
  
  if (progress.length > 0) {
    const bookIds = progress.map(p => p.bookId)
    try {
      const books = await $api.getBooks(bookIds)
      continueReadingBooks.value = books.map((book: any) => {
        const progressInfo = progress.find(p => p.bookId === book.id)
        return {
          ...book,
          continueReading: progressInfo
        }
      })
    } catch (error) {
      console.error('Error fetching continue reading books:', error)
    }
  }
}

const stats = ref([
  { label: "Books Available", value: "50,000+", icon: "📖" },
  { label: "Happy Readers", value: "25,000+", icon: "👥" },
  { label: "Authors", value: "5,000+", icon: "✍️" },
  { label: "Countries", value: "120+", icon: "🌍" }
])
</script>

<template>
  <div class="home-page">
    <HeroSection />
    <ContinueReading 
      v-if="continueReadingBooks.length > 0"
      :books="continueReadingBooks"
    />
    <StatsSection :stats="stats" />
    <BookGrid 
      v-if="newBooks && newBooks.length > 0"
      title="New Arrivals"
      subtitle="Fresh additions to our collection"
      :books="newBooks"
      view-all-link="/books?sort=newest"
      :show-new-badges="true"
      background-class="new-books"
    />
    <CallToAction />
  </div>
</template>

<style scoped lang="scss">
.home-page {
  min-height: 100vh;
}
</style>
