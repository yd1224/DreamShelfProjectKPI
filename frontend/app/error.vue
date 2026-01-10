<script setup lang="ts">
const props = defineProps<{ error: any }>()


const statusCode = computed(() => Number(props?.error?.statusCode || props?.error?.status || 500))
const message = computed(() => props?.error?.statusMessage || props?.error?.message || 'Something went wrong')

const goBack = () => {
  if (import.meta.client) {
    // Try to go back; if no history, go to home
    if (window.history.length > 1) return window.history.back()
    navigateTo('/')
  }
}
</script>

<template>
  <div class="error-wrapper">
    <div class="error-card">
      <div class="error-code">{{ statusCode }}</div>
      <div class="error-title">
        <span v-if="statusCode === 404">Page Not Found</span>
        <span v-else>Unexpected Error</span>
      </div>
      <div class="error-message">{{ message }}</div>
      <div class="error-actions">
        <button class="btn" @click="goBack">Go Back</button>
        <NuxtLink class="btn btn-secondary" to="/">Go Home</NuxtLink>
      </div>
    </div>
  </div>
</template>

<style scoped>
.error-wrapper {
  min-height: 60vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}
.error-card {
  max-width: 720px;
  width: 100%;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 24px;
  text-align: center;
  background: #fff;
}
.error-code {
  font-size: 64px;
  font-weight: 800;
  color: #111827;
}
.error-title {
  font-size: 24px;
  font-weight: 700;
  margin-top: 8px;
  color: #111827;
}
.error-message {
  margin-top: 8px;
  color: #6b7280;
}
.error-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
  margin-top: 20px;
}
.btn {
  padding: 10px 16px;
  border-radius: 8px;
  background: #111827;
  color: #fff;
  border: none;
  cursor: pointer;
}
.btn-secondary {
  background: #f3f4f6;
  color: #111827;
}
</style>