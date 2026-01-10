interface ReadingProgress {
  bookId: number
  chapterNumber: number
  chapterId: string
  lastRead: number
}

const STORAGE_KEY = 'dreamshelf_reading_progress'

export const useReadingProgress = () => {
  const getReadingProgress = (): ReadingProgress[] => {
    if (!import.meta.client) return []
    
    try {
      const stored = localStorage.getItem(STORAGE_KEY)
      return stored ? JSON.parse(stored) : []
    } catch (error) {
      console.error('Error reading progress from localStorage:', error)
      return []
    }
  }

  const saveReadingProgress = (bookId: number, chapterId: string, chapterNumber: number) => {
    if (!import.meta.client) return

    try {
      const progress = getReadingProgress()
      const existingIndex = progress.findIndex(p => p.bookId === bookId)

      if (existingIndex !== -1) {
        const existing = progress[existingIndex]!
        if (chapterNumber > existing.chapterNumber) {
          progress[existingIndex] = {
            bookId,
            chapterId,
            chapterNumber,
            lastRead: Date.now()
          }
        }
      } else {
        progress.push({
          bookId,
          chapterId,
          chapterNumber,
          lastRead: Date.now()
        })
      }

      progress.sort((a, b) => b.lastRead - a.lastRead)
      
      const maxItems = 10
      const trimmedProgress = progress.slice(0, maxItems)

      localStorage.setItem(STORAGE_KEY, JSON.stringify(trimmedProgress))
    } catch (error) {
      console.error('Error saving reading progress:', error)
    }
  }

  const getBookProgress = (bookId: number): ReadingProgress | null => {
    const progress = getReadingProgress()
    return progress.find(p => p.bookId === bookId) || null
  }

  const clearReadingProgress = () => {
    if (!import.meta.client) return
    localStorage.removeItem(STORAGE_KEY)
  }

  const removeBookProgress = (bookId: number) => {
    if (!import.meta.client) return

    try {
      const progress = getReadingProgress()
      const filtered = progress.filter(p => p.bookId !== bookId)
      localStorage.setItem(STORAGE_KEY, JSON.stringify(filtered))
    } catch (error) {
      console.error('Error removing book progress:', error)
    }
  }

  return {
    getReadingProgress,
    saveReadingProgress,
    getBookProgress,
    clearReadingProgress,
    removeBookProgress
  }
}
