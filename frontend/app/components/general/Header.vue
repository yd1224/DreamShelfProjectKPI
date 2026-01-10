<script setup lang="ts">
const isSidebarOpen = ref(false)
const isBecomeAuthorPopUpOpen = ref(false)
const auth = useAuthStore()
const route = useRoute()
const $api = useNuxtApp().$api

const isMain= computed(() => route.path === '/')

const toggleSidebar = () => {
    isSidebarOpen.value = !isSidebarOpen.value
}

const closeSidebar = () => {
    isSidebarOpen.value = false
}

const handleLogout = async () => {
  await auth.logout()
  navigateTo('/')
}

const openBecomeAuthorPopUp = () => {
  isBecomeAuthorPopUpOpen.value = true

}

const closeBecomeAuthorPopUp = () => {
  isBecomeAuthorPopUpOpen.value = false
}

const handleBecomeAuthorSuccess = () => {
  console.log('Successfully became an author!')
}

const handleBookSearch = async (query: string) => {
  try {
    const results = await $api.searchBooks(query)
    return results
  } catch (error) {
    console.error('Error searching books:', error)
    return []
  }
}
</script>

<template>
    <div>
        <div class="section">
          <NuxtLink to="/">
            <div class="logo">
                <img src="/images/logo.png" width="70" height="50"/>
            </div>
          </NuxtLink>
            <div v-if="isMain" class="search-wrapper">
                <SearchInput :on-search="handleBookSearch" placeholder="Search books..." />
            </div>
            <div class="panel">
                <div class="panel-mob">
                    <button 
                        class="burger-button"
                        @click="toggleSidebar"
                        aria-label="Open menu"
                    >
                        <SpriteSymbol name="burger-menu" width="32" height="32"/>
                    </button>
                </div>
                <div class="panel-desk">
                    <SimpleModal>
                      <template #trigger>
                        <div class="user-panel">
                          <SpriteSymbol name="user" width="24" height="24"/>
                        </div>
                      </template>
                      <div class="dropdown-menu">
                        <div class="menu-item" v-if="!auth.isLoggedIn">
                          <NuxtLink to="/login" class="menu-link">
                            Login
                          </NuxtLink>
                        </div>
                        <div class="menu-item" v-if="auth.isLoggedIn">
                          <div class="menu-link">
                            {{ auth.decodedToken.email }}
                          </div>
                        </div>
                        <div class="menu-item" v-if="auth?.decodedToken?.role === 'User'">
                           <button class="menu-link menu-button" @click="openBecomeAuthorPopUp">
                            Become Author
                          </button>
                        </div>
                        <div class="menu-item" v-if="auth.isLoggedIn">
                          <button class="menu-link menu-button" @click="handleLogout">
                            Logout
                          </button>
                        </div>
                      </div>
                    </SimpleModal>
                    
                    <SimpleModal>
                      <template #trigger>
                        <div class="info-panel">
                          <SpriteSymbol name="attention" width="27" height="27"/>
                          <div style="color: black; font-size: 14px;">Useful Info</div>
                        </div>
                      </template>
                      <div class="dropdown-menu">
                        <div class="menu-item">
                          <NuxtLink to="/" class="menu-link">
                            Home
                          </NuxtLink>
                        </div>
                        <div class="menu-item">
                          <NuxtLink to="/privacy_policy" class="menu-link">
                            Privacy Policy
                          </NuxtLink>
                        </div>
                        <div class="menu-item">
                          <NuxtLink to="/terms_conditions" class="menu-link">
                            Terms & Conditions
                          </NuxtLink>
                        </div>
                        <div class="menu-item">
                          <NuxtLink to="/cookie_policy" class="menu-link">
                            Cookie Policy
                          </NuxtLink>
                        </div>
                      </div>
                    </SimpleModal>
                </div>
            </div>
        </div>
        
        <!-- Mobile Sidebar Panel -->
        <SideBarPanel 
            :is-open="isSidebarOpen" 
            @close="closeSidebar"
            :openBecomeAuthorPopUp="openBecomeAuthorPopUp"
        />

        <!-- Become Author Popup -->
        <BecomeAuthorPopUp
            :is-open="isBecomeAuthorPopUpOpen"
            @close="closeBecomeAuthorPopUp"
            @success="handleBecomeAuthorSuccess"
        />
    </div>
</template>

<style scoped lang="scss">
.section {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  padding: 1rem;
}

.logo {
  display: flex;
  align-items: center;
  flex-shrink: 0;
}

.search-wrapper {
  flex: 1;
  max-width: 500px;
  display: none;
  
  @media (min-width: 768px) {
    display: block;
  }
}

.panel {
  display: flex;
  align-items: center;
}

.panel-mob {
  display: block;
  
  @media (min-width: 768px) {
    display: none;
  }
}

.panel-desk {
  display: none;
  gap: 1rem;
  
  @media (min-width: 768px) {
    display: flex;
  }
}

.user-panel,
.info-panel {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
  color: #374151;

  &:hover {
    background-color: #f3f4f6;
    color: #1f2937;
  }
}

.burger-button {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0.5rem;
  border: none;
  background: none;
  border-radius: 6px;
  cursor: pointer;
  color: #374151;
  transition: all 0.2s ease;

  &:hover {
    background-color: #f3f4f6;
    color: #1f2937;
  }

  &:active {
    transform: scale(0.95);
  }

  &:focus {
    outline: 2px solid #3b82f6;
    outline-offset: 2px;
  }
}

// Dropdown menu styles
.dropdown-menu {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  min-width: 180px;
  padding: 0.5rem;
}

.menu-item {
  display: flex;
}

.menu-link {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 0.75rem 1rem;
  color: #374151;
  text-decoration: none;
  font-weight: 500;
  border-radius: 6px;
  transition: all 0.2s ease;

  &:hover {
    background-color: #f3f4f6;
    color: #1f2937;
  }

  &.router-link-active {
    background-color: #eff6ff;
    color: #1d4ed8;
  }
}

.menu-button {
  background: none;
  border: none;
  cursor: pointer;
  font-family: inherit;
  font-size: inherit;
}
</style>

