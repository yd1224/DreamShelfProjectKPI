<script setup lang="ts">
import type { FormBookData, Author } from '~/types/book'

interface Props {
  bookData?: FormBookData | null;
  isEditMode?: boolean;
  handleSubmit?: (data: FormBookData) => void;
  deleteImage: (fileName: string) => void | Promise<void>;
}

const props = withDefaults(defineProps<Props>(), {
  bookData: null,
  isEditMode: false,
  handleSubmit: () => {},
  deleteImage: async () => {}
});


const { $api } = useNuxtApp()
const router = useRouter()
const authStore = useAuthStore()

const isAdmin = computed(() => authStore.decodedToken?.role === 'Admin')

// Available book statuses for selection
const availableBookStatuses = useConstants().bookStatus

// Form data - initialize with existing book data if in edit mode
const formData: FormBookData = reactive({
  title: props.bookData?.title || '',
  description: props.bookData?.description || '',
  genres: props.bookData?.genres || [],
  ageRestriction: props.bookData?.ageRestriction ?? null,
  coverImageUrl: props.bookData?.coverImageUrl || null,
  documentFile: null,
  documentHeading: 'h1',
  isPublished: props.bookData?.isPublished || false,
  bookStatus: props.bookData?.bookStatus || '',
  authorIds: props.bookData?.authorIds || [],
})

// Available genres for selection
const availableGenres = await useAsyncData('genres', async () => (await $api.genres()))


// Form state
const isSubmitting = ref(false)
const errors = reactive({
  title: '',
  description: '',
  genres: '',
  ageRestriction: '',
  coverImageUrl: '',
  documentFile: '',
  bookStatus: '',
})

const validateForm = () => {
  let isValid = true

  // Reset errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })

  // Title validation
  if (!formData.title.trim()) {
    errors.title = 'Title is required'
    isValid = false
  } else if (formData.title.length < 2) {
    errors.title = 'Title must be at least 2 characters long'
    isValid = false
  }

  // Description validation
  if (!formData.description.trim()) {
    errors.description = 'Description is required'
    isValid = false
  } else if (formData.description.length < 10) {
    errors.description = 'Description must be at least 10 characters long'
    isValid = false
  }

  // Genre validation
  if (formData.genres.length === 0) {
    errors.genres = 'Please select at least one genre'
    isValid = false
  }

  // Age restriction validation
  if (formData.ageRestriction === null || formData.ageRestriction === undefined) {
    errors.ageRestriction = 'Please select an age restriction'
    isValid = false
  }

  return isValid
}

const submitForm = async () => {
  if (!validateForm()) {
    return
  }

  isSubmitting.value = true

  try {
    props.handleSubmit(formData)
  } catch (error) {
    console.error(`Error ${props.isEditMode ? 'updating' : 'creating'} book:`, error)
  } finally {
    isSubmitting.value = false
  }
}

const toolbarOptions = {
  bold: true,
  italic: true,
  underline: true,
  strikethrough: true,
  alignment: true,
  orderedList: true,
  bulletList: true,
  link: true,
  htmlMode: true,
  image: false,
}

const ages = useConstants().ages

const isBookEditPage = computed(() => /^\/books\/\d+\/edit$/.test(router.currentRoute.value.path))
</script>

<template>
  <form @submit.prevent="submitForm" class="book-form">
    <div class="form-layout">
      <!-- Left Column - Form Fields -->
      <div class="form-fields">
        <!-- Title Field -->
        <div class="form-group">
          <label for="title" class="form-label">
            Book Title <span class="required">*</span>
          </label>
          <input id="title" v-model="formData.title" type="text" class="form-input" :class="{ 'error': errors.title }"
            placeholder="Enter the book title" maxlength="200" />
          <span v-if="errors.title" class="error-message">{{ errors.title }}</span>
        </div>

        <div v-if="isEditMode && isBookEditPage" class="form-group" style="display: flex; align-items: center; gap: 1rem">
          <label for="chapter-title" class="form-label" style="margin-bottom: 0;">Publish immediately</label>
          <input id="chapter-title" type="checkbox" v-model="formData.isPublished" />
        </div>

        <MultiSelect label="Book Status" :options="availableBookStatuses" v-model="formData.bookStatus"
          placeholder="Select book status" :error="errors.bookStatus" :multiple="false" />

        <!-- Author Select - Admin Only -->
        <AuthorSelect v-if="isAdmin" v-model="formData.authorIds" />

        <!-- Genre Multi-Select -->
        <MultiSelect label="Genres" :options="availableGenres.data.value || []" v-model="formData.genres"
          placeholder="Select genres" :error="errors.genres" required />

        <!-- Age Restriction Single-Select -->
        <MultiSelect label="Age Restriction" :options="ages" v-model="formData.ageRestriction"
          placeholder="Select age restriction" :error="errors.ageRestriction" :multiple="false" required />

        <!-- Description Field -->
        <DescriptionEditor v-model="formData.description" :error="errors.description"
          @update:error="errors.description = $event" :toolbarOptions="toolbarOptions" />

        <!-- Document Upload - Only show in create mode -->
        <DocumentUpload v-if="!isEditMode" v-model:modelValue="formData.documentFile"
          v-model:documentHeading="formData.documentHeading" :error="errors.documentFile"
          @update:error="errors.documentFile = $event" />
      </div>
      <ImageUpload v-model="formData.coverImageUrl" :error="errors.coverImageUrl"
        @update:error="errors.coverImageUrl = $event" :deleteImage="deleteImage" />
    </div>

    <!-- Submit Button -->
    <div class="form-actions">
      <button type="submit" class="submit-button" :disabled="isSubmitting">
        <span v-if="isSubmitting">{{ isEditMode ? 'Updating Book...' : 'Creating Book...' }}</span>
        <span v-else>{{ isEditMode ? 'Update Book' : 'Create Book' }}</span>
      </button>
    </div>
  </form>
  <!-- Backdrop for closing dropdown -->
