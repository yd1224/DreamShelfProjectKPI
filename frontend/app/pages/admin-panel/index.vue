<script setup lang="ts">
interface AdminPanelBook {
  id: number
  title: string
  authors: string[]
  publicationYear: number | null
  bookStatus: number
  lastUpdatedAt: string
  isPublished: boolean
}

interface AdminPanelBooksResponse {
  books: AdminPanelBook[]
  hasNextPage?: boolean
  hasPreviousPage?: boolean
}

useSeoMeta({
  title: 'Admin Panel - DreamShelf',
  description: 'Manage books in the DreamShelf admin panel.',
})

const $api = useNuxtApp().$api
const { bookStatus } = useConstants()

const currentPage = ref(1)
const loading = ref(false)
const error = ref<string | null>(null)
const displayData = ref<AdminPanelBooksResponse | null>(null)
const isSearching = ref(false)
const currentQuery = ref('')

const fetchBooks = async (page: number) => {
  loading.value = true
  error.value = null
  try {
    const response = await $api.getAdminPanelBooks(page)
    console.log(response);
    
    displayData.value = response
    currentPage.value = page
  } catch (e: any) {
    error.value = e.message || 'Failed to fetch books'
    console.error('Error fetching admin panel books:', e)
    throw new Error('Failed to fetch books')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchBooks(1)
})


const handleBookSearch = async (query: string, page: number = 1) => {
  try {
    const results = await $api.searchBooksInAdminPanel(query, page)
    console.log("RRR", results);
    currentQuery.value = query;
    currentPage.value = page;
    
    displayData.value = results;
    return results
  } catch (error) {
    console.error('Error searching books:', error)
    return []
  }
}

const handleSearchResults = (results: AdminPanelBooksResponse | null) => {
  if (!results) {
    clearSearch()
    return
  }
  isSearching.value = true
  displayData.value = results
}

const clearSearch = () => {
  isSearching.value = false
  currentQuery.value = ''
  fetchBooks(1)
}

const getStatusName = (statusId: number): string => {
  const status = bookStatus.find(s => s.id === statusId)
  return status?.name || 'Unknown'
}

const getStatusClass = (statusId: number): string => {
  const statusClasses: Record<number, string> = {
    0: 'status-announced',
    1: 'status-ongoing',
    2: 'status-completed',
    3: 'status-frozen',
  }
  return statusClasses[statusId] || ''
}

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

const goToNextPage = () => {
  if (displayData.value?.hasNextPage) {
    if(!isSearching.value) {
      fetchBooks(currentPage.value + 1)
      return;
    }
    handleBookSearch(currentQuery.value, currentPage.value + 1)
  }
}

const goToPreviousPage = () => {
  if (displayData.value?.hasPreviousPage) {
    if(!isSearching.value) {
      fetchBooks(currentPage.value - 1)
      return;
    }
    handleBookSearch(currentQuery.value, currentPage.value - 1)
  }
}

const deleteBook = async (bookId: number | string) => {
  await $api.deleteBook(String(bookId))
}
</script>

