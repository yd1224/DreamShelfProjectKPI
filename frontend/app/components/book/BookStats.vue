<script setup lang="ts">
interface Props {
  totalChapters: number
  lastUpdatedAt: string
}

defineProps<Props>()

const formatDate = (dateString: string | null | undefined) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return 'Invalid Date'
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}
</script>

<template>
  <div class="sidebar-card stats-card">
    <div class="section-header">
      <h3 class="sidebar-title">
        <SpriteSymbol name="attention" width="24" height="24" />
        Information
      </h3>
    </div>
    <div class="stats-list">
      <div class="stat-item">
        <div class="stat-icon">
          <svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M3 4.5H15M3 9H15M3 13.5H9" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-label">Chapters</span>
          <span class="stat-value">{{ totalChapters }}</span>
        </div>
      </div>
      <div class="stat-item">
        <div class="stat-icon">
          <svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M9 15.75C12.7279 15.75 15.75 12.7279 15.75 9C15.75 5.27208 12.7279 2.25 9 2.25C5.27208 2.25 2.25 5.27208 2.25 9C2.25 12.7279 5.27208 15.75 9 15.75Z" stroke="currentColor" stroke-width="1.5"/>
            <path d="M9 6V9L11.25 11.25" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-label">Last Updated</span>
          <span class="stat-value">{{ formatDate(lastUpdatedAt) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.sidebar-card {
  background-color: #ffffff;
  border-radius: 0.75rem;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  border: 1px solid #e5e7eb;

  &.stats-card {
    background: linear-gradient(135deg, #ffffff 0%, #f9fafb 100%);
  }
}

.section-header {
  margin-bottom: 1.5rem;
}

.sidebar-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.125rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
}

.section-icon {
  color: #3b82f6;
  flex-shrink: 0;
}

.stats-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  padding: 0.875rem;
  background-color: #ffffff;
  border-radius: 0.5rem;
  border: 1px solid #f3f4f6;
  transition: all 0.2s ease;

  &:hover {
    border-color: #e5e7eb;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
    transform: translateY(-1px);
  }
}

.stat-icon {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #eff6ff 0%, #dbeafe 100%);
  border-radius: 0.5rem;
  color: #3b82f6;
  flex-shrink: 0;
}

.stat-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
  min-width: 0;
}

.stat-label {
  font-size: 0.8125rem;
  color: #6b7280;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}

.stat-value {
  font-size: 0.9375rem;
  color: #1f2937;
  font-weight: 700;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
