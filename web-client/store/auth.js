// Doing user init on client only -> user gonna get populated only when we return to the client

// Local state
const initState = () => ({
  user: null,
  loading: true // At start it's loading => process of initializing Auth
})

export const state = initState

// Roles
const ROLES = {
  MODERATOR: "Mod"
}

export const getters = {
  // If it's not loading && is user is not null => we are authenticated
  authenticated: (state) => !state.loading && state.user != null,

  // If User is authenticated first && he is moderator
  moderator: (state, getters) => getters.authenticated && state.user.profile.role === ROLES.MODERATOR
}

export const mutations = {
  saveUser(state, {user}) {
    // Save user, to initial state user
    state.user = user
  },

  // Authentication initialization has finished? => not loading anymore
  finish(state) {
    state.loading = false
  }
}

export const actions = {
  initialize({commit}) {
    // Load the User object for the currently authenticated user
    // * This is only get triggered if we are already authenticated and SIGNED IN
    // * $auth is nuxt plugin - client-init.js -> injecting UserManager
    return this.$auth.getUser()
      .then(user => {
        // If we have a user
        if (user) {
          console.log('User from local storage [already signed in] ', user)

          // Commit user obj to saveUser mutation which will write to state => user is basically bih token
          commit('saveUser', {user})

          // * Setting the token to user that's in local storage
          this.$axios.setToken(`Bearer ${user.access_token}`)
        }
      })
      // Finally, doesn't matter what result is, commit finish -> stop loading
      .finally(() => commit('finish'))

  }
}
