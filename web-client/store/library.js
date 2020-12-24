// Root store function, with initial state - acts like "factory"
const initState = () => ({
  // Dict -> for fast look up -> indexing
  // Set techniques,category,subcategory to null as initial state
  dictionary: {
    techniques: null,
    categories: null,
    subcategories: null
  },

  // Set techniques,category,subcategory to empty array as initial state
  lists: {
    techniques: [],
    categories: [],
    subcategories: []
  },
})

// Export initState function as constant state
export const state = initState;

// Func for setting entities data that's retrieved from API, type is what we accessing, data === payload
const setEntities = (state, type, data) => {
  // Initialize dict for particular type to empty obj.
  state.dictionary[type] = {}
  state.lists[type] = []

  // Loop over data
  data.forEach(t => {
    // Push particular type from data to arr of types => for front page where we load collection
    state.lists[type].push(t)

    // Dynamically resolve type from dict and assign data which is coming from some data collection to particular type id
    state.dictionary[type][t.id] = t

    // Assign it to dict of specific type
    state.dictionary[type][t.slug] = t
  })
}

// Mutations (sync) - object with functions <- that will change the state
// When write to state, use mutations
export const mutations = {

  // Writing to state techniques that get's retrieved from our API
  setTechniques(state, {techniques}) {
    // Calling set entities with state, type which is acting like key and data as value for that key
    setEntities(state, 'techniques', techniques)
  },

  setCategories(state, {categories}) {
    setEntities(state, 'categories', categories)
  },

  setSubCategories(state, {subcategories}) {
    setEntities(state, 'subcategories', subcategories)
  },

  // Resets state to initial state
  reset(state) {
    Object.assign(state, initState())
  },

}

// Actions (async) - object with functions <- doing async operations
// When handling API calls, use actions
export const actions = {

  // # Base URL is set in nuxt.config.js
  // Fetching techniques, commit <- context of store, commit func is to invoke one of the mutation func's
  loadContent({commit}) {
    // Promise for making all 3 calls to API at the "same" time
    return Promise.all([
      // Passing array of Tasks => API cals to fetch data

      // Getting techniques, then commiting to write to store state
      this.$axios.$get("/api/techniques").then(techniques => commit("setTechniques", {techniques})),
      this.$axios.$get("/api/categories").then(categories => commit("setCategories", {categories})),
      this.$axios.$get("/api/subcategories").then(subcategories => commit("setSubCategories", {subcategories})),
    ])
  },

}

