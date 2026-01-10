<script setup lang="ts">
import type { CreateAuthorData } from '~/types/auth'

interface Author {
  id: number
  displayName: string
}

interface CreateAuthorResponse {
  author: Author
}

const props = withDefaults(defineProps<{
  modelValue: Author[]
  error?: string
}>(), {
  modelValue: () => []
})

const emit = defineEmits<{
  'update:modelValue': [value: Author[]]
}>()

const { $api } = useNuxtApp()

const isOpen = ref(false)
const isCreateOpen = ref(false)
const searchQuery = ref('')
const searchResults = ref<Author[]>([])
const selectedAuthors = computed(() => props.modelValue)
const isSearching = ref(false)
const isCreating = ref(false)
const searchTimeout = ref<ReturnType<typeof setTimeout> | null>(null)

// Create author form data
const createFormData = reactive<CreateAuthorData>({
  firstName: null,
  lastName: null,
  pseudonym: null,
  birthDate: null,
  deathDate: null
})

const createErrors = reactive({
  firstName: '',
  lastName: '',
  pseudonym: '',
  birthDate: '',
  deathDate: '',
  general: ''
})

const openPopup = () => {
  isOpen.value = true
  searchQuery.value = ''
  searchResults.value = []
}

const closePopup = () => {
  isOpen.value = false
  searchQuery.value = ''
  searchResults.value = []
}

const openCreateAuthorPopup = () => {
  isCreateOpen.value = true
  resetCreateForm()
}

const closeCreateAuthorPopup = () => {
  isCreateOpen.value = false
  resetCreateForm()
}

const resetCreateForm = () => {
  createFormData.firstName = null
  createFormData.lastName = null
  createFormData.pseudonym = null
  createFormData.birthDate = null
  createFormData.deathDate = null
  Object.keys(createErrors).forEach(key => {
    createErrors[key as keyof typeof createErrors] = ''
  })
}

const hasRealName = computed(() => 
  createFormData.firstName?.trim() !== '' && createFormData.lastName?.trim() !== ''
)

const hasPseudonym = computed(() => 
  createFormData.pseudonym?.trim() !== ''
)

const validateCreateForm = (): boolean => {
  let isValid = true

  Object.keys(createErrors).forEach(key => {
    createErrors[key as keyof typeof createErrors] = ''
  })

  if (!hasRealName.value && !hasPseudonym.value) {
    createErrors.general = 'Please provide either full name (first and last name) or a pseudonym'
    isValid = false
  }

  if (createFormData.firstName?.trim() && !createFormData.lastName?.trim()) {
    createErrors.lastName = 'Last name is required when first name is provided'
    isValid = false
  }

  if (createFormData.lastName?.trim() && !createFormData.firstName?.trim()) {
    createErrors.firstName = 'First name is required when last name is provided'
    isValid = false
  }

  if (createFormData.firstName?.trim() && createFormData.firstName.trim().length < 2) {
    createErrors.firstName = 'First name must be at least 2 characters'
    isValid = false
  }

  if (createFormData.lastName?.trim() && createFormData.lastName.trim().length < 2) {
    createErrors.lastName = 'Last name must be at least 2 characters'
    isValid = false
  }

  if (createFormData.pseudonym?.trim() && createFormData.pseudonym.trim().length < 2) {
    createErrors.pseudonym = 'Pseudonym must be at least 2 characters'
    isValid = false
  }

  if (createFormData.birthDate) {
    const birthDate = new Date(createFormData.birthDate)
    const today = new Date()
    if (birthDate >= today) {
      createErrors.birthDate = 'Birth date must be in the past'
      isValid = false
    }
  }

  if (createFormData.deathDate) {
    const deathDate = new Date(createFormData.deathDate)
    const today = new Date()
    if (deathDate >= today) {
      createErrors.deathDate = 'Death date must be in the past'
      isValid = false
    }
    if (createFormData.birthDate) {
      const birthDate = new Date(createFormData.birthDate)
      if (deathDate <= birthDate) {
        createErrors.deathDate = 'Death date must be after birth date'
        isValid = false
      }
    }
  }

  return isValid
}

