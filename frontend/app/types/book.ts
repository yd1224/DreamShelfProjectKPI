export interface Author {
  id: number
  displayName: string
}

export interface Genre {
  id: number
  name: string
}

export interface Chapter {
  uuid: string
  title: string
  chapterNumber: number
  lastUpdatedAt: string
}

export interface Book {
  id: number
  title: string
  description: string
  coverImageUrl?: string
  publicationYear?: number
  bookStatus: number
  isPublished: boolean
  ageRestriction: number
  lastUpdatedAt: string
  authors: Author[]
  genres: Genre[]
  chapters: Chapter[]
}

export interface BookStats {
  totalChapters: number
}

export interface UpdateBookData {
  title: string
  description: string
  coverImageUrl: string
  bookStatus: number
  ageRestriction: number
  isPublished: boolean
  genres: number[]
  authorIds: number[]
}

export interface FormBookData {
  title: string
  description: string
  coverImageUrl: string | null
  bookStatus: string
  ageRestriction: number | null
  genres: number[]
  documentFile: File | null
  isPublished: boolean
  documentHeading: string
  authorIds: Author[]
}
