<script setup lang="ts">
interface ChapterFormData {
  title: string
  htmlContent: string
  chapterNumber: number
  isPublished: boolean
}

interface Props {
  modelValue: ChapterFormData
  mode: 'create' | 'edit'
  isSaving?: boolean
  saveSuccess?: boolean
  saveError?: string
  deleteImage: (fileName: string) => void | Promise<void>;
}

interface Emits {
  (e: 'update:modelValue', value: ChapterFormData): void
  (e: 'save'): void
}

const props = withDefaults(defineProps<Props>(), {
  isSaving: false,
  saveSuccess: false,
  saveError: '',
  deleteImage: async () => {}
})

const emit = defineEmits<Emits>()

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
  image: true
}

const updateField = <K extends keyof ChapterFormData>(field: K, value: ChapterFormData[K]) => {
  emit('update:modelValue', {
    ...props.modelValue,
    [field]: value
  })
}

const headerTitle = computed(() => props.mode === 'create' ? 'Create Chapter' : 'Edit Chapter')
const buttonText = computed(() => props.mode === 'create' ? 'Create Chapter' : 'Save Changes')
</script>

<template>
  <div class="editor-section">
    <div class="editor-header">
      <h2>{{ headerTitle }}</h2>
      <div class="editor-actions">
        <button 
          @click="$emit('save')" 
          :disabled="isSaving"
          class="btn-save"
        >
          <SpriteSymbol v-if="!isSaving" name="save" width="18" height="18" />
          <span v-if="isSaving">Saving...</span>
          <span v-else>{{ buttonText }}</span>
        </button>
      </div>
    </div>

    <!-- Success/Error Messages -->
    <div v-if="saveSuccess" class="alert alert-success">
      <SpriteSymbol name="check-circle" width="20" height="20" />
      <span>Changes saved successfully!</span>
    </div>
    <div v-if="saveError" class="alert alert-error">
      <SpriteSymbol name="alert-circle" width="20" height="20" />
      <span>{{ saveError }}</span>
    </div>

    <!-- Chapter Title Input -->
    <div class="form-group">
      <label for="chapter-title" class="form-label">Chapter Title</label>
      <input 
        id="chapter-title"
        :value="modelValue.title"
        @input="updateField('title', ($event.target as HTMLInputElement).value)"
        type="text" 
        class="form-input"
        placeholder="Enter chapter title"
        maxlength="200"
      />
    </div>

    <!-- Chapter Number Input -->
    <div class="form-group">
      <label for="chapter-number" class="form-label">Chapter Number</label>
      <input 
        id="chapter-number"
        :value="modelValue.chapterNumber"
        @input="updateField('chapterNumber', Number(($event.target as HTMLInputElement).value))"
        type="number" 
        class="form-input"
        placeholder="Enter chapter number"
      />
    </div>

    <!-- Publish Checkbox -->
    <div class="form-group form-group-inline">
      <label for="chapter-published" class="form-label">Publish immediately</label>
      <input 
        id="chapter-published"
        :checked="modelValue.isPublished"
        @change="updateField('isPublished', ($event.target as HTMLInputElement).checked)"
        type="checkbox" 
      />
    </div>

    <!-- Chapter Content Editor -->
    <div class="form-group">
      <label class="form-label">Chapter Content</label>
      <CustomTextEditor 
        :modelValue="modelValue.htmlContent"
        @update:modelValue="updateField('htmlContent', $event)"
        :toolbarOptions="toolbarOptions" 
        :delete-image="deleteImage"
      />
    </div>
  </div>
</template>

<style scoped lang="scss">
.editor-section {
  background: white;
  border-radius: 0.75rem;
  padding: 2rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e5e7eb;

  h2 {
    font-size: 1.25rem;
    font-weight: 600;
    color: #1f2937;
    margin: 0;
  }
}

.editor-actions {
  display: flex;
  gap: 0.75rem;
}

.btn-save {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;

  &:hover:not(:disabled) {
    background: #2563eb;
    transform: translateY(-1px);
    box-shadow: 0 4px 6px rgba(59, 130, 246, 0.3);
  }

  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
}

.alert {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
  margin-bottom: 1.5rem;
  font-size: 0.875rem;
  font-weight: 500;

  &.alert-success {
    background: #d1fae5;
    color: #065f46;
    border: 1px solid #6ee7b7;
  }

  &.alert-error {
    background: #fee2e2;
    color: #991b1b;
    border: 1px solid #fca5a5;
  }
}

.form-group {
  margin-bottom: 1.5rem;

  &.form-group-inline {
    display: flex;
    align-items: center;
    gap: 1rem;

    .form-label {
      margin-bottom: 0;
    }
  }
}

.form-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  color: #1f2937;
  transition: all 0.2s;

  &:focus {
    outline: none;
    border-color: #3b82f6;
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  }

  &::placeholder {
    color: #9ca3af;
  }
}

:deep(.editor-content) {
  max-height: none !important;
  min-height: 500px;
  max-width: 100%;
  overflow-wrap: break-word;
  word-break: break-word;
}

@media (min-width: 768px) {
  .editor-section {
    padding: 2.5rem;
  }
}

@media (max-width: 767px) {
  .editor-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .editor-actions {
    width: 100%;

    .btn-save {
      width: 100%;
      justify-content: center;
    }
  }
}
</style>
