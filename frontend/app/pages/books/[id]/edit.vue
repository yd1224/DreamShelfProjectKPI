<script setup lang="ts">
import type { Chapter, UpdateBookData, FormBookData } from '~/types/book'


const route = useRoute()
const { $api } = useNuxtApp()

// Book ID from route
const bookId = computed(() => String(route.params.id))

const { bookData } = await useBook(bookId.value, true)

const deletedImages = ref<string[]>([]);

// Convert Book to FormBookData for the form
const formBookData = computed<FormBookData | null>(() => {
  if (!bookData.value) return null
  console.log(bookData);

  return {
    title: bookData.value.title,
    description: bookData.value.description,
    coverImageUrl: bookData.value.coverImageUrl || null,
    bookStatus: bookData.value.bookStatus,
    ageRestriction: bookData.value.ageRestriction,
    isPublished: bookData.value.isPublished,
    genres: bookData.value.genres.map(g => g.id),
    documentFile: null,
    authorIds: bookData.value.authors,
    documentHeading: 'h1'
  }
})

// Chapter actions
const handleEditChapter = (chapter: Chapter) => {
  navigateTo(`/books/${bookId.value}/chapters/${chapter.uuid}/edit`)
}

const handleDeleteChapter = async (chapter: Chapter) => {
  try {
    await $api.deleteChapter(bookId.value, chapter.uuid)

   location.reload()
  } catch (error) {
    console.error('Error deleting chapter:', error)
  }
}

// SEO Meta
if (bookData.value) {
  useSeoMeta({
    title: `Edit ${bookData.value.title} - DreamShelf`,
    description: 'Edit your book details and information.',
  })
}

const handleUpdateBook = async (formData: FormBookData) => {
  try {
    console.log("FormData", formData);
    
    // Convert FormBookData to UpdateBookData
    const updateData: UpdateBookData = {
      title: formData.title,
      description: formData.description,
      coverImageUrl: formData.coverImageUrl || '',
      ageRestriction: formData.ageRestriction,
      bookStatus: formData.bookStatus,
      isPublished: formData.isPublished,
      authorIds: formData.authorIds?.map(author => author.id) || [],
      genres: formData.genres.map(id => id)
    }

    await $api.updateBook(bookId.value, updateData)

    useImageUpload().deleteImage(deletedImages.value);
  } catch (error) {
    console.error('Error updating book:', error)
  }
}

const addToDeletedImages = (fileName: string)=>{
  deletedImages.value.push(fileName)
}
</script>

<template>
  <div class="edit-book-page">
    <div class="section">
      <div class="page-header">
        <h1 class="page-title">Edit Your Book</h1>
        <p class="page-description">Update your book's details, cover image, and information.</p>
      </div>
      <CreateBookForm :book-data="formBookData" :is-edit-mode="true" :handle-submit="handleUpdateBook" :delete-image="addToDeletedImages" />

      <div v-if="bookData" class="chapters-section-wrapper">
        <BookChapters :chapters="bookData.chapters" :book-id="bookData.id">
          <template #actions="{ chapter }">
            <div class="chapter-actions">
              <button @click="handleEditChapter(chapter)" class="action-btn edit-btn" title="Edit chapter">
                <SpriteSymbol name="edit" width="18" height="18" />
              </button>
              <button @click="handleDeleteChapter(chapter)" class="action-btn delete-btn" title="Delete chapter">
                <SpriteSymbol name="delete" width="18" height="18" />
              </button>
            </div>
          </template>
        </BookChapters>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.edit-book-page {
  background-color: #f9fafb;
  color: #1f2937;
  min-height: calc(100vh - 200px);
  padding: 2rem 0;
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

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-title {
  font-size: 1.875rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.page-description {
  color: #6b7280;
  font-size: 1rem;
  margin: 0;
}

@media (min-width: 768px) {
  .edit-book-page {
    padding: 2.5rem 0;
  }

  .page-title {
    font-size: 2.25rem;
  }
}

@include media-breakpoint-up(lg) {
  .edit-book-page {
    padding: 3rem 0;
  }

  .page-title {
    font-size: 2.5rem;
  }
}

.chapters-section-wrapper {
  margin-top: 2rem;
}

.chapter-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border: none;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.2s ease;
  background-color: transparent;

  &:hover {
    transform: scale(1.05);
  }

  &:active {
    transform: scale(0.95);
  }
}

.edit-btn {
  color: #3b82f6;

  &:hover {
    background-color: #eff6ff;
  }
}

.delete-btn {
  color: #ef4444;

  &:hover {
    background-color: #fef2f2;
  }
}
</style>
