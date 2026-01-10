<script setup lang="ts">
const route = useRoute()
const { $api } = useNuxtApp()

const bookId = computed(() => String(route.params.id))
const chapterId = computed(() => String(route.params.chapterId))

const { chapterData } = await useChapter(chapterId.value, bookId.value, true)

const deletedImages = ref<string[]>([]);
const uploadedImages = ref<string[]>([]);

// Editable chapter state
const editableChapter = ref({
  title: chapterData.value?.title || '',
  htmlContent: chapterData.value?.htmlContent.replaceAll('[PATH]', 'http://localhost:5153/images') || '',
  chapterNumber: chapterData.value?.chapterNumber || 0,
  isPublished: chapterData.value?.isPublished || false
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
    await $api.updateChapter(bookId.value, chapterId.value, {
      title: editableChapter.value.title,
      htmlContent: editableChapter.value.htmlContent.replace(
        /<img\s+([^>]*?)src=["']http:\/\/localhost:5153\/images\/([^"']+)["']([^>]*)>/gi,
        '<img $1src="[PATH]/$2"$3>'
      ),
      chapterNumber: editableChapter.value.chapterNumber,
      isPublished: editableChapter.value.isPublished,
      newImageIds: Array.from(useTextEditor(ref(null)).currentImages.value)
    })

    saveSuccess.value = true

    if (deletedImages.value.length > 0) {
      useImageUpload().deleteImage(deletedImages.value);
      deletedImages.value = [];
    }

    // Hide success message after 3 seconds
    setTimeout(() => {
      saveSuccess.value = false
    }, 3000)
  } catch (error: any) {
    saveError.value = error.message || 'Failed to save changes'
  } finally {
    isSaving.value = false
  }
}

const addToDeletedImages = (fileName: string) => {
  deletedImages.value.push(fileName)
}
</script>

<template>
  <div class="chapter-page">
    <div class="section">
      <ChapterEditorHeader :book-title="chapterData?.bookTitle || ''" :book-cover-url="chapterData?.bookCoverImageUrl"
        :chapter-number="chapterData?.chapterNumber" />

      <ChapterEditorForm v-model="editableChapter" mode="edit" :is-saving="isSaving" :save-success="saveSuccess"
        :save-error="saveError" @save="saveChanges" :delete-image="addToDeletedImages" />
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
