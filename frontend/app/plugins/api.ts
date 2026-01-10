import type { UpdateBookData } from '@/types/book';
import type { BecomeAuthorData, CreateAuthorData } from '~/types/auth';

export default defineNuxtPlugin(() => {
    const config = useRuntimeConfig();
    const apiBaseUrl = config.public.apiBaseUrl;

    return {
        provide: {
            api: {
                genres: () => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/genres`),
                createBook: (data: FormData) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/book/create`, {
                    method: 'POST',
                    body: data,
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                }),
                getBook: (id: string, isEdit: boolean = false) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/book/${id}`, {
                    method: 'GET',
                    params: {
                        isEdit,
                    },
                }),
                getChapter: (bookId: string, chapterId: string, isEdit: boolean = false) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/chapters/${bookId}/${chapterId}`, {
                    method: 'GET',
                    params: {
                        isEdit,
                    },
                }),
                updateChapter: (bookId: string, chapterId: string, data: { title: string; htmlContent: string; chapterNumber: number; isPublished: boolean; newImageIds: string[] }) =>
                    (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/chapters/${bookId}/${chapterId}`, {
                        method: 'PUT',
                        body: JSON.stringify(data),
                        headers: {
                            'Content-Type': 'application/json',
                        },
                    }),
                createChapter: (bookId: string, data: { title: string; htmlContent: string; chapterNumber: number; isPublished: boolean }) =>
                    (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/chapters/${bookId}`, {
                        method: 'POST',
                        body: JSON.stringify(data),
                        headers: {
                            'Content-Type': 'application/json',
                        },
                    }),
                deleteChapter: (bookId: string, chapterId: string) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/chapters/${bookId}/${chapterId}`, {
                    method: 'DELETE',
                }),
                updateBook: (bookId: string, data: UpdateBookData) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/book/update/${bookId}`, {
                    method: 'PUT',
                    body: data,
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                }),
                uploadImage: (data: FormData) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/images/upload`, {
                    method: 'POST',
                    body: data,
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                }),
                deleteImage: (ids: string[]) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/images/delete`, {
                    method: 'POST',
                    body: JSON.stringify(ids),
                    headers: {
                        'Content-Type': 'application/json',
                    },
                }),
                becomeAuthor: (data: BecomeAuthorData) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/user/become-author`, {
                    method: 'POST',
                    body: JSON.stringify(data),
                    headers: {
                        'Content-Type': 'application/json',
                    },
                }),
                getAdminPanelBooks: (page: number) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/admin-panel/books?page=${page}`),
                deleteBook: (bookId: string) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/book/delete/${bookId}`, {
                    method: 'DELETE',
                }),
                searchAuthors: (query: string) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/user/search?query=${encodeURIComponent(query)}`),
                createAuthor: (data: CreateAuthorData) =>
                    (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/user/create-author`, {
                        method: 'POST',
                        body: JSON.stringify(data),
                        headers: {
                            'Content-Type': 'application/json',
                        },
                    }),
                searchBooksInAdminPanel: (query: string, page: number) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/admin-panel/books/search?query=${encodeURIComponent(query)}&page=${page}`),
                searchBooks: (query: string) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/book/search?query=${encodeURIComponent(query)}`),
                getNewBooks: () => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/book/new`),
                getBooks: (ids: number[]) => (useNuxtApp() as any).$fetchWrapper(`${apiBaseUrl}/api/book/books`, {
                    method: 'POST',
                    body: JSON.stringify(ids),
                    headers: {
                        'Content-Type': 'application/json',
                    },
                }),
            },
        },
    };
});