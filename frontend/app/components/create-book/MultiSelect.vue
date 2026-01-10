<script setup lang="ts">
interface Option {
  id: number | string
  name: string
}

const props = withDefaults(defineProps<{
  label: string
  options: Option[]
  modelValue: number | number[] | string | string[] | null
  placeholder?: string
  error?: string
  required?: boolean
  multiple?: boolean
}>(), {
  multiple: true
})

const emit = defineEmits<{
  'update:modelValue': [value: number | string | (number | string)[] | null]
}>()

const isOpen = ref(false)

const toggleDropdown = () => {
  isOpen.value = !isOpen.value
}

const closeDropdown = () => {
  isOpen.value = false
}

const isSelected = (id: number | string) => {
  if (props.multiple) {
    return Array.isArray(props.modelValue) && props.modelValue.includes(id)
  }
  return props.modelValue === id
}

const toggleItem = (id: number | string) => {
  if (props.multiple) {
    const currentValue = Array.isArray(props.modelValue) ? props.modelValue : []
    const newValue: (number | string)[] = [...currentValue]
    const index = newValue.indexOf(id)
    if (index >= 0) {
      newValue.splice(index, 1)
    } else {
      newValue.push(id)
    }
    emit('update:modelValue', newValue)
  } else {
    // Single select mode
    emit('update:modelValue', id)
    closeDropdown()
  }
}

const removeItem = toggleItem

const getItemName = (id: number | string | null) => {
  if (id === null) return ''
  return props.options.find((o) => o.id === id)?.name || String(id)
}
</script>

<template>
  <div class="form-group">
    <label class="form-label">
      {{ label }} <span v-if="required" class="required">*</span>
    </label>

    <div class="multi-select" @click.stop>
      <div
        class="multi-select-trigger"
        :class="{ 'error': error, 'active': isOpen }"
        @click="toggleDropdown"
      >
        <div class="selected-items">
          <span v-if="modelValue === null || modelValue === undefined || (Array.isArray(modelValue) && modelValue.length === 0)" class="placeholder">{{ placeholder || 'Select...' }}</span>
          <div v-else-if="multiple" class="item-tags">
            <span v-for="item in (modelValue as number[])" :key="item" class="item-tag">
              {{ getItemName(item) }}
              <button type="button" @click.stop="removeItem(item)" class="remove-item">×</button>
            </span>
          </div>
          <span v-else class="selected-value">{{ getItemName(modelValue as number) }}</span>
        </div>
        <SpriteSymbol name="dropdown-arrow" class="dropdown-arrow" :class="{ 'open': isOpen }" width="24" height="24" />
      </div>

      <div v-if="isOpen" class="dropdown">
        <label
          v-for="option in options"
          :key="option.id"
          class="dropdown-option"
        >
          <input
            v-if="multiple"
            type="checkbox"
            class="option-checkbox"
            :checked="isSelected(option.id)"
            @change="toggleItem(option.id)"
          />
          <input
            v-else
            type="radio"
            class="option-checkbox"
            :checked="isSelected(option.id)"
            @change="toggleItem(option.id)"
          />
          <span class="option-label">{{ option.name }}</span>
        </label>
      </div>
    </div>

    <span v-if="error" class="error-message">{{ error }}</span>
    <div v-if="multiple" class="selected-count">{{ Array.isArray(modelValue) ? modelValue.length : 0 }} item(s) selected</div>
  </div>
  
  <!-- Backdrop for closing dropdown -->
  <div 
    v-if="isOpen" 
    class="dropdown-backdrop"
    @click="closeDropdown"
  ></div>
</template>

<style scoped lang="scss">
.multi-select {
  position: relative;
}

.multi-select-trigger {
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

.selected-items {
  flex: 1;
}

.placeholder {
  color: #9ca3af;
  font-size: 1rem;
}

.item-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.item-tag {
  background-color: #3b82f6;
  color: white;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.remove-item {
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
  flex-shrink: 0;
  
  &.open {
    transform: rotate(180deg);
  }
}

.dropdown {
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

.dropdown-option {
  padding: 0.75rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: background-color 0.2s ease;

  &:hover {
    background-color: #f3f4f6;
  }
}

.option-checkbox {
  width: 1rem;
  height: 1rem;
  cursor: pointer;
}

.option-label {
  flex: 1;
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
</style>
