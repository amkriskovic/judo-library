// Produce the endpoint based on url type parameter, e.g. techniques
const endpointResolver = (type) => {
  // If type is technique, returns techniques string which we will use for resolving our API endpoint
  if (type === 'technique') return 'techniques'
  if (type === 'category') return 'categories'
}

// Reviews statuses, mimicking backend enum
export const REVIEW_STATUS = {
  APPROVED: 0,
  REJECTED: 1,
  WAITING: 2
}

// Depending on review status returns color corresponding to that status
const reviewStatusColor = (status) => {
  if (REVIEW_STATUS.APPROVED === status) return "green"
  if (REVIEW_STATUS.REJECTED === status) return "red"
  if (REVIEW_STATUS.WAITING === status) return "orange"
  return ''
}


// Depending on review status returns icon corresponding to that status
const reviewStatusIcon = (status) => {
  if (REVIEW_STATUS.APPROVED === status) return "mdi-check"
  if (REVIEW_STATUS.REJECTED === status) return "mdi-close"
  if (REVIEW_STATUS.WAITING === status) return "mdi-clock"
  return ''
}

export const modItemRenderer = {
  methods: {
    endpointResolver,
    reviewStatusColor,
    reviewStatusIcon,
  }
}
