import { defineNuxtRouteMiddleware, navigateTo } from '#app'

export default defineNuxtRouteMiddleware((to) => {
  const auth = useAuthStore()
  
  const segment = '[^/]+'

  const protectedPatterns = [
    new RegExp(`^/books/create$`),
    new RegExp(`^/books/${segment}/chapters/create$`),
    new RegExp(`^/books/${segment}/edit$`),
    new RegExp(`^/books/${segment}/chapters/${segment}/edit$`),
    new RegExp(`^/admin-panel$`),
  ]

  const isProtectedRoute = protectedPatterns.some(pattern => pattern.test(to.path))

  if (isProtectedRoute && !auth.isLoggedIn) {
    return navigateTo({ path: '/login', query: { redirect: to.path } })
  }

  if ((to.path === '/login' || to.path === '/register') && auth.isLoggedIn) {
    return navigateTo('/')
  }
})
