<script setup lang="ts">
import type { ToolbarOptions } from '@/types/editor'

const props = defineProps<{
  modelValue: string
  error?: string
  toolbarOptions?: ToolbarOptions
  deleteImage?: (fileName: string) => void | Promise<void>;
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const { uploadImage, getImageUrl, validateImageFile } = useImageUpload()
const editorRef = ref<HTMLDivElement | null>(null)
const showLinkPopup = ref(false)
const linkUrl = ref('')
const showAlignmentPicker = ref(false)
const savedSelection = ref<Range | null>(null)
const imageInputRef = ref<HTMLInputElement | null>(null)
const paragraphTag = 'div'

const { handleBeforeInput, handleKeyDown, insertList, toggleTextStyle, toggleAlignment, updateActiveFormats, activeFormats, insertLink, insertImageAtSelection, toggleHtmlMode, htmlSource, handlePaste, initializeImageTracking, checkForDeletedImages, onImageDelete, trackImage } = useTextEditor(editorRef)

// Setup image deletion callback
onImageDelete(async (fileNames: string[]) => {
  console.log('Images deleted from editor:', fileNames);

  for (const fileName of fileNames) {
    await props?.deleteImage?.(fileName);
  }
})

// Initialize editor with modelValue
onMounted(() => {
  if (editorRef.value) {
    // Set initial content if modelValue is provided
    if (props.modelValue) {
      editorRef.value.innerHTML = props.modelValue;
    } else {
      const initialP = document.createElement(paragraphTag);
      editorRef.value.appendChild(initialP);
    }
    
    // Initialize image tracking after content is set
    initializeImageTracking();
  }
  
  // Add click outside listener to close dropdowns
  document.addEventListener('click', handleClickOutside);
  document.addEventListener('selectionchange', updateActiveFormats);
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

const updateModelValue = () => {
  console.log("updateModelValue");
  
  if (editorRef.value?.innerHTML !== props.modelValue) {
    emit('update:modelValue', editorRef.value?.innerHTML || '');
  }
}

const handleInput = (event: Event) => {
  console.log("handleInput");
  
  if (editorRef.value) {
    const html = editorRef.value.innerHTML;
    console.log(html, props.modelValue);
    
    // Check for deleted images
    checkForDeletedImages();
    console.log("INPUT");
    
    updateModelValue();
  }
}

const handleBeforeInputWrapper = (event: InputEvent) => {
  handleBeforeInput(event, () => {
    // After text insertion, update modelValue
    if (editorRef.value) {
      updateModelValue();
    }
  });
}

const handlePasteWrapper = (event: ClipboardEvent) => {
  handlePaste(event,() => {
    // After text insertion, update modelValue
    if (editorRef.value) {
      updateModelValue();
    }
  } )
}

// Handle click outside to close dropdowns
const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement

  // Close alignment picker if clicked outside
  if (showAlignmentPicker.value && !target.closest('.alignment-picker-wrapper')) {
    showAlignmentPicker.value = false
  }

  // Close link popup if clicked outside
  if (showLinkPopup.value && !target.closest('.link-popup-wrapper')) {
    showLinkPopup.value = false
  }
}

const openLinkPopup = () => {
  const selection = window.getSelection()
  if (selection && selection.toString() && selection.rangeCount > 0) {
    // Save the current selection
    savedSelection.value = selection.getRangeAt(0).cloneRange()
    showLinkPopup.value = true
  } else {
    alert('Please select text to create a link')
  }
}

const cancelLink = () => {
  showLinkPopup.value = false
  linkUrl.value = ''
  savedSelection.value = null
}

const openImageUpload = () => {
  imageInputRef.value?.click()
}

const handleImageUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];

  if (!file) return;

  // Validate file
  const validation = validateImageFile(file);
  if (!validation.isValid) {
    alert(validation.error || 'Invalid file');
    target.value = '';
    return;
  }

  // Upload image to server immediately
  const fileName = await uploadImage(file);
  
  if (fileName) {
    const src = getImageUrl(fileName);
    
    editorRef.value?.focus();
    insertImageAtSelection(src);
    
    // Track the newly uploaded image
    trackImage(fileName);
    
    // Update modelValue to trigger parent update
    if (editorRef.value) {
     updateModelValue();
    }
  } else {
    alert('Failed to upload image. Please try again.');
  }

  // Allow same file reselect
  target.value = '';
};
</script>

