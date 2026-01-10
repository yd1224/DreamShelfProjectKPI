<script setup lang="ts">
const props = defineProps<{
  modelValue: File | null
  documentHeading: string
  error?: string
}>()

const emit = defineEmits<{
  'update:modelValue': [file: File | null]
  'update:documentHeading': [heading: string]
  'update:error': [error: string]
}>()

const isDragOver = ref(false)
const fileInfo = ref<{name: string, size: string} | null>(null)

// Watch for file changes to update file info
watch(() => props.modelValue, (newFile) => {
  if (newFile) {
    fileInfo.value = {
      name: newFile.name,
      size: formatFileSize(newFile.size)
    }
  } else {
    fileInfo.value = null
  }
}, { immediate: true })

// Format file size for display
const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// Validate document file
const validateDocument = (file: File): string | null => {
  // Check file type (DOCX)
  const allowedTypes = [
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    '.docx'
  ]
  
  const isValidType = allowedTypes.some(type => 
    file.type === type || file.name.toLowerCase().endsWith('.docx')
  )
  
  if (!isValidType) {
    return 'Please select a valid DOCX file'
  }
  
  // Check file size (10MB limit)
  const maxSize = 10 * 1024 * 1024 // 10MB in bytes
  if (file.size > maxSize) {
    return 'File size must be less than 10MB'
  }
  
  return null
}

const handleDocumentUpload = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (file) {
    const validationError = validateDocument(file)
    if (validationError) {
      emit('update:error', validationError)
      return
    }
    
    emit('update:modelValue', file)
    emit('update:error', '')
  }
}

const removeDocument = () => {
  emit('update:modelValue', null)
  fileInfo.value = null
  // Reset file input
  const fileInput = document.getElementById('documentFile') as HTMLInputElement
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

const handleDrop = (e: DragEvent) => {
  e.preventDefault()
  e.stopPropagation()
  isDragOver.value = false
  
  const files = e.dataTransfer?.files
  if (files && files.length > 0) {
    const file = files[0]
    
    if (file) {
      const validationError = validateDocument(file)
      if (validationError) {
        emit('update:error', validationError)
        return
      }
      
      emit('update:modelValue', file)
      emit('update:error', '')
    }
  }
}

const headings = [
  { id: 'h1', name: 'Heading 1' },
  { id: 'h2', name: 'Heading 2' },
  { id: 'h3', name: 'Heading 3' },
  { id: 'h4', name: 'Heading 4' },
  { id: 'h5', name: 'Heading 5' },
  { id: 'h6', name: 'Heading 6' },
]
</script>

<template>
  <div class="document-section">
    <div class="form-group">
      <label for="documentFile" class="form-label">
        Have you already started writing a book?
      </label>
      <div>
        <div class="form-description">Choose the heading you want to split your book into chapters</div>
        <div>
          <MultiSelect
            :model-value="props.documentHeading"
            @update:model-value="(value) => emit('update:documentHeading', value as string)"
            :options="headings"
            label="Heading"
            placeholder="Select heading"
            :multiple="false"
          />
        </div>
      </div>
      <p class="form-description">
        Upload your DOCX document and we'll automatically detect chapters based on selected heading styles (or "Heading 1" by default).
      </p>
      
      <!-- Document Preview -->
      <div v-if="fileInfo" class="document-preview">
        <div class="document-info">
          <div class="document-icon">📄</div>
          <div class="document-details">
            <span class="document-name">{{ fileInfo.name }}</span>
            <span class="document-size">{{ fileInfo.size }}</span>
          </div>
          <button type="button" @click="removeDocument" class="remove-document-btn">
            <SpriteSymbol name="delete" width="20" height="20" />
          </button>
        </div>
      </div>
      
      <!-- Upload Area -->
      <div v-else class="document-upload-container">
        <input
          id="documentFile"
          type="file"
          accept=".docx,application/vnd.openxmlformats-officedocument.wordprocessingml.document"
          @change="handleDocumentUpload"
          class="document-input"
        />
        <label 
          for="documentFile" 
          class="document-upload-label"
          :class="{ 'drag-over': isDragOver }"
          @dragenter="handleDragEnter"
          @dragleave="handleDragLeave"
          @dragover="handleDragOver"
          @drop="handleDrop"
        >
          <div class="upload-content">
            <div class="upload-icon">📄</div>
            <div class="upload-text">
              <span class="upload-title">
                {{ isDragOver ? 'Drop document here' : 'Choose DOCX document' }}
              </span>
              <span class="upload-subtitle">
                {{ isDragOver ? 'Release to upload' : 'DOCX files up to 10MB' }}
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
.document-section {
  margin-bottom: 1.5rem;
}

.form-description {
  font-size: 0.875rem;
  color: #6b7280;
  margin-bottom: 1rem;
  line-height: 1.5;
}

// Document Upload Styles
.document-upload-container {
  position: relative;
}

.document-input {
  position: absolute;
  opacity: 0;
  pointer-events: none;
}

.document-upload-label {
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

// Document Preview Styles
.document-preview {
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  padding: 1rem;
  background-color: #f9fafb;
}

.document-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.document-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.document-details {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.document-name {
  font-weight: 600;
  color: #374151;
  word-break: break-word;
}

.document-size {
  font-size: 0.875rem;
  color: #6b7280;
}

.remove-document-btn {
  background: none;
  border: none;
  color: #dc2626;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 0.25rem;
  transition: background-color 0.2s ease;
  flex-shrink: 0;

  &:hover {
    background-color: #fee2e2;
  }
}

// Mobile responsiveness
@include media-breakpoint-up(md) {
  .upload-content {
    flex-direction: row;
    gap: 1rem;
  }

  .upload-icon {
    font-size: 2.5rem;
  }
}
</style>
