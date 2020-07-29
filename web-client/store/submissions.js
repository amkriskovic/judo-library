// Root store function, with initial state
const initState = () => ({
  submissions: []
})

export const state = initState;

export const mutations = {

  // We are getting submissions from payload
  setSubmissions(state, {submissions}) {
    state.submissions = submissions;
  },

  // Resets state to initial state
  reset(state) {
    Object.assign(state, initState())
  }

}

export const actions = {

  // Fetching submissions for particular technique, payload <- techniqueId
  async fetchSubmissionsForTechnique({commit}, {techniqueId}) {
    // Await for response from GET request and getting data from response (url)
    const submissions = await this.$axios.$get(`/api/techniques/${techniqueId}/submissions`);

    console.log('submissions: ', submissions)

    commit("setSubmissions", {submissions});
  },

  // Create submission with accessing state of store, and with payload <- form
  createSubmission({state, commit, dispatch}, {form}) {
    // Make post request to url, send payload <- form, return Promise
    return this.$axios.$post("/api/submissions", form);
  }

}