<template>
  <div class="form-group" :class="{ 'error': props.error }">
    <div class="custom-editor">
      <!-- Toolbar -->
      <div class="editor-toolbar">
        <!-- Text Formatting -->
        <div class="toolbar-group">
          <button v-if="props.toolbarOptions?.htmlMode" type="button" @click="toggleHtmlMode" :class="{ active: activeFormats.htmlMode }"
            class="toolbar-btn" title="HTML">
            <strong>&lt;&gt;</strong>
          </button>
          <button v-if="props.toolbarOptions?.bold" type="button" @click="toggleTextStyle('bold', () => updateModelValue())" :class="{ active: activeFormats.bold }"
            class="toolbar-btn" title="Bold">
            <strong>B</strong>
          </button>
          <button v-if="props.toolbarOptions?.italic" type="button" @click="toggleTextStyle('italic', () => updateModelValue())" :class="{ active: activeFormats.italic }"
            class="toolbar-btn" title="Italic">
            <em>I</em>
          </button>
          <button v-if="props.toolbarOptions?.underline" type="button" @click="toggleTextStyle('underline', () => updateModelValue())" :class="{ active: activeFormats.underline }"
            class="toolbar-btn" title="Underline">
            <u>U</u>
          </button>
          <button v-if="props.toolbarOptions?.strikethrough" type="button" @click="toggleTextStyle('strikethrough', () => updateModelValue())"
            :class="{ active: activeFormats.strikethrough }" class="toolbar-btn" title="Strike">
            <s>S</s>
          </button>
        </div>

        <!-- Lists -->
        <div class="toolbar-group">
          <button v-if="props.toolbarOptions?.orderedList" type="button" @click="insertList('ol', () => updateModelValue())" class="toolbar-btn" title="Numbered List"
            :class="{ active: activeFormats.orderedList }">
            <SpriteSymbol name="numbered-list" width="18" height="18" />
          </button>
          <button v-if="props.toolbarOptions?.bulletList" type="button" @click="insertList('ul', () => updateModelValue())" class="toolbar-btn" title="Bullet List"
            :class="{ active: activeFormats.bulletList }">
            <SpriteSymbol name="unordered-list" width="18" height="18" />
          </button>
        </div>

        <!-- Alignment -->
        <div class="toolbar-group">
          <div class="alignment-picker-wrapper">
            <button v-if="props.toolbarOptions?.alignment" type="button" @click="showAlignmentPicker = !showAlignmentPicker" class="toolbar-btn"
              title="Text Alignment" :class="{ active: activeFormats.alignment !== 'left' }">
              <SpriteSymbol name="text-left" width="16" height="16" />
            </button>
            <div v-if="showAlignmentPicker" class="alignment-picker-dropdown">
              <button type="button" @click="toggleAlignment('left', () => updateModelValue())" class="alignment-option" title="Align Left">
                <SpriteSymbol name="text-left" width="16" height="16" />
              </button>
              <button type="button" @click="toggleAlignment('center', () => updateModelValue())" class="alignment-option" title="Align Center">
                <SpriteSymbol name="text-center" width="16" height="16" />
              </button>
              <button type="button" @click="toggleAlignment('right', () => updateModelValue())" class="alignment-option" title="Align Right">
                <SpriteSymbol name="text-right" width="16" height="16" />
              </button>
              <button type="button" @click="toggleAlignment('justify', () => updateModelValue())" class="alignment-option" title="Justify">
                <SpriteSymbol name="text-justify" width="16" height="16" />
              </button>
            </div>
          </div>
        </div>

        <!-- Link & Image -->
        <div class="toolbar-group">
          <div v-if="props.toolbarOptions?.link" class="link-popup-wrapper">
            <button type="button" @click="openLinkPopup" class="toolbar-btn" title="Insert Link"
              :class="{ active: activeFormats.link }">
              <SpriteSymbol name="link" width="16" height="16" />
            </button>
            <div v-if="showLinkPopup" class="link-popup-dropdown">
              <input v-model="linkUrl" type="url" placeholder="https://example.com" class="link-input"
                @keyup.enter="insertLink(linkUrl, savedSelection, () => updateModelValue())" autofocus />
              <div class="link-actions">
                <button type="button" @click="insertLink(linkUrl, savedSelection, () => updateModelValue())"
                  class="link-btn-primary">Insert</button>
                <button type="button" @click="cancelLink" class="link-btn-secondary">Cancel</button>
              </div>
            </div>
          </div>
          <button v-if="props.toolbarOptions?.image" type="button" @click="openImageUpload" class="toolbar-btn" title="Insert Image">
            <SpriteSymbol name="image" width="16" height="16" />
          </button>
        </div>
      </div>

      <!-- Hidden file input for image upload -->
      <input ref="imageInputRef" type="file" accept="image/png,image/jpeg,image/jpg,image/webp"
        @change="handleImageUpload" style="display: none;" />

      <!-- Editor Content -->
      <div v-show="!activeFormats.htmlMode" ref="editorRef" class="editor-content" contenteditable="true"
        @keydown="handleKeyDown" @beforeinput="handleBeforeInputWrapper" @input="handleInput" @paste="handlePasteWrapper">
      </div>

      <textarea v-show="activeFormats.htmlMode" v-model="htmlSource" class="editor-content"></textarea>
    </div>

    <span v-if="props.error" class="error-message">{{ props.error }}</span>
  </div>
