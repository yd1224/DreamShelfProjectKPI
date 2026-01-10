<script setup lang="ts">
import type { FormBookData } from '~/types/book'

if(!useAuthStore().decodedToken.authorId) {
  navigateTo('/login')
}

useSeoMeta({
  title: 'Create Book - DreamShelf',
  description: 'Create a new book in your personal digital library.',
})

const $api = useNuxtApp().$api

const handleCreateBook = async (formData: FormBookData) => {
  // Convert reactive object to FormData
  const formDataToSend = new FormData()
  formDataToSend.append('title', formData.title)
  formDataToSend.append('description', formData.description)

  formDataToSend.append('splitBy', formData.documentHeading)

  if(useAuthStore().decodedToken.role === 'Admin') {
    formData?.authorIds?.forEach(element => {
      formDataToSend.append('authorIds', String(element.id))
    });
  }else{
    formDataToSend.append('authorIds', useAuthStore().decodedToken.authorId)
  }

  // Convert bookStatus name to ID
  const bookStatusId = formData.bookStatus
  if (bookStatusId !== undefined) {
    formDataToSend.append('bookStatus', String(bookStatusId))
  }

  if (formData.documentFile) {
    formDataToSend.append('document', formData.documentFile)
  }


  // Append each genre separately
  formData.genres.forEach(genreId => {
    formDataToSend.append('genres', String(genreId))
  })

  // Append age restriction
  if (formData.ageRestriction !== null) {
    formDataToSend.append('ageRestriction', String(formData.ageRestriction))
  }

  // Only send coverImageUrl (fileName) to the API
  if (formData.coverImageUrl) {
    formDataToSend.append('coverImageUrl', formData.coverImageUrl)
  }

  console.log('Form data:', formData)

  try {
    const response = await $api.createBook(formDataToSend)
    console.log('Book created successfully:', response)
  } catch (error) {
    console.error('Error creating book:', error)
  }
}

const handleDeleteImage = async (uploadedFileName: string)=>{
  useImageUpload().deleteImage([uploadedFileName]);
}
</script>

<template>
  <div class="create-book-page">
    <div class="section">
      <div>
        <div class="page-header">
          <h1 class="page-title">Start Writing Your Book</h1>
          <p class="page-description">Bring your imagination to life. Write, organize, and publish your stories right
            from your digital library.</p>
        </div>
      </div>
      <CreateBookForm :handle-submit="handleCreateBook" :delete-image="handleDeleteImage" />
    </div>
  </div>
</template>

<style scoped lang="scss">
.create-book-page {
  background-color: #f9fafb;
  color: #1f2937;
  min-height: calc(100vh - 200px);
  padding: 2rem 0;
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
</style>