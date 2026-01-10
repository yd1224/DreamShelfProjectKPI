<script setup lang="ts">
interface Props {
  bookTitle: string
  bookCoverUrl?: string
  chapterNumber?: number
}

defineProps<Props>()
</script>

<template>
  <div class="book-info-header">
    <div class="book-meta">
      <div class="book-cover" v-if="bookCoverUrl">
        <img :src="`http://localhost:5153/images/${bookCoverUrl}`" :alt="bookTitle" />
      </div>
      <div class="book-details">
        <h1 class="book-title">{{ bookTitle }}</h1>
        <div v-if="chapterNumber" class="chapter-badge">
          <span class="badge">Chapter {{ chapterNumber }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.book-info-header {
  background: white;
  border-radius: 0.75rem;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.book-meta {
  display: flex;
  gap: 1.5rem;
  align-items: flex-start;
}

.book-cover {
  flex-shrink: 0;
  width: 80px;
  height: 120px;
  border-radius: 0.5rem;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}

.book-details {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.book-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
  line-height: 1.3;
}

.chapter-badge {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.75rem;
  background: #dbeafe;
  color: #1e40af;
  border-radius: 9999px;
  font-size: 0.875rem;
  font-weight: 600;
}

@media (min-width: 768px) {
  .book-title {
    font-size: 1.875rem;
  }
}

@media (max-width: 767px) {
  .book-meta {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  .book-cover {
    width: 100px;
    height: 150px;
  }
}
</style>
