import { defineStore } from 'pinia'
import type { AuthResponse } from '~/types/auth'
import { useNuxtApp } from '#app'
import { jwtDecode } from "jwt-decode";

export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(null)
  const isLoggedIn = ref<boolean>(false)
  const decodedToken = ref<any>(null)

  const login = async (email: string, password: string) => {
    const data = await (useNuxtApp() as any).$fetchWrapper(`${useRuntimeConfig().public.apiBaseUrl}/api/auth/login`, {
      method: 'POST',
      body: { email, password },
      credentials: 'include'
    })

    isLoggedIn.value = true
    setAccessToken(data.accessToken)
  }

  const register = async (email: string, password: string) => {
    const data: AuthResponse = await (useNuxtApp() as any).$fetchWrapper(`${useRuntimeConfig().public.apiBaseUrl}/api/auth/register`, {
      method: 'POST',
      body: { email, password },
      credentials: 'include'
    })

    isLoggedIn.value = true
    setAccessToken(data.accessToken)
  }


  const logout = async () => {
    await (useNuxtApp() as any).$fetchWrapper(`${useRuntimeConfig().public.apiBaseUrl}/api/auth/logout`, { method: 'POST', credentials: 'include' })
    isLoggedIn.value = false
    setAccessToken(null)
  }

  const setAccessToken = (token: string | null) => {
    accessToken.value = token
    decodedToken.value = token ? jwtDecode(token) : null
  }

  return { accessToken, isLoggedIn, login, register, logout, decodedToken, setAccessToken }
})
