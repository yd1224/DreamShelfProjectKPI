<script setup lang="ts">
const props = defineProps<{
  modelValue: string | null
  error?: string
  deleteImage: (fileName: string) => void | Promise<void>;
}>()

const emit = defineEmits<{
  'update:modelValue': [fileName: string | null]
  'update:error': [error: string]
}>()

const { uploadImage, getImageUrl, validateImageFile } = useImageUpload()
const imagePreviewUrl = ref<string | null>(null)
const uploadedFileName = ref<string | null>(props.modelValue)
const isDragOver = ref(false)
const isUploading = ref(false)

// Initialize preview URL from existing image
watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    uploadedFileName.value = newValue
    imagePreviewUrl.value = getImageUrl(newValue)
  } else {
    uploadedFileName.value = null
    imagePreviewUrl.value = null
  }
}, { immediate: true })

const uploadImageToServer = async (file: File) => {
  try {
    isUploading.value = true
    emit('update:error', '')
    
    const fileName = await uploadImage(file)
    console.log(fileName);
    
    if (fileName) {
      uploadedFileName.value = fileName
      imagePreviewUrl.value = getImageUrl(fileName)
      emit('update:modelValue', fileName)
    } else {
      emit('update:error', 'Failed to upload image. Please try again.')
      imagePreviewUrl.value = null
    }
  } finally {
    isUploading.value = false
  }
}

const handleImageUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    // Validate file first
    const validation = validateImageFile(file)
    if (!validation.isValid) {
      emit('update:error', validation.error || 'Invalid file')
      target.value = ''
      return
    }
    
    await uploadImageToServer(file)
  }
}

const removeImage = async () => {
  if (uploadedFileName.value) {
    await props.deleteImage(uploadedFileName.value)
  }
  
  emit('update:modelValue', null)
  uploadedFileName.value = null
  imagePreviewUrl.value = null
  
  // Reset file input
  const fileInput = document.getElementById('coverImage') as HTMLInputElement
  if (fileInput) {
    fileInput.value = ''
  }
}

// Drag and drop handlers
const handleDragEnter = (e: DragEvent) => {
  e.preventDefault()
  e.stopPropagation()
  isDragOver.value = true
}

const handleDragLeave = (e: DragEvent) => {
  e.preventDefault()
  e.stopPropagation()
  isDragOver.value = false
}

const handleDragOver = (e: DragEvent) => {
  e.preventDefault()
  e.stopPropagation()
}

const handleDrop = async (e: DragEvent) => {
  e.preventDefault()
  e.stopPropagation()
  isDragOver.value = false
  
  const files = e.dataTransfer?.files
  if (files && files.length > 0) {
    const file = files[0]
    
    if (!file) return
    
    // Validate file
    const validation = validateImageFile(file)
    if (!validation.isValid) {
      emit('update:error', validation.error || 'Invalid file')
      return
    }
    
    await uploadImageToServer(file)
  }
}
</script>

<template>
    <!-- Right Column - Image Upload & Preview -->
    <div class="image-section">
        <div class="form-group">
            <label for="coverImage" class="form-label">
                Book Cover Image
            </label>
            <!-- Image Preview -->
            <div v-if="imagePreviewUrl" class="image-preview">
                <div v-if="isUploading" class="upload-overlay">
                    <div class="spinner"></div>
                    <p>Uploading...</p>
                </div>
                <img :src="imagePreviewUrl" alt="Book cover preview" class="preview-image" />
                <button type="button" @click="removeImage" class="remove-image-btn" :disabled="isUploading">
                <SpriteSymbol name="delete" width="24" height="24" />
                </button>
            </div>
            
            <!-- Upload Area -->
            <div v-else class="image-upload-container">
                <input
                id="coverImage"
                type="file"
                accept="image/*"
                @change="handleImageUpload"
                class="image-input"
                :disabled="isUploading"
                />
                <label 
                for="coverImage" 
                class="image-upload-label"
                :class="{ 'drag-over': isDragOver }"
                @dragenter="handleDragEnter"
                @dragleave="handleDragLeave"
                @dragover="handleDragOver"
                @drop="handleDrop"
                >
                <div class="upload-content">
                    <div class="upload-icon">📚</div>
                    <div class="upload-text">
                    <span class="upload-title">
                        {{ isDragOver ? 'Drop image here' : 'Choose book cover' }}
                    </span>
                    <span class="upload-subtitle">
                        {{ isDragOver ? 'Release to upload' : 'PNG, JPG, WEBP up to 5MB' }}
                    </span>
                    </div>
                </div>
                </label>
            </div>
            
            <span v-if="props.error" class="error-message">{{ props.error }}</span>
        </div>
    </div>
</template>

<style scoped lang="scss">
// Image Upload Styles
.image-upload-container {
  position: relative;
}

.image-input {
  position: absolute;
  opacity: 0;
  pointer-events: none;
}

.image-upload-label {
  display: block;
  width: 100%;
  padding: 2rem;
  border: 2px dashed #d1d5db;
  border-radius: 0.5rem;
  background-color: #f9fafb;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    border-color: #9ca3af;
    background-color: #f3f4f6;
  }

  &.drag-over {
    border-color: #3b82f6;
    background-color: #eff6ff;
    transform: scale(1.02);
    box-shadow: 0 4px 12px rgba(59, 130, 246, 0.15);
  }
}

.upload-content {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 1rem;
}

.upload-icon {
  font-size: 2rem;
}

.upload-title {
  display: block;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.25rem;
}

.upload-subtitle {
  display: block;
  font-size: 0.875rem;
  color: #6b7280;
}

// Image Preview Styles
.image-preview {
  text-align: center;
  position: relative;
}

.preview-image {
  max-width: 100%;
  max-height: 300px;
  border-radius: 0.5rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  margin-bottom: 1rem;
}

// Additional styles for ImageUpload component

.remove-image-btn {
  position: absolute;
  right: 10px;
  top: 10px;
  cursor: pointer;
  background-color: rgba(239, 68, 68, 0.9);
  color: white;
  border: none;
  border-radius: 0.375rem;
  padding: 0.5rem;
  transition: all 0.2s ease;
  
  &:hover:not(:disabled) {
    background-color: rgba(220, 38, 38, 0.95);
    transform: scale(1.05);
  }
  
  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
}

.upload-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.9);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
  z-index: 10;
  
  p {
    margin-top: 1rem;
    font-weight: 600;
    color: #3b82f6;
  }
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
