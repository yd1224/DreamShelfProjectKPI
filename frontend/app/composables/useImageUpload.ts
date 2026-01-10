export const useImageUpload = () => {
  const { $api } = useNuxtApp()

  /**
   * Upload an image file to the server
   * @param file - The image file to upload
   * @returns The uploaded fileName or null if failed
   */
  const uploadImage = async (file: File): Promise<string | null> => {
    try {
      const formData = new FormData()
      formData.append('image', file)
      
      const response = await $api.uploadImage(formData)
      return response
    } catch (error) {
      console.error('Error uploading image:', error)
      return null
    }
  }

  /**
   * Delete an image from the server
   * @param fileName - The fileName to delete
   */
  const deleteImage = async (fileNames: string[]): Promise<boolean> => {
    try {
      await $api.deleteImage(fileNames)
      console.log('Deleted image:', fileNames)
      return true
    } catch (error) {
      console.error('Error deleting image:', error)
      return false
    }
  }

  /**
   * Get the full image URL from fileName
   * @param fileName - The fileName
   * @returns Full URL to the image
   */
  const getImageUrl = (fileName: string): string => {
    return `http://localhost:5153/images/${fileName}`
  }

  /**
   * Validate image file
   * @param file - File to validate
   * @returns Object with isValid flag and error message
   */
  const validateImageFile = (file: File): { isValid: boolean; error?: string } => {
    const allowedTypes = ['image/png', 'image/jpeg', 'image/jpg', 'image/webp']
    const maxSizeInBytes = 5 * 1024 * 1024 // 5MB

    if (!allowedTypes.includes(file.type)) {
      return {
        isValid: false,
        error: 'Only PNG, JPEG, and WebP images are allowed.'
      }
    }

    if (file.size > maxSizeInBytes) {
      return {
        isValid: false,
        error: 'Image size must be less than 5MB.'
      }
    }

    return { isValid: true }
  }

  return {
    uploadImage,
    deleteImage,
    getImageUrl,
    validateImageFile
  }
}
