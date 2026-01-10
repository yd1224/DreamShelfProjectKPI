<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const auth = useAuthStore()
const router = useRouter()

const form = reactive({
  email: '',
  password: ''
})

const errors = reactive({
  email: '',
  password: '',
  general: ''
})

const loading = ref(false)

const validateForm = () => {
  errors.email = ''
  errors.password = ''
  errors.general = ''

  if (!form.email) {
    errors.email = 'Email is required'
    return false
  }
  
  if (!/\S+@\S+\.\S+/.test(form.email)) {
    errors.email = 'Please enter a valid email'
    return false
  }

  if (!form.password) {
    errors.password = 'Password is required'
    return false
  }

  if (form.password.length < 6) {
    errors.password = 'Password must be at least 6 characters'
    return false
  }

  return true
}

const handleLogin = async () => {
  if (!validateForm()) return

  loading.value = true
  
  try {
    await auth.login(form.email, form.password)
    const redirectTo = useRoute().query.redirect as string || '/'
    await router.push(redirectTo)
  } catch (error: any) {
    errors.general = error.message || 'Login failed. Please try again.'
  } finally {
    loading.value = false
  }
}

const goToRegister = () => {
  // Pass along the redirect query when navigating to register
  router.push({ path: '/register', query: { redirect: useRoute().query.redirect } })
}
</script>

<template>
  <div class="login-page">
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h1>Welcome Back</h1>
          <p>Sign in to your DreamShelf account</p>
        </div>

        <form @submit.prevent="handleLogin" class="login-form">
          <div class="form-group">
            <label for="email">Email</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              :class="{ 'error': errors.email }"
              placeholder="Enter your email"
            />
            <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              :class="{ 'error': errors.password }"
              placeholder="Enter your password"
            />
            <span v-if="errors.password" class="error-message">{{ errors.password }}</span>
          </div>

          <div class="form-actions">
            <button type="submit" :disabled="loading" class="login-btn">
              <span v-if="loading">Signing in...</span>
              <span v-else>Sign In</span>
            </button>
          </div>

          <div v-if="errors.general" class="general-error">
            {{ errors.general }}
          </div>
        </form>

        <div class="login-footer">
          <p>Don't have an account? <NuxtLink @click="goToRegister" class="register-link">Sign up</NuxtLink></p>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.login-page {
  background-color: #f9fafb;
  color: #1f2937;
  min-height: calc(100vh - 200px);
  padding: 2rem 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-container {
  width: 100%;
  max-width: 400px;
  padding: 0 1rem;
}

.login-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border: 1px solid #e5e7eb;
}

.login-header {
  text-align: center;
  margin-bottom: 2rem;

  h1 {
    color: #1f2937;
    font-size: 1.875rem;
    font-weight: 700;
    margin-bottom: 0.5rem;
  }

  p {
    color: #6b7280;
    font-size: 1rem;
  }
}

.login-form {
  .form-group {
    margin-bottom: 1.5rem;

    label {
      display: block;
      color: #374151;
      font-size: 0.875rem;
      font-weight: 600;
      margin-bottom: 0.5rem;
    }

    input {
      width: 100%;
      padding: 0.75rem;
      background: #ffffff;
      border: 1px solid #d1d5db;
      border-radius: 8px;
      color: #1f2937;
      font-size: 0.875rem;
      transition: all 0.2s;

      &:focus {
        outline: none;
        border-color: #3b82f6;
        box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
      }

      &.error {
        border-color: #ef4444;
        box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
      }

      &::placeholder {
        color: #9ca3af;
      }
    }

    .error-message {
      display: block;
      color: #ef4444;
      font-size: 0.75rem;
      margin-top: 0.25rem;
    }
  }
}

.form-actions {
  margin-bottom: 1rem;

  .login-btn {
    width: 100%;
    padding: 0.875rem;
    background: #1f2937;
    color: white;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);

    &:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none;
    }
  }
}

.general-error {
  background: #fef2f2;
  border: 1px solid #fecaca;
  color: #dc2626;
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 0.875rem;
  text-align: center;
  margin-top: 1rem;
}

.login-footer {
  text-align: center;
  margin-top: 1.5rem;

  p {
    color: #6b7280;
    font-size: 0.875rem;
  }

  .register-link {
    color: #3b82f6;
    text-decoration: none;
    font-weight: 600;

    &:hover {
      color: #2563eb;
      text-decoration: underline;
    }
  }
}
</style>