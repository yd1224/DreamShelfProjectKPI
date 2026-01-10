export const useConstants = () => {
  const ages = [
    { id: 0, name: 'Kids', value: "0-12" },
    { id: 1, name: 'Teen', value: "12-18" },
    { id: 2, name: 'Adult', value: "18+" },
    { id: 3, name: 'All ages', value: "0+" },
  ]

  const bookStatus = [
    { id: 0, name: 'Announced' },
    { id: 1, name: 'Ongoing' },
    { id: 2, name: 'Completed' },
    { id: 3, name: 'Frozen' },
  ]

  return {
    ages,
    bookStatus
  }
}