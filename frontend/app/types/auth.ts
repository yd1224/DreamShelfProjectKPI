export type AuthResponse = {
    accessToken: string
}

export type User = {
    id: number
    name: string
    email: string
    createdAt?: string
    updatedAt?: string
}

export interface BecomeAuthorData {
    firstName: string | null;
    lastName: string | null;
    pseudonym: string | null;
    birthDate: string | null | Date;
}

export interface CreateAuthorData {
    firstName: string | null;
    lastName: string | null;
    pseudonym: string | null;
    birthDate: string | null | Date;
    deathDate: string | null | Date;
}