const handleCreateAuthor = async () => {
  if (!validateCreateForm()) {
    return
  }

  isCreating.value = true
  try {
    const dataToSubmit: CreateAuthorData = {
      firstName: createFormData.firstName?.trim() || null,
      lastName: createFormData.lastName?.trim() || null,
      pseudonym: createFormData.pseudonym?.trim() || null,
      birthDate: createFormData.birthDate ? new Date(createFormData.birthDate) : null,
      deathDate: createFormData.deathDate ? new Date(createFormData.deathDate) : null
    }

    const response: CreateAuthorResponse = await $api.createAuthor(dataToSubmit)
    
    // Add the created author to selected authors
    const newAuthor: Author = {
      id: response.author.id,
      displayName: response.author.displayName
    }
    
    selectAuthor(newAuthor)
    closeCreateAuthorPopup()
  } catch (e) {
    console.error('Error creating author:', e)
    createErrors.general = 'Failed to create author. Please try again.'
  } finally {
    isCreating.value = false
  }
}

watch([() => createFormData.firstName, () => createFormData.lastName, () => createFormData.pseudonym], () => {
  if (createErrors.general) {
    createErrors.general = ''
  }
})

const searchAuthors = async () => {
  if (!searchQuery.value.trim()) {
    searchResults.value = []
    return
  }

  isSearching.value = true
  try {
    const results = await $api.searchAuthors(searchQuery.value)
    // Map API response 'name' to 'displayName'
    searchResults.value = results.map((author: any) => ({
      id: author.id,
      displayName: author.name || author.displayName
    }))
  } catch (e) {
    console.error('Error searching authors:', e)
    searchResults.value = []
  } finally {
    isSearching.value = false
  }
}

const onSearchInput = () => {
  if (searchTimeout.value) {
    clearTimeout(searchTimeout.value)
  }
  searchTimeout.value = setTimeout(() => {
    searchAuthors()
  }, 300)
}

const isSelected = (authorId: number) => {
  return props.modelValue.some(a => a.id === authorId)
}

const selectAuthor = (author: Author) => {
  if (!isSelected(author.id)) {
    const newValue = [...props.modelValue, author]
    emit('update:modelValue', newValue)
  }
}

const removeAuthor = (authorId: number) => {
  const newValue = props.modelValue.filter(a => a.id !== authorId)
  emit('update:modelValue', newValue)
}

</script>

