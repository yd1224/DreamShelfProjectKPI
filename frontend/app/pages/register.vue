<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const router = useRouter()

const form = reactive({
  email: '',
  password: '',
  confirmPassword: ''
})

const errors = reactive({
  email: '',
  password: '',
  confirmPassword: '',
  general: ''
})

const loading = ref(false)

const validateForm = () => {
  errors.email = ''
  errors.password = ''
  errors.confirmPassword = ''
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

  if (!form.confirmPassword) {
    errors.confirmPassword = 'Please confirm your password'
    return false
  }

  if (form.password !== form.confirmPassword) {
    errors.confirmPassword = 'Passwords do not match'
    return false
  }

  return true
}

const handleRegister = async () => {
  if (!validateForm()) return

  loading.value = true
  
  try {
    await useAuthStore().register(form.email, form.password)
    
    const redirectTo = useRoute().query.redirect as string || '/'
    router.push(redirectTo)
  } catch (error: any) {
    console.error('Registration error:', error)
    console.log('Error status:', error.status)
    console.log('Error data:', error.data)
    console.log('Error message:', error.message)
    
    if (error.data?.message) {
      errors.general = error.data.message
    } else if (error.data?.errors) {
      // Handle validation errors from backend
      const backendErrors = error.data.errors
      if (backendErrors.email) errors.email = backendErrors.email[0]
      if (backendErrors.password) errors.password = backendErrors.password[0]
    } else {
      errors.general = `Registration failed: ${error.message || error.statusText || 'Unknown error'}`
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="register-page">
    <div class="register-container">
      <div class="register-card">
        <div class="register-header">
          <h1>Join DreamShelf</h1>
          <p>Create your account to start building your digital library</p>
        </div>

        <form @submit.prevent="handleRegister" class="register-form">
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
              placeholder="Create a password"
            />
            <span v-if="errors.password" class="error-message">{{ errors.password }}</span>
          </div>
          <div class="form-group">
            <label for="confirmPassword">Confirm Password</label>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              type="password"
              required
              :class="{ 'error': errors.confirmPassword }"
              placeholder="Confirm your password"
            />
            <span v-if="errors.confirmPassword" class="error-message">{{ errors.confirmPassword }}</span>
          </div>

          <div class="form-actions">
            <button type="submit" :disabled="loading" class="register-btn">
              <span v-if="loading">Creating Account...</span>
              <span v-else>Create Account</span>
            </button>
          </div>

          <div v-if="errors.general" class="general-error">
            {{ errors.general }}
          </div>
        </form>

        <div class="register-footer">
          <p>Already have an account? <NuxtLink to="/login" class="login-link">Sign in</NuxtLink></p>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.register-page {
  background-color: #f9fafb;
  color: #1f2937;
  min-height: calc(100vh - 200px);
  padding: 2rem 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.register-container {
  width: 100%;
  max-width: 450px;
  padding: 0 1rem;
}

.register-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border: 1px solid #e5e7eb;
}

.register-header {
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
    line-height: 1.4;
  }
}

.register-form {
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
      line-height: 1.3;
    }
  }
}

.form-actions {
  margin-bottom: 1rem;

  .register-btn {
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
  line-height: 1.3;
  margin-top: 1rem;
}

.register-footer {
  text-align: center;
  margin-top: 1.5rem;

  p {
    color: #6b7280;
    font-size: 0.875rem;
  }

  .login-link {
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