<template>
  <div class="admin-panel">
    <div class="section">
      <div class="page-header">
        <h1 class="page-title">Admin Panel</h1>
        <p class="page-description">Manage and review all books in the system.</p>
        <div class="header-actions">
          <div class="search-container-admin">
            <SearchInput 
              :on-search="handleBookSearch" 
              placeholder="Search books..." 
              :show-popup="false"
              @results-update="handleSearchResults"
            />
          </div>
          <NuxtLink to="/books/create" class="btn-add-book" target="_blank"
                      rel="noopener">
            + Add Book
          </NuxtLink>
        </div>
      </div>

      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Loading books...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <p>{{ error }}</p>
        <button class="btn-retry" @click="fetchBooks(currentPage)">Retry</button>
      </div>

      <div v-else-if="displayData" class="books-container">
        <div class="table-wrapper">
          <table class="books-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Authors</th>
                <th>Year</th>
                <th>Status</th>
                <th>Published</th>
                <th>Last Updated</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="book in displayData?.books" :key="book.id">
                <td class="cell-id">{{ book.id }}</td>
                <td class="cell-title">
                  <NuxtLink :to="`/books/${book.id}`" class="book-link">
                    {{ book.title }}
                  </NuxtLink>
                </td>
                <td class="cell-authors">
                  {{ book.authors.length > 0 ? book.authors.join(', ') : '—' }}
                </td>
                <td class="cell-year">{{ book.publicationYear || '—' }}</td>
                <td class="cell-status">
                  <span class="status-badge" :class="getStatusClass(book.bookStatus)">
                    {{ getStatusName(book.bookStatus) }}
                  </span>
                </td>
                <td class="cell-published">
                  <span class="published-badge" :class="book.isPublished ? 'published' : 'draft'">
                    {{ book.isPublished ? 'Yes' : 'No' }}
                  </span>
                </td>
                <td class="cell-date">{{ formatDate(book.lastUpdatedAt) }}</td>
                <td class="cell-actions">
                  <NuxtLink :to="`/books/${book.id}/edit`" class="btn-action btn-edit" style="margin-right: 10px" target="_blank"
                    rel="noopener">
                    Edit
                  </NuxtLink>
                  <div class="btn-action btn-edit" @click="deleteBook(book.id)">
                    Delete
                  </div>
                </td>
              </tr>
              <tr v-if="displayData.books.length === 0">
                <td colspan="8" class="empty-state">No books found.</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="pagination">
          <button 
            class="btn-pagination" 
            :disabled="!displayData?.hasPreviousPage"
            @click="goToPreviousPage"
          >
            ← Previous
          </button>
          <span class="page-indicator">Page {{ currentPage }}</span>
          <button 
            class="btn-pagination" 
            :disabled="!displayData?.hasNextPage"
            @click="goToNextPage"
          >
            Next →
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.admin-panel {
  background-color: #f9fafb;
  color: #1f2937;
  min-height: calc(100vh - 200px);
  padding: 2rem 0;
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-title {
  font-size: 1.875rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.page-description {
  color: #6b7280;
  font-size: 1rem;
  margin: 0;
}

.loading-state,
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: #6b7280;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.btn-retry {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s;

  &:hover {
    background-color: #2563eb;
  }
}

.books-container {
  background-color: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.table-wrapper {
  overflow-x: auto;
}

.books-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;

  th,
  td {
    padding: 0.75rem 1rem;
    text-align: left;
    border-bottom: 1px solid #e5e7eb;
  }

  th {
    background-color: #f9fafb;
    font-weight: 600;
    color: #374151;
    white-space: nowrap;
  }

  tbody tr {
    transition: background-color 0.15s;

    &:hover {
      background-color: #f9fafb;
    }
  }
}

.cell-id {
  color: #6b7280;
  font-family: monospace;
}

.cell-title {
  max-width: 250px;
}

.book-link {
  color: #3b82f6;
  text-decoration: none;
  font-weight: 500;

  &:hover {
    text-decoration: underline;
  }
}

.cell-authors {
  color: #6b7280;
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.cell-year {
  color: #6b7280;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
  text-transform: capitalize;

  &.status-announced {
    background-color: #fef3c7;
    color: #92400e;
  }

  &.status-ongoing {
    background-color: #dbeafe;
    color: #1e40af;
  }

  &.status-completed {
    background-color: #d1fae5;
    color: #065f46;
  }

  &.status-frozen {
    background-color: #e5e7eb;
    color: #374151;
  }
}

.published-badge {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;

  &.published {
    background-color: #d1fae5;
    color: #065f46;
  }

  &.draft {
    background-color: #fee2e2;
    color: #991b1b;
  }
}

.cell-date {
  color: #6b7280;
  white-space: nowrap;
}

.cell-actions {
  white-space: nowrap;
}

.btn-action {
  display: inline-block;
  padding: 5px 10px;
  border-radius: 0.375rem;
  font-size: 0.75rem;
  font-weight: 500;
  text-decoration: none;
  transition: all 0.15s;
}

.btn-edit {
  background-color: #3b82f6;
  color: white;

  &:hover {
    background-color: #2563eb;
  }
}

.empty-state {
  text-align: center;
  color: #6b7280;
  padding: 2rem !important;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 1rem;
  border-top: 1px solid #e5e7eb;
}

.btn-pagination {
  padding: 0.5rem 1rem;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  color: #374151;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s;

  &:hover:not(:disabled) {
    background-color: #f9fafb;
    border-color: #9ca3af;
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
}

.page-indicator {
  color: #6b7280;
  font-size: 0.875rem;
}

.header-actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-top: 1rem;
  align-items: center;

  @media (min-width: 768px) {
    flex-direction: row;
    justify-content: center;
  }
}

.search-container-admin {
  width: 100%;
  max-width: 400px;
}

.btn-add-book {
  display: inline-block;
  padding: 0.625rem 1.25rem;
  background-color: #10b981;
  color: white;
  border-radius: 0.375rem;
  font-weight: 500;
  text-decoration: none;
  transition: background-color 0.2s;
  white-space: nowrap;

  &:hover {
    background-color: #059669;
  }
}

.search-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: #f9fafb;
  border-bottom: 1px solid #e5e7eb;

  p {
    margin: 0;
    font-weight: 600;
    color: #374151;
  }
}

.btn-clear-search {
  padding: 0.5rem 1rem;
  background-color: #6b7280;
  color: white;
  border: none;
  border-radius: 0.375rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;

  &:hover {
    background-color: #4b5563;
  }
}

@media (max-width: 768px) {
  .admin-panel {
    padding: 1rem 0;
  }

  .page-title {
    font-size: 1.5rem;
  }

  .books-table {
    font-size: 0.8rem;

    th,
    td {
      padding: 0.5rem;
    }
  }

  .cell-authors {
    max-width: 120px;
  }

  .cell-title {
    max-width: 150px;
  }
}
</style>