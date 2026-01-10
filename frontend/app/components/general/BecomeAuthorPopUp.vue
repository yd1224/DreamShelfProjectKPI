<script setup lang="ts">
import type { BecomeAuthorData } from '~/types/auth';

interface Props {
  isOpen?: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'success'): void
}

const props = withDefaults(defineProps<Props>(), {
  isOpen: false
})

const emit = defineEmits<Emits>()

const popupRef = ref<HTMLElement>()
const isSubmitting = ref(false)
const submitError = ref('')

const closePopup = () => {
  if (!isSubmitting.value) {
    emit('close')
  }
}

const handleSubmit = async (formData: BecomeAuthorData) => {
  isSubmitting.value = true
  submitError.value = ''

  try {
    const { $api } = useNuxtApp()
    const auth = useAuthStore()
    const response = await ($api as any).becomeAuthor(formData)
    
    // If the API returns a new access token, update it
    if (response?.accessToken) {
      auth.setAccessToken(response.accessToken)
    }
    
    emit('success')
    emit('close')
  } catch (error: any) {
    console.error('Error becoming author:', error)
    submitError.value = error?.message || 'Failed to register as author. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}

const handleCancel = () => {
  closePopup()
}

// Handle escape key
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && props.isOpen && !isSubmitting.value) {
    closePopup()
  }
}

// Handle click outside
const handleClickOutside = (event: MouseEvent) => {
  if (popupRef.value && !popupRef.value.contains(event.target as Node) && !isSubmitting.value) {
    closePopup()
  }
}

// Prevent body scroll when popup is open
watch(() => props.isOpen, (isOpen) => {
  if (import.meta.client) {
    if (isOpen) {
      useScroll().lockScroll()
      document.addEventListener('keydown', handleKeydown)
    } else {
      useScroll().unlockScroll()
      document.removeEventListener('keydown', handleKeydown)
      submitError.value = ''
    }
  }
})

// Clean up on unmount
onUnmounted(() => {
  if (import.meta.client) {
    useScroll().unlockScroll()
    document.removeEventListener('keydown', handleKeydown)
  }
})
</script>

<template>
  <Teleport to="body">
    <Transition name="popup">
      <div v-if="isOpen" class="popup-overlay" @click="handleClickOutside">
        <div ref="popupRef" class="popup-container" @click.stop>
          <!-- Header -->
          <div class="popup-header">
            <h2 class="popup-title">Become an Author</h2>
            <button
              class="close-button"
              @click="closePopup"
              :disabled="isSubmitting"
              aria-label="Close popup"
            >
              <SpriteSymbol name="close" width="20" height="20" />
            </button>
          </div>

          <!-- Content -->
          <div class="popup-content">
            <p class="popup-description">
              Start your journey as an author on DreamShelf. Share your stories with readers around the world.
            </p>

            <!-- Error Message -->
            <div v-if="submitError" class="submit-error">
              {{ submitError }}
            </div>

            <!-- Form -->
            <BecomeAuthorForm
              :is-submitting="isSubmitting"
              @submit="handleSubmit"
              @cancel="handleCancel"
            />
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped lang="scss">
.popup-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  -webkit-backdrop-filter: blur(4px);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.popup-container {
  background: #ffffff;
  border-radius: 0.75rem;
  box-shadow: 
    0 20px 25px -5px rgba(0, 0, 0, 0.1),
    0 10px 10px -5px rgba(0, 0, 0, 0.04);
  width: 100%;
  max-width: 480px;
  max-height: 90vh;
  overflow-y: auto;
  display: flex;
  flex-direction: column;

  @include media-breakpoint-down(md) {
    width: 100%;
    height: 100%;
  }

  // Custom scrollbar
  &::-webkit-scrollbar {
    width: 6px;
  }

  &::-webkit-scrollbar-track {
    background: #f3f4f6;
    border-radius: 0 0.75rem 0.75rem 0;
  }

  &::-webkit-scrollbar-thumb {
    background: #d1d5db;
    border-radius: 3px;

    &:hover {
      background: #9ca3af;
    }
  }
}

.popup-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  position: sticky;
  top: 0;
  background: #ffffff;
  z-index: 1;
  border-radius: 0.75rem 0.75rem 0 0;
}

.popup-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
}

.close-button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: none;
  background: #f3f4f6;
  border-radius: 6px;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s ease;

  &:hover:not(:disabled) {
    background: #e5e7eb;
    color: #374151;
  }

  &:active:not(:disabled) {
    transform: scale(0.95);
  }

  &:disabled {
    cursor: not-allowed;
    opacity: 0.5;
  }

  svg {
    width: 20px;
    height: 20px;
  }
}

.popup-content {
  padding: 1.5rem;
}

.popup-description {
  font-size: 0.9375rem;
  color: #6b7280;
  margin: 0 0 1.5rem 0;
  line-height: 1.5;
}

.submit-error {
  padding: 0.75rem 1rem;
  background-color: rgba(220, 38, 38, 0.1);
  border: 1px solid rgba(220, 38, 38, 0.3);
  border-radius: 0.5rem;
  color: #dc2626;
  font-size: 0.875rem;
  margin-bottom: 1rem;
}

// Popup transitions
.popup-enter-active,
.popup-leave-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.popup-enter-from,
.popup-leave-to {
  opacity: 0;

  .popup-container {
    transform: scale(0.95) translateY(10px);
  }
}

.popup-enter-active .popup-container,
.popup-leave-active .popup-container {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

// Responsive
@media (max-width: 480px) {
  .popup-overlay {
    padding: 0;
    align-items: flex-end;
  }

  .popup-container {
    border-radius: 0.75rem 0.75rem 0 0;
    max-width: none;
    max-height: none;
  }

  .popup-header {
    border-radius: 0.75rem 0.75rem 0 0;
  }
}
</style>
