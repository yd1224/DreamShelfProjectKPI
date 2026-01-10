<script setup lang="ts">
interface Props {
  isOpen?: boolean
  openBecomeAuthorPopUp: ()=>void
}

interface Emits {
  (e: 'close'): void
}

const props = withDefaults(defineProps<Props>(), {
  isOpen: false
})

const emit = defineEmits<Emits>()
const auth = useAuthStore()

const closeSidebar = () => {
  emit('close')
}

const handleLogout = async () => {
  await auth.logout()
  closeSidebar()
  navigateTo('/')
}

// Prevent body scroll when sidebar is open
watch(() => props.isOpen, (isOpen) => {
  if (import.meta.client) {
    if (isOpen) {
      useScroll().lockScroll()
    } else {
      useScroll().unlockScroll()
    }
  }
})

// Clean up on unmount
onUnmounted(() => {
  if (import.meta.client) {
    useScroll().unlockScroll()
  }
})
</script>

<template>
  <div>
    <!-- Overlay -->
    <Transition name="overlay">
      <div 
        v-if="isOpen" 
        class="sidebar-overlay"
        @click="closeSidebar"
      />
    </Transition>

    <!-- Sidebar Panel -->
    <Transition name="sidebar">
      <div v-if="isOpen" class="sidebar-panel">
        <!-- Header with close button -->
        <div class="sidebar-header">
          <div class="sidebar-title">Menu</div>
          <button 
            class="close-button"
            @click="closeSidebar"
            aria-label="Close sidebar"
          >
            <SpriteSymbol name="close" width="24" height="24" />
          </button>
        </div>

        <!-- Sidebar Content -->
        <div class="sidebar-content">
          <!-- Profile Section -->
          <div class="nav-section">
            <h3 class="section-title">
              <SpriteSymbol name="user" width="18" height="18" style="flex-shrink: 0;" />
              {{ useAuthStore().decodedToken?.email }}
            </h3>
            <ul class="nav-list">
              <li class="nav-item" v-if="!auth.isLoggedIn">
                <NuxtLink to="/login" class="nav-link nav-button" @click="closeSidebar">
                  Login
                </NuxtLink>
              </li>
              <li class="nav-item" v-if="auth?.decodedToken?.role === 'User'">
                <button class="nav-link nav-button" @click="openBecomeAuthorPopUp">
                    Become Author
                  </button>
              </li>
              <li class="nav-item" v-if="auth.isLoggedIn">
                <button class="nav-link nav-button" @click="handleLogout">
                  Logout
                </button>
              </li>
            </ul>
          </div>

          <!-- Useful Links Section -->
          <div class="nav-section">
            <h3 class="section-title">
              <SpriteSymbol name="attention" width="18" height="18" />
              Useful Links
            </h3>
            <ul class="nav-list">
              <li class="nav-item">
                <NuxtLink to="/" class="nav-link" @click="closeSidebar">
                  Home
                </NuxtLink>
              </li>
              <li class="nav-item">
                <NuxtLink to="/privacy_policy" class="nav-link" @click="closeSidebar">
                  Privacy Policy
                </NuxtLink>
              </li>
              <li class="nav-item">
                <NuxtLink to="/terms_conditions" class="nav-link" @click="closeSidebar">
                  Terms & Conditions
                </NuxtLink>
              </li>
              <li class="nav-item">
                <NuxtLink to="/cookie_policy" class="nav-link" @click="closeSidebar">
                  Cookie Policy
                </NuxtLink>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped lang="scss">
// Breakpoint mixins for responsive design
@mixin mobile {
  @media (max-width: 767px) {
    @content;
  }
}

@mixin tablet {
  @media (min-width: 768px) {
    @content;
  }
}

@mixin desktop {
  @media (min-width: 992px) {
    @content;
  }
}

.sidebar-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4));
  z-index: 998;
  backdrop-filter: blur(4px);
  -webkit-backdrop-filter: blur(4px);
}

