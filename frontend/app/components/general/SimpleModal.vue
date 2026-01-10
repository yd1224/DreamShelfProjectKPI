<script setup lang="ts">
interface Props {
  maxWidth?: string
}

interface Emits {
  (e: 'close'): void
  (e: 'open'): void
}

const props = withDefaults(defineProps<Props>(), {
  maxWidth: '400px',
})

const emit = defineEmits<Emits>()

const isDropdownOpen = ref(false)
const dropdownRef = ref<HTMLElement>()

const openDropdown = () => {
  isDropdownOpen.value = true
  emit('open')
}

const closeDropdown = () => {
  isDropdownOpen.value = false
  emit('close')
}

const toggleDropdown = () => {
  if (isDropdownOpen.value) {
    closeDropdown()
  } else {
    openDropdown()
  }
}

// Handle escape key
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && isDropdownOpen.value) {
    closeDropdown()
  }
}

// Handle click outside
const handleClickOutside = (event: MouseEvent) => {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    closeDropdown()
  }
}

// Setup event listeners
watch(isDropdownOpen, (isOpen) => {
  if (import.meta.client) {
    if (isOpen) {
      document.addEventListener('keydown', handleKeydown)
      document.addEventListener('click', handleClickOutside)
    } else {
      document.removeEventListener('keydown', handleKeydown)
      document.removeEventListener('click', handleClickOutside)
    }
  }
})

// Clean up on unmount
onUnmounted(() => {
  if (import.meta.client) {
    document.removeEventListener('keydown', handleKeydown)
    document.removeEventListener('click', handleClickOutside)
  }
})
</script>

<template>
  <div class="dropdown-container" ref="dropdownRef">
    <!-- Trigger Button -->
    <button 
      class="dropdown-trigger"
      @click="toggleDropdown"
      type="button"
    >
      <slot name="trigger">
        Open Dropdown
      </slot>
    </button>

    <!-- Dropdown Content -->
    <Transition name="dropdown">
      <div 
        v-if="isDropdownOpen" 
        class="dropdown-content"
        :style="{ maxWidth: props.maxWidth }"
      >
        <slot></slot>
      </div>
    </Transition>
  </div>
</template>

<style scoped lang="scss">
.dropdown-container {
  position: relative;
  display: inline-block;
}

.dropdown-trigger {
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
}

.dropdown-content {
  position: absolute;
  z-index: 1000;
  background: #ffffff;
  border-radius: 8px;
  box-shadow: 
    0 10px 15px -3px rgba(0, 0, 0, 0.1),
    0 4px 6px -2px rgba(0, 0, 0, 0.05),
    0 0 0 1px rgba(0, 0, 0, 0.05);
  border: 1px solid #e5e7eb;
  overflow: hidden;
  min-width: 250px;
  max-height: 400px;
  overflow-y: auto;
  top: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-top: 0.5rem;
}


// Responsive design
@media (max-width: 640px) {
  .dropdown-content {
    min-width: 160px;
    max-height: 300px;
  }
  
  .dropdown-content {
    left: 0;
    transform: none;
    right: 0;
    margin-left: auto;
    margin-right: auto;
  }
}
</style>