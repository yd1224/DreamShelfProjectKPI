import type { AuthResponse } from "~/types/auth"

export default defineNuxtPlugin(async (nuxtApp) => {
  const event = nuxtApp.ssrContext?.event;
  if (!event) return;

  const auth = useAuthStore();

  try {
    const data : AuthResponse = await $fetch(`${useRuntimeConfig().public.apiBaseUrl}/api/auth/refresh`, {
      method: 'POST',
      headers: {
        cookie: event.node.req.headers.cookie || '', // forward HttpOnly cookies
      },
      onResponse({ response }) {
       forwardCookies(response, event)
      },
    });

    auth.setAccessToken(data.accessToken);
    auth.isLoggedIn = true;
  } catch(error) {
    auth.setAccessToken(null);
    auth.isLoggedIn = false;
  }
});