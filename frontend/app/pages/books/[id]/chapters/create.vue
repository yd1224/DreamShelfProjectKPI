<script setup lang="ts">
const route = useRoute()
const { $api } = useNuxtApp()

const bookId = computed(() => String(route.params.id))

const { bookData } = await useBook(bookId.value)

// Editable chapter state
const editableChapter = ref({
  title: '',
  htmlContent: '',
  chapterNumber: 0,
  isPublished: false
})

// Save state
const isSaving = ref(false)
const saveError = ref('')
const saveSuccess = ref(false)

// Save changes
const saveChanges = async () => {
  if (!editableChapter.value.title.trim()) {
    saveError.value = 'Chapter title is required'
    return
  }

  isSaving.value = true
  saveError.value = ''
  saveSuccess.value = false

  try {
    const data = {
      title: editableChapter.value.title,
      htmlContent: editableChapter.value.htmlContent.replace(
        /<img\s+([^>]*?)src=["']http:\/\/localhost:5153\/images\/([^"']+)["']([^>]*)>/gi,
        '<img $1src="[PATH]/$2"$3>'
      ),
      chapterNumber: editableChapter.value.chapterNumber,
      isPublished: editableChapter.value.isPublished,
      newImageIds: Array.from(useTextEditor(ref(null)).currentImages.value)
    }
    await $api.createChapter(bookId.value, data)

    saveSuccess.value = true

    // Hide success message after 3 seconds
    setTimeout(() => {
      saveSuccess.value = false
    }, 3000)
  } catch (error: any) {
    saveError.value = error.message || 'Failed to create chapter'
  } finally {
    isSaving.value = false
  }
}

const handleDeleteImage = async (uploadedFileName: string) => {
  useImageUpload().deleteImage([uploadedFileName]);
}
</script>

<template>
  <div class="chapter-page">
    <div class="section">
      <ChapterEditorHeader :book-title="bookData?.title || ''" :book-cover-url="bookData?.coverImageUrl" />

      <ChapterEditorForm v-model="editableChapter" mode="create" :is-saving="isSaving" :save-success="saveSuccess"
        :save-error="saveError" :delete-image="handleDeleteImage" @save="saveChanges" />
    </div>
  </div>
</template>

<style scoped lang="scss">
.chapter-page {
  background-color: #f9fafb;
  min-height: 100vh;
  padding: 2rem 0;
}
</style>