<template>
  <div class="form-group">
    <label class="form-label">
      Authors
    </label>

    <div class="author-select">
      <div class="selected-authors">
        <span v-if="modelValue.length === 0" class="placeholder">No authors selected</span>
        <div v-else class="author-tags">
          <span v-for="author in modelValue" :key="author.id" class="author-tag">
            {{ author.displayName }}
            <button type="button" @click="removeAuthor(author.id)" class="remove-author">×</button>
          </span>
        </div>
      </div>
      
      <button type="button" class="add-author-btn" @click="openPopup">
        + Select Author
      </button>
    <button type="button" class="add-author-btn" @click="openCreateAuthorPopup">
        + Create Author
      </button>
    </div>

    <span v-if="error" class="error-message">{{ error }}</span>
  </div>

  <!-- Popup Modal -->
  <Teleport to="body">
    <div v-if="isOpen" class="popup-overlay" @click.self="closePopup">
      <div class="popup-modal">
        <div class="popup-header">
          <h3 class="popup-title">Search Authors</h3>
          <button type="button" class="popup-close" @click="closePopup">×</button>
        </div>

        <div class="popup-body">
          <div class="search-input-wrapper">
            <input
              v-model="searchQuery"
              type="text"
              class="search-input"
              placeholder="Type author name to search..."
              @input="onSearchInput"
            />
            <div v-if="isSearching" class="search-spinner"></div>
          </div>

          <div class="search-results">
            <div v-if="searchQuery && !isSearching && searchResults.length === 0" class="no-results">
              No authors found
            </div>
            <div
              v-for="author in searchResults"
              :key="author.id"
              class="search-result-item"
              :class="{ 'selected': isSelected(author.id) }"
              @click="selectAuthor(author)"
            >
              <span class="author-name">{{ author.displayName }}</span>
              <span v-if="isSelected(author.id)" class="selected-indicator">✓</span>
            </div>
          </div>
        </div>

        <div class="popup-footer">
          <button type="button" class="btn-done" @click="closePopup">Done</button>
        </div>
      </div>
    </div>

    <!-- Create Author Popup -->
    <div v-if="isCreateOpen" class="popup-overlay" @click.self="closeCreateAuthorPopup">
      <div class="popup-modal create-author-modal">
        <div class="popup-header">
          <h3 class="popup-title">Create New Author</h3>
          <button type="button" class="popup-close" @click="closeCreateAuthorPopup">×</button>
        </div>

        <div class="popup-body">
          <form @submit.prevent="handleCreateAuthor" class="create-author-form">
            <!-- General Error -->
            <div v-if="createErrors.general" class="general-error">
              <span class="error-icon">⚠</span>
              {{ createErrors.general }}
            </div>

            <!-- Name Section -->
            <div class="form-section">
              <h4 class="section-title">Author Name</h4>
              <p class="section-description">
                Provide real name or use a pseudonym. At least one is required.
              </p>

              <div class="name-fields">
                <div class="form-group">
                  <label for="createFirstName" class="form-label">First Name</label>
                  <input
                    id="createFirstName"
                    v-model="createFormData.firstName"
                    type="text"
                    class="form-input"
                    :class="{ 'error': createErrors.firstName }"
                    placeholder="Enter first name"
                    maxlength="100"
                  />
                  <span v-if="createErrors.firstName" class="error-message">{{ createErrors.firstName }}</span>
                </div>

                <div class="form-group">
                  <label for="createLastName" class="form-label">Last Name</label>
                  <input
                    id="createLastName"
                    v-model="createFormData.lastName"
                    type="text"
                    class="form-input"
                    :class="{ 'error': createErrors.lastName }"
                    placeholder="Enter last name"
                    maxlength="100"
                  />
                  <span v-if="createErrors.lastName" class="error-message">{{ createErrors.lastName }}</span>
                </div>
              </div>

              <div class="divider">
                <span class="divider-text">or at least</span>
              </div>

              <div class="form-group">
                <label for="createPseudonym" class="form-label">Pseudonym</label>
                <input
                  id="createPseudonym"
                  v-model="createFormData.pseudonym"
                  type="text"
                  class="form-input"
                  :class="{ 'error': createErrors.pseudonym }"
                  placeholder="Enter pen name"
                  maxlength="200"
                />
                <span v-if="createErrors.pseudonym" class="error-message">{{ createErrors.pseudonym }}</span>
              </div>
            </div>

            <!-- Dates Section -->
            <div class="form-section">
              <div class="date-fields">
                <div class="form-group">
                  <label for="createBirthDate" class="form-label">
                    Birth Date <span class="optional">(optional)</span>
                  </label>
                  <input
                    id="createBirthDate"
                    v-model="createFormData.birthDate"
                    type="date"
                    class="form-input"
                    :class="{ 'error': createErrors.birthDate }"
                    :max="new Date().toISOString().split('T')[0]"
                  />
                  <span v-if="createErrors.birthDate" class="error-message">{{ createErrors.birthDate }}</span>
                </div>

                <div class="form-group">
                  <label for="createDeathDate" class="form-label">
                    Death Date <span class="optional">(optional)</span>
                  </label>
                  <input
                    id="createDeathDate"
                    v-model="createFormData.deathDate"
                    type="date"
                    class="form-input"
                    :class="{ 'error': createErrors.deathDate }"
                    :max="new Date().toISOString().split('T')[0]"
                  />
                  <span v-if="createErrors.deathDate" class="error-message">{{ createErrors.deathDate }}</span>
                </div>
              </div>
            </div>

            <!-- Form Actions -->
            <div class="form-actions">
              <button
                type="button"
                class="cancel-button"
                @click="closeCreateAuthorPopup"
                :disabled="isCreating"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="submit-button"
                :disabled="isCreating"
              >
                <span v-if="isCreating">Creating...</span>
                <span v-else>Create Author</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<style scoped lang="scss">