.sidebar-panel {
  position: fixed;
  top: 0;
  right: 0;
  height: 100vh;
  width: 100%;
  max-width: 320px;
  background: linear-gradient(180deg, #1f2937 0%, #111827 100%);
  box-shadow: -8px 0 32px rgba(0, 0, 0, 0.3), -2px 0 8px rgba(0, 0, 0, 0.2);
  z-index: 999;
  display: flex;
  flex-direction: column;
  border-left: 1px solid rgba(75, 85, 99, 0.3);

  @include mobile {
    max-width: 85vw;
  }

  @include tablet {
    max-width: 350px;
  }
}

.sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem;
  border-bottom: 1px solid rgba(75, 85, 99, 0.2);
  background: rgba(31, 41, 55, 0.8);
  backdrop-filter: blur(8px);
  position: relative;

  &::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 1.5rem;
    right: 1.5rem;
    height: 1px;
    background: linear-gradient(90deg, transparent, #3b82f6, transparent);
  }
}

.sidebar-title {
  font-size: 1.375rem;
  font-weight: 700;
  color: #f9fafb;
  letter-spacing: -0.025em;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.close-button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border: none;
  background: rgba(75, 85, 99, 0.3);
  border-radius: 8px;
  cursor: pointer;
  color: #d1d5db;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid rgba(75, 85, 99, 0.4);

  &:hover {
    background: rgba(239, 68, 68, 0.2);
    color: #fca5a5;
    border-color: rgba(239, 68, 68, 0.3);
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(239, 68, 68, 0.2);
  }

  &:active {
    transform: translateY(0) scale(0.95);
  }

  svg{
    filter: brightness(0) invert(1);
    opacity: 0.8;
  }
}

.sidebar-content {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem 0 2rem;
  
  // Custom scrollbar
  &::-webkit-scrollbar {
    width: 4px;
  }

  &::-webkit-scrollbar-track {
    background: rgba(75, 85, 99, 0.1);
  }

  &::-webkit-scrollbar-thumb {
    background: rgba(75, 85, 99, 0.4);
    border-radius: 2px;

    &:hover {
      background: rgba(75, 85, 99, 0.6);
    }
  }
}

.nav-section {
  margin-bottom: 2.5rem;
  position: relative;

  &:last-child {
    margin-bottom: 1rem;
  }

  &::before {
    content: '';
    position: absolute;
    top: -0.5rem;
    left: 1.5rem;
    right: 1.5rem;
    height: 1px;
    background: linear-gradient(90deg, transparent, rgba(75, 85, 99, 0.3), transparent);
  }

  &:first-child::before {
    display: none;
  }
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1rem;
  font-weight: 700;
  color: #f9fafb;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  padding: 1rem 1.5rem 0.75rem;
  margin: 0;
  position: relative;

  svg {
    filter: brightness(0) invert(1);
    opacity: 0.8;
  }
}

.nav-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.nav-item {
  margin: 0;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem 1.5rem;
  color: #d1d5db;
  text-decoration: none;
  font-weight: 500;
  font-size: 0.95rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border-left: 3px solid transparent;
  width: 100%;
  text-align: left;
  position: relative;
  overflow: hidden;

  &::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(90deg, transparent, rgba(59, 130, 246, 0.05), transparent);
    transform: translateX(-100%);
    transition: transform 0.3s ease;
  }

  &:hover {
    background: rgba(59, 130, 246, 0.1);
    color: #f9fafb;
    border-left-color: #3b82f6;
    transform: translateX(4px);
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.1);

    &::before {
      transform: translateX(0);
    }
  }

  &.router-link-active {
    background: linear-gradient(90deg, rgba(59, 130, 246, 0.15), rgba(59, 130, 246, 0.08));
    color: #93c5fd;
    border-left-color: #60a5fa;
    font-weight: 600;
    box-shadow: inset 0 1px 0 rgba(147, 197, 253, 0.2);

    &::after {
      content: '';
      position: absolute;
      right: 1rem;
      top: 50%;
      transform: translateY(-50%);
      width: 4px;
      height: 4px;
      background: #60a5fa;
      border-radius: 50%;
      box-shadow: 0 0 8px rgba(96, 165, 250, 0.6);
    }
  }
}

.nav-button {
  background: none;
  border: none;
  cursor: pointer;
  font-family: inherit;
  font-size: inherit;
}

// Enhanced animations
.overlay-enter-active,
.overlay-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.overlay-enter-from,
.overlay-leave-to {
  opacity: 0;
  backdrop-filter: blur(0px);
}

.sidebar-enter-active,
.sidebar-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.sidebar-enter-from,
.sidebar-leave-to {
  transform: translateX(100%);
  opacity: 0.8;
}
</style>