import type { Chapter } from '~/types/chapter'

export const useChapter = async (chapterId: string, bookId: string, isEdit: boolean = false) => {
    const { $api } = useNuxtApp()

    const { data: rawData, error, refresh } = await useAsyncData(
        `chapter-${chapterId}`,
        () => $api.getChapter(bookId, chapterId, isEdit)
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
    const chapterData = computed<Chapter | null>(() => {
        if (!rawData.value) return null

        const data = rawData.value as any

        return {
            uuid: data.uuid,
            title: data.title,
            chapterNumber: data.chapterNumber,
            htmlContent: data.htmlContent,
            isPublished: data.isPublished,
            lastUpdatedAt: data.lastUpdatedAt,
            bookTitle: data.bookTitle,
            bookCoverImageUrl: data.bookCoverImageUrl,
        }
    })

    return {
        chapterData,
        rawData,
        refresh
    }
}
