// Root store function, with initial state - acts like "factory"
const initState = () => ({
  // Set techniques,category,subcategory to empty array as initial state
  techniques: [],
  category: [],
  subcategory: []
})

// Export initState function as constant state
export const state = initState;

// Getters for techniques,category,subcategory mapping data from initial state
export const getters = {
  // * Lambda that is returning Lambda, returning technique by id
  techniqueById: state => id => state.techniques.find(t => t.id === id),

  categoryById: state => id => state.category.find(c => c.id === id),

  subcategoryById: state => id => state.subcategory.find(sc => sc.id === id),

  techniqueItems: state => state.techniques.map(t => ({
    text: t.name,
    value: t.id
  })),

  categoryItems: state => state.category.map(c => ({
    text: c.name,
    value: c.id
  })),

  subcategoryItems: state => state.subcategory.map(sc => ({
    text: sc.name,
    value: sc.id
  })),
}

// Mutations (sync) - object with functions <- that will change the state
// When write to state, use mutations
export const mutations = {

  setTechniques(state, {techniques, categories, subcategories}) {
    // Assigning techniques from payload to techniques state
    state.techniques = techniques;

    // Assigning category and subcategory from payload to store state
    state.category = categories;
    state.subcategory = subcategories;
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
  async fetchTechniques({commit}) {
    // Await for response from GET request, getting data from response (url)
    const techniques = await this.$axios.$get("/api/techniques");

    // Await for response from GET request, getting data for category and subcategory
    const categories = await this.$axios.$get("/api/categories");
    const subcategories = await this.$axios.$get("/api/subcategories");

    console.log('techniques::', techniques);

    // Trigger a mutation func with commit func and provide payload <- techniques, category, subcategory
    // Mutation function setTechniques renders UI
    commit("setTechniques", {techniques, categories, subcategories});
  },

  // Create technique with accessing state of store, and with payload <- form
  createTechnique({state, commit, dispatch}, {form}) {

    // Make post request to url, send payload <- from, return Promise
    return this.$axios.$post("/api/techniques", form);
  }

}

