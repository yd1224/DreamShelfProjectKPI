<script setup lang="ts">
import type { ToolbarOptions } from '@/types/editor'

const props = defineProps<{
  modelValue: string
  error?: string
  toolbarOptions?: ToolbarOptions
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const localValue = ref(props.modelValue)

watch(localValue, (newVal) => {
  emit('update:modelValue', newVal)
})

// Calculate plain text length for character count
const getPlainTextLength = (htmlContent: string): number => {
  if (!htmlContent) return 0

  // strip tags
  const plainText = htmlContent.replace(/<[^>]+>/g, '')
  return plainText.length
}
</script>

<template>
    <div class="form-group" :class="{ 'error': props.error }">
        <label for="description" class="form-label">
            Description <span class="required">*</span>
        </label>
        <div class="description-editor-box">
          <client-only>
              <CustomTextEditor 
                v-model="localValue"
                :error="props.error"
                :toolbarOptions="toolbarOptions"
                @update:modelValue="(val) => localValue = val"
              />
          </client-only>
        </div>
        <span v-if="props.error" class="error-message">{{ props.error }}</span>
        <span class="character-count">{{ getPlainTextLength(localValue) }}/1000</span>
    </div>
</template>

<style scoped lang="scss">

.description-editor-box{
  min-height: 166px;
}
</style>
