<script setup lang="ts">
import type { BecomeAuthorData } from '~/types/auth';

interface Props {
  isSubmitting?: boolean
}

interface Emits {
  (e: 'submit', data: BecomeAuthorData): void
  (e: 'cancel'): void
}

const props = withDefaults(defineProps<Props>(), {
  isSubmitting: false
})

const emit = defineEmits<Emits>()

// Form data
const formData = reactive<BecomeAuthorData>({
  firstName: '',
  lastName: '',
  pseudonym: '',
  birthDate: null
})

// Form errors
const errors = reactive({
  firstName: '',
  lastName: '',
  pseudonym: '',
  birthDate: '',
  general: ''
})

// Computed to check if using real name or pseudonym
const hasRealName = computed(() => 
  formData?.firstName?.trim() !== '' && formData?.lastName?.trim() !== ''
)

const hasPseudonym = computed(() => 
  formData?.pseudonym?.trim() !== ''
)

const validateForm = (): boolean => {
  let isValid = true

  // Reset errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })

  // Validate: must have (firstName AND lastName) OR pseudonym
  if (!hasRealName.value && !hasPseudonym.value) {
    errors.general = 'Please provide either your full name (first and last name) or a pseudonym'
    isValid = false
  }

  // If first name is provided, last name is required
  if (formData.firstName?.trim() && !formData.lastName?.trim()) {
    errors.lastName = 'Last name is required when first name is provided'
    isValid = false
  }

  // If last name is provided, first name is required
  if (formData.lastName?.trim() && !formData.firstName?.trim()) {
    errors.firstName = 'First name is required when last name is provided'
    isValid = false
  }

  // Validate first name length if provided
  if (formData.firstName?.trim() && formData.firstName.trim().length < 2) {
    errors.firstName = 'First name must be at least 2 characters'
    isValid = false
  }

  // Validate last name length if provided
  if (formData.lastName?.trim() && formData.lastName.trim().length < 2) {
    errors.lastName = 'Last name must be at least 2 characters'
    isValid = false
  }

  // Validate pseudonym length if provided
  if (formData.pseudonym?.trim() && formData.pseudonym?.trim().length < 2) {
    errors.pseudonym = 'Pseudonym must be at least 2 characters'
    isValid = false
  }

  // Validate birth date if provided (must be in the past)
  if (formData.birthDate) {
    const birthDate = new Date(formData.birthDate)
    const today = new Date()
    if (birthDate >= today) {
      errors.birthDate = 'Birth date must be in the past'
      isValid = false
    }
  }

  return isValid
}

const handleSubmit = () => {
  if (!validateForm()) {
    return
  }

  const dataToSubmit: BecomeAuthorData = {
    firstName: formData.firstName?.trim() || null,
    lastName: formData.lastName?.trim() || null,
    pseudonym: formData.pseudonym?.trim() || null,
    birthDate: formData.birthDate ? new Date(formData.birthDate) : null
  }
  
  emit('submit', dataToSubmit)
}

const handleCancel = () => {
  emit('cancel')
}

// Clear general error when user starts typing
watch([() => formData.firstName, () => formData.lastName, () => formData.pseudonym], () => {
  if (errors.general) {
    errors.general = ''
  }
})
</script>

<template>
  <form @submit.prevent="handleSubmit" class="author-form">
    <!-- General Error -->
    <div v-if="errors.general" class="general-error">
      <span class="error-icon">⚠</span>
      {{ errors.general }}
    </div>

    <!-- Name Section -->
    <div class="form-section">
      <h3 class="section-title">Author Name</h3>
      <p class="section-description">
        Provide your real name or use a pseudonym. At least one is required.
      </p>

      <div class="name-fields">
        <!-- First Name -->
        <div class="form-group">
          <label for="firstName" class="form-label">
            First Name
          </label>
          <input
            id="firstName"
            name="firstName"
            v-model="formData.firstName"
            type="text"
            class="form-input"
            :class="{ 'error': errors.firstName }"
            placeholder="Enter your first name"
            maxlength="100"
          />
          <span v-if="errors.firstName" class="error-message">{{ errors.firstName }}</span>
        </div>

        <!-- Last Name -->
        <div class="form-group">
          <label for="lastName" class="form-label">
            Last Name
          </label>
          <input
            id="lastName"
            name="lastName"
            v-model="formData.lastName"
            type="text"
            class="form-input"
            :class="{ 'error': errors.lastName }"
            placeholder="Enter your last name"
            maxlength="100"
          />
          <span v-if="errors.lastName" class="error-message">{{ errors.lastName }}</span>
        </div>
      </div>

      <!-- Divider -->
      <div class="divider">
        <span class="divider-text">or at least</span>
      </div>

      <!-- Pseudonym -->
      <div class="form-group">
        <label for="pseudonym" class="form-label">
          Pseudonym
        </label>
        <input
          id="pseudonym"
          name="pseudonym"
          v-model="formData.pseudonym"
          type="text"
          class="form-input"
          :class="{ 'error': errors.pseudonym }"
          placeholder="Enter your pen name"
          maxlength="200"
        />
        <span v-if="errors.pseudonym" class="error-message">{{ errors.pseudonym }}</span>
      </div>
    </div>

    <!-- Birth Date Section -->
    <div class="form-section">
      <div class="form-group">
        <label for="birthDate" class="form-label">
          Birth Date <span class="optional">(optional)</span>
        </label>
        <input
          id="birthDate"
          name="birthDate"
          v-model="formData.birthDate"
          type="date"
          class="form-input"
          :class="{ 'error': errors.birthDate }"
          :max="new Date().toISOString().split('T')[0]"
        />
        <span v-if="errors.birthDate" class="error-message">{{ errors.birthDate }}</span>
      </div>
    </div>

    <!-- Form Actions -->
    <div class="form-actions">
      <button
        type="button"
        class="cancel-button"
        @click="handleCancel"
        :disabled="isSubmitting"
      >
        Cancel
      </button>
      <button
        type="submit"
        class="submit-button"
        :disabled="isSubmitting"
      >
        <span v-if="isSubmitting">Submitting...</span>
        <span v-else>Become an Author</span>
      </button>
    </div>
  </form>
</template>

<style scoped lang="scss">
.author-form {
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

.name-fields {
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

  &:active:not(:disabled) {
    transform: scale(0.98);
  }
}

.submit-button {
  background-color: #1f2937;
  border: none;
  color: white;

  &:hover:not(:disabled) {
    background-color: #374151;
  }

  &:active:not(:disabled) {
    transform: scale(0.98);
  }
}
</style>