</template>

<style scoped lang="scss">
.custom-editor {
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background: white;
}

.editor-toolbar {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  padding: 0.5rem;
  background-color: #f9fafb;
  border-bottom: 1px solid #d1d5db;
  border-radius: 0.5rem;
  border-bottom-left-radius: 0;
  border-bottom-right-radius: 0;
}

.toolbar-group {
  display: flex;
  gap: 0.25rem;
  position: relative;
}

.toolbar-btn {
  padding: 0.375rem 0.625rem;
  border: 1px solid transparent;
  background: white;
  border-radius: 0.25rem;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 600;
  transition: all 0.2s;
  min-width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;

  &:hover {
    background-color: #e5e7eb;
    border-color: #d1d5db;
  }

  &.active {
    background-color: #dbeafe;
    border-color: #3b82f6;
    color: #3b82f6;
  }
}

.alignment-picker-wrapper {
  position: relative;
}

.alignment-picker-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  margin-top: 0.25rem;
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  padding: 0.2rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  z-index: 10;
}

.alignment-option {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 0.3rem 0.3rem;
  border: none;
  background: white;
  border-radius: 0.25rem;
  cursor: pointer;
  font-size: 0.875rem;
  color: #1f2937;
  transition: background 0.2s;
  text-align: left;

  &:hover {
    background: #f3f4f6;
  }

  span {
    flex: 1;
  }
}

.link-popup-wrapper {
  position: relative;
}

.link-popup-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  margin-top: 0.25rem;
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  padding: 0.75rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  z-index: 10;
  min-width: 280px;
}

.link-input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;

  &:focus {
    outline: none;
    border-color: #3b82f6;
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  }
}

.link-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.link-btn-primary {
  padding: 0.375rem 0.75rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: background 0.2s;

  &:hover {
    background: #2563eb;
  }
}

.link-btn-secondary {
  padding: 0.375rem 0.75rem;
  background: #e5e7eb;
  color: #1f2937;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: background 0.2s;

  &:hover {
    background: #d1d5db;
  }
}

.editor-content {
  min-height: 120px;
  padding: 0.75rem;
  font-size: 1rem;
  color: #1f2937;
  outline: none;
  overflow-y: auto;
  max-height: 400px;
  width: 100%;
  border: none;

  &:focus {
    outline: none;
  }

  // Style content inside editor
  :deep(img) {
    max-width: 400px;
    height: auto;
    display: inline-block;
  }

  :deep(a) {
    color: #3b82f6;
    text-decoration: underline;
  }

  :deep(ul),
  :deep(ol) {
    padding-left: 1.5rem;
    margin: 0.5rem 0;
  }
}

.error {
  .custom-editor {
    border-color: #ef4444;
    box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
  }
}

.btn-primary {
  padding: 0.5rem 1rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: background 0.2s;

  &:hover {
    background: #2563eb;
  }
}

.btn-secondary {
  padding: 0.5rem 1rem;
  background: #e5e7eb;
  color: #1f2937;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: background 0.2s;

  &:hover {
    background: #d1d5db;
  }
}

.character-count {
  display: block;
  text-align: right;
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.25rem;
}

.error-message {
  display: block;
  color: #ef4444;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}
</style>