.author-select {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.selected-authors {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background-color: #ffffff;
  min-height: 48px;
}

.placeholder {
  color: #9ca3af;
  font-size: 0.875rem;
}

.author-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.author-tag {
  background-color: #1f2937;
  color: white;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.remove-author {
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

.add-author-btn {
  align-self: flex-start;
  padding: 0.5rem 1rem;
  background-color: #1f2937;
  color: white;
  border: none;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
}

// Popup styles
.popup-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.popup-modal {
  background-color: white;
  border-radius: 0.75rem;
  width: 90%;
  max-width: 500px;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.popup-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.popup-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.popup-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #6b7280;
  cursor: pointer;
  line-height: 1;
  padding: 0;

  &:hover {
    color: #1f2937;
  }
}

.popup-body {
  padding: 1.5rem;
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.search-input-wrapper {
  position: relative;
  margin-bottom: 1rem;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem;
  padding-right: 2.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;

  &:focus {
    outline: none;
    border-color: #1f2937;
    box-shadow: 0 0 0 3px rgba(139, 92, 246, 0.1);
  }

  &::placeholder {
    color: #9ca3af;
  }
}

.search-spinner {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  width: 20px;
  height: 20px;
  border: 2px solid #e5e7eb;
  border-top-color: #1f2937;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: translateY(-50%) rotate(360deg);
  }
}

.search-results {
  flex: 1;
  overflow-y: auto;
  max-height: 300px;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
}

.no-results {
  padding: 2rem;
  text-align: center;
  color: #6b7280;
}

.search-result-item {
  padding: 0.75rem 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  transition: background-color 0.15s;
  border-bottom: 1px solid #f3f4f6;

  &:last-child {
    border-bottom: none;
  }

  &:hover {
    background-color: #f9fafb;
  }

  &.selected {
    background-color: #f3e8ff;
  }
}

.author-name {
  color: #1f2937;
  font-size: 0.875rem;
}

.selected-indicator {
  color: #1f2937;
  font-weight: 600;
}

.popup-footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid #e5e7eb;
  display: flex;
  justify-content: flex-end;
}

.btn-done {
  padding: 0.5rem 1.5rem;
  background-color: #1f2937;
  color: white;
  border: none;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;

  &:hover {
    background-color: #374151;
  }
}

// Create Author Form Styles
.create-author-modal {
  max-height: 90vh;
  overflow-y: auto;
}

.create-author-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.general-error {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background-color: rgba(220, 38, 38, 0.1);
  border: 1px solid rgba(220, 38, 38, 0.3);
  border-radius: 0.5rem;
  color: #dc2626;
  font-size: 0.875rem;
  font-weight: 500;
}

.error-icon {
  font-size: 1rem;
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.section-title {
  font-size: 1rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.section-description {
  font-size: 0.875rem;
  color: #6b7280;
  margin: 0;
}

.name-fields,
.date-fields {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;

  @media (min-width: 480px) {
    grid-template-columns: 1fr 1fr;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.form-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.optional {
  color: #9ca3af;
  font-weight: 400;
}

.form-input {
  width: 100%;
  padding: 0.625rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background-color: #ffffff;
  color: #1f2937;
  font-size: 0.9375rem;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;

  &:focus {
    outline: none;
    border-color: #1f2937;
    box-shadow: 0 0 0 3px rgba(31, 41, 55, 0.1);
  }

  &.error {
    border-color: #dc2626;
    box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
  }

  &::placeholder {
    color: #9ca3af;
  }

  &:disabled {
    background-color: #f3f4f6;
    cursor: not-allowed;
  }
}

.error-message {
  font-size: 0.75rem;
  color: #dc2626;
}

.divider {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin: 0.5rem 0;

  &::before,
  &::after {
    content: '';
    flex: 1;
    height: 1px;
    background-color: #e5e7eb;
  }
}

.divider-text {
  font-size: 0.875rem;
  color: #9ca3af;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.form-actions {
  display: flex;
  flex-direction: column-reverse;
  gap: 0.75rem;
  margin-top: 0.5rem;

  @media (min-width: 480px) {
    flex-direction: row;
    justify-content: flex-end;
  }
}

.cancel-button,
.submit-button {
  padding: 0.625rem 1.25rem;
  border-radius: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;

  &:disabled {
    cursor: not-allowed;
    opacity: 0.6;
  }
}

.cancel-button {
  background-color: transparent;
  border: 1px solid #d1d5db;
  color: #374151;

  &:hover:not(:disabled) {
    background-color: #f3f4f6;
    border-color: #9ca3af;
  }
}

.submit-button {
  background-color: #1f2937;
  border: none;
  color: white;

  &:hover:not(:disabled) {
    background-color: #374151;
  }
}
</style>
