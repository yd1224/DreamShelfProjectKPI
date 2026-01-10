<script setup lang="ts">
interface SearchResult {
  id: number
  title: string
  authors?: string[]
  coverImageUrl?: string
  [key: string]: any
}

interface Props {
  placeholder?: string
  showPopup?: boolean
  onSearch: (query: string) => Promise<any[]>
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Search books...',
  showPopup: true,
})

const emit = defineEmits<{
  resultsUpdate: [results: any]
}>()

const searchQuery = ref('')
const searchResults = ref<SearchResult[]>([])
const isSearching = ref(false)
const showResults = ref(false)
const searchInputRef = ref<HTMLInputElement | null>(null)
const searchContainerRef = ref<HTMLDivElement | null>(null)

let searchTimeout: ReturnType<typeof setTimeout> | null = null

const handleSearch = async () => {
  if (!searchQuery.value.trim()) {
    searchResults.value = []
    showResults.value = false
    emit('resultsUpdate', null)
    return
  }

  isSearching.value = true
  try {
    const results = await props.onSearch(searchQuery.value.trim())
    searchResults.value = results
    showResults.value = props.showPopup && results.length > 0
    emit('resultsUpdate', results)
  } catch (error) {
    console.error('Search error:', error)
    searchResults.value = []
    showResults.value = false
    emit('resultsUpdate', [])
  } finally {
    isSearching.value = false
  }
}

const debouncedSearch = () => {
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
  searchTimeout = setTimeout(() => {
    handleSearch()
  }, 300)
}

const handleClickOutside = (event: MouseEvent) => {
  if (searchContainerRef.value && !searchContainerRef.value.contains(event.target as Node)) {
    showResults.value = false
  }
}

const clearSearch = () => {
  searchQuery.value = ''
  searchResults.value = []
  showResults.value = false
  emit('resultsUpdate', null)
}

onMounted(() => {
  if (import.meta.client) {
    document.addEventListener('click', handleClickOutside)
  }
})

onUnmounted(() => {
  if (import.meta.client) {
    document.removeEventListener('click', handleClickOutside)
  }
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
})

watch(searchQuery, () => {
  debouncedSearch()
})
</script>

<template>
  <div ref="searchContainerRef" class="search-container">
    <div class="search-input-wrapper">
      <SpriteSymbol name="search" width="20" height="20" class="search-icon" />
      <input
        ref="searchInputRef"
        v-model="searchQuery"
        type="text"
        :placeholder="placeholder"
        class="search-input"
        @focus="showResults = searchResults.length > 0 && showPopup"
      />
      <button
        v-if="searchQuery"
        class="clear-button"
        @click="clearSearch"
        aria-label="Clear search"
      >
        <SpriteSymbol name="close" width="16" height="16" />
      </button>
      <div v-if="isSearching" class="search-spinner"></div>
    </div>

    <div v-if="showResults && showPopup" class="search-results-popup">
      <div v-if="searchResults.length === 0" class="no-results">
        No books found
      </div>
      <NuxtLink
        v-for="book in searchResults"
        :key="book.id"
        :to="`/books/${book.id}`"
        class="search-result-item"
        @click="showResults = false"
      >
        <img
          v-if="book.coverImageUrl"
          :src="`${useRuntimeConfig().public.apiBaseUrl}/images/${book.coverImageUrl}`"
          :alt="book.title"
          class="result-cover"
        />
        <div v-else class="result-cover-placeholder">
          📚
        </div>
        <div class="result-info">
          <div class="result-title">{{ book.title }}</div>
          <div v-if="book.authors && book.authors.length > 0" class="result-authors">
            {{ book.authors.join(', ') }}
          </div>
        </div>
      </NuxtLink>
    </div>
  </div>
</template>

<style scoped lang="scss">
.search-container {
  position: relative;
  width: 100%;
}

.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  background-color: #f3f4f6;
  border-radius: 8px;
  padding: 0.5rem 1rem;
  transition: all 0.2s ease;

  &:focus-within {
    background-color: #e5e7eb;
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  }
}

.search-icon {
  color: #6b7280;
  flex-shrink: 0;
  margin-right: 0.5rem;
}

.search-input {
  flex: 1;
  border: none;
  background: transparent;
  outline: none;
  font-size: 0.875rem;
  color: #1f2937;
  padding: 0;

  &::placeholder {
    color: #9ca3af;
  }
}

.clear-button {
  display: flex;
  align-items: center;
  justify-content: center;
  background: none;
  border: none;
  cursor: pointer;
  color: #6b7280;
  padding: 0.25rem;
  border-radius: 4px;
  transition: all 0.2s ease;
  margin-left: 0.5rem;

  &:hover {
    background-color: #d1d5db;
    color: #374151;
  }
}

.search-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-left: 0.5rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.search-results-popup {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  right: 0;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
  max-height: 400px;
  overflow-y: auto;
  z-index: 1000;
  padding: 0.5rem;
}

.no-results {
  padding: 1.5rem;
  text-align: center;
  color: #6b7280;
  font-size: 0.875rem;
}

.search-result-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 6px;
  text-decoration: none;
  color: #1f2937;
  transition: background-color 0.15s ease;

  &:hover {
    background-color: #f3f4f6;
  }
}

.result-cover {
  width: 48px;
  height: 64px;
  object-fit: cover;
  border-radius: 4px;
  flex-shrink: 0;
}

.result-cover-placeholder {
  width: 48px;
  height: 64px;
  background-color: #e5e7eb;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  flex-shrink: 0;
}

.result-info {
  flex: 1;
  min-width: 0;
}

.result-title {
  font-weight: 600;
  font-size: 0.875rem;
  color: #1f2937;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  margin-bottom: 0.25rem;
}

.result-authors {
  font-size: 0.75rem;
  color: #6b7280;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

@media (max-width: 768px) {
  .search-results-popup {
    max-height: 300px;
  }

  .result-cover,
  .result-cover-placeholder {
    width: 40px;
    height: 56px;
  }
}
</style>
