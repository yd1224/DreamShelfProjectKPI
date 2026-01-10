import type { Book } from '~/types/book'

export const useBook = async (bookId: string, isEdit: boolean = false) => {
  const { $api } = useNuxtApp()

  // Fetch book data with proper SSR support
  const { data: rawData, refresh, error } = await useAsyncData(
    `book-${bookId}`,
    () => $api.getBook(bookId, isEdit)
  )

  watchEffect(() => {
    if (!error.value) return

    if ([404, 403].includes(error.value?.statusCode ?? 0)) {
      throw createError({
        statusCode: error.value.statusCode,
        statusMessage: error.value.statusMessage
      })
    }
  })

  // Transform data reactively with proper typing
  const bookData = computed<Book | null>(() => {
    if (!rawData.value) return null

    const data = rawData.value as any

    return data;
  })

  return {
    bookData,
    rawData,
    refresh
  }
}
