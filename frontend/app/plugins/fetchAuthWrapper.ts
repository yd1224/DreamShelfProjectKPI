import type { AuthResponse } from '~/types/auth'

export default defineNuxtPlugin((nuxtApp) => {
  const auth = useAuthStore()
  const event = process.server ? useRequestEvent() : null

  const fetchWrapper = $fetch.create({
    async onRequest({ options, request }) {

      if (auth.accessToken) {
        options.headers = {
          ...options.headers,
          Authorization: `Bearer ${auth.accessToken}`
        }
      }

      if (process.server && event?.node.req.headers.cookie) {
        options.headers = {
          ...options.headers,
          cookie: event.node.req.headers.cookie
        }
      }
    },

    async onResponse({ response }) {
      if (process.server && event && response.headers) {
        forwardCookies(response, event)
      }
    },

    async onResponseError({ response, request, options }) {
      // refresh token logic
      if (response?.status === 401) {
        // use HttpOnly cookie refresh on client
        const refreshResponse : AuthResponse = await $fetch('/api/auth/refresh', {
          headers: {
            cookie: event.node.req.headers.cookie || '', // forward HttpOnly cookies
          },
          onResponse({ response }) {
            if (process.server && event && response.headers) {
              forwardCookies(response, event)
            }
          },
          credentials: 'include'
        })
        auth.setAccessToken(refreshResponse.accessToken)
        options.headers = {
          ...options.headers,
          Authorization: `Bearer ${auth.accessToken}`
        }
        return await $fetch(request, options as any)
      }
      throw response
    }
  })

  nuxtApp.provide('fetchWrapper', fetchWrapper) // override Nuxt’s $fetch
})
