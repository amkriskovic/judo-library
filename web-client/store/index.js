// Root store function, with initial state - acts like "factory"
const initState = () => ({})

// Export initState function as constant state
export const state = initState;

// Mutations (sync) - object with functions <- that will change the state
// When write to state, use mutations
export const mutations = {

  // Resets state to initial state
  reset(state) {
    Object.assign(state, initState())
  },

}

// Actions (async) - object with functions <- doing async operations
// When handling API calls, use actions
export const actions = {

  // nuxtServerInit only does server side job, pulls data once
  async nuxtServerInit({dispatch}) {
    // Init authentication 1st
    await dispatch("auth/initialize");

    // Pre-load all available tricks, returning Promise...
    await dispatch("library/loadContent");
  },

}