</template>

<style scoped lang="scss">
.book-form {
  background-color: #ffffff;
  border-radius: 0.75rem;
  padding: 1.5rem;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
  border: 1px solid #e5e7eb;
}

.form-layout {
  max-width: 100%;
  display: grid;
  grid-template-columns: 1fr;
  gap: 2rem;
}

::v-deep(.form-group) {
  margin-bottom: 1.5rem;
}

::v-deep(.form-label) {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
}

::v-deep(.required) {
  color: #dc2626;
}

.form-input,
.form-textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background-color: #ffffff;
  color: #1f2937;
  font-size: 1rem;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;

  &:focus {
    outline: none;
    border-color: #3b82f6;
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  }

  &.error {
    border-color: #dc2626;
    box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
  }

  &::placeholder {
    color: #9ca3af;
  }
}

.form-textarea {
  resize: vertical;
  min-height: 100px;
}

::v-deep(.character-count) {
  display: block;
  font-size: 0.75rem;
  color: #6b7280;
  text-align: right;
  margin-top: 0.25rem;
}

::v-deep(.error-message) {
  display: block;
  font-size: 0.75rem;
  color: #dc2626;
  margin-top: 0.25rem;
}

// Genre Selector Styles
.genre-selector {
  position: relative;
}

.genre-dropdown-trigger {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background-color: #ffffff;
  color: #1f2937;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    border-color: #9ca3af;
  }

  &.active {
    border-color: #3b82f6;
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  }

  &.error {
    border-color: #dc2626;
    box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
  }
}

.selected-genres {
  flex: 1;
}

.placeholder {
  color: #9ca3af;
  font-size: 1rem;
}

.genre-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.genre-tag {
  background-color: #3b82f6;
  color: white;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.remove-genre {
  background: none;
  border: none;
  color: white;
  cursor: pointer;
  font-size: 1rem;
  line-height: 1;
  padding: 0;

  &:hover {
    opacity: 0.7;
  }
}

.dropdown-arrow {
  transition: transform 0.2s ease;

  &.open {
    transform: rotate(180deg);
  }
}

.genre-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background-color: #ffffff;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  margin-top: 0.25rem;
  max-height: 200px;
  overflow-y: auto;
  z-index: 50;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

.genre-option {
  padding: 0.75rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: background-color 0.2s ease;

  &:hover {
    background-color: #f3f4f6;
  }

  &.selected {
    background-color: #3b82f6;
    color: white;
  }
}

.genre-checkbox {
  width: 1rem;
  text-align: center;
  font-weight: bold;
}

.selected-count {
  font-size: 0.75rem;
  color: #6b7280;
  margin-top: 0.25rem;
}

.dropdown-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 40;
}

// Submit Button
.form-actions {
  margin-top: 2rem;
  text-align: center;

  @include media-breakpoint-up(lg) {
    text-align: left;
  }
}

.submit-button {
  background-color: #1f2937;
  color: white;
  border: none;
  padding: 0.75rem 2rem;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.1s ease;
  min-width: 150px;

  &:active:not(:disabled) {
    transform: translateY(0);
  }

  &:disabled {
    background-color: #9ca3af;
    cursor: not-allowed;
    transform: none;
  }
}

// Tablet styles (768px and up)
@include media-breakpoint-up(md) {
  .page-title {
    font-size: 2.25rem;
  }

  .book-form {
    padding: 2rem;
  }

  ::v-deep(.form-group) {
    margin-bottom: 2rem;
  }

  .upload-content {
    flex-direction: row;
    gap: 1rem;
  }

  .upload-icon {
    font-size: 2.5rem;
  }
}

// Desktop styles (992px and up)
@include media-breakpoint-up(lg) {
  .page-title {
    font-size: 2.5rem;
  }

  .book-form {
    padding: 2.5rem;
  }

  .form-layout {
    grid-template-columns: 2fr 1fr;
    gap: 3rem;
    align-items: start;
  }

  .preview-image {
    max-height: 400px;
  }

  .form-input,
  .form-textarea {
    font-size: 1.125rem;
  }

  .submit-button {
    padding: 1rem 2.5rem;
    font-size: 1.125rem;

  }
}
</style>
