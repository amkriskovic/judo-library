// Doing user init on client only -> user gonna get populated only when we return to the client

// Local state
const initState = () => ({
  user: null,
  profile: null,
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

  saveProfile(state, {profile}) {
    // Save profile, to initial state profile
    state.profile = profile
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

    // Query OP for user's current signin status => promise -> it will give us session status
    return this.$auth.querySessionStatus()
      .then(sessionStatus => {
        // Session status gives us infos in obj: {session_state, sid -> which is User profile id, sub -> which is subject => User}
        console.log('sessionStatus: ', sessionStatus)

        // If we have sessionStatus => get/reinitialize the User
        if (sessionStatus) {
          // Returns promise which will give us User obj
          // Get the User again
          return this.$auth.getUser()
        }
      })
      // User
      .then(async user => {
        // If we have the User
        if (user) {
          console.log('User[GOT] from local storage [already signed in] ', user)

          // Commit user obj to saveUser mutation which will write to state => user is basically big token
          commit('saveUser', {user})

          // Set the access_token to local storage
          this.$axios.setToken(`Bearer ${user.access_token}`)

          // Once we set access_token, make HTTP GET request to get our profile -> which will resole in profile obj containing:
          // id, username, submissions(arr), deleted
          const profile = await this.$axios.$get('/api/users/me')

          console.log('store auth.js profile:: ', profile)

          // Writing -> saving profile to state
          commit('saveProfile', {profile})
        }
      })
      .catch(err => {
        console.log('err: ', err.message)
        if (err.message === 'login_required') {
          // Remove from any storage the currently authenticated user
          return this.$auth.removeUser()
        }
      })
      // Finally, doesn't matter what result is, commit finish -> stop loading skeletons
      .finally(() => commit('finish'))
  },

  login() {
    // Fuck off
    if (process.server) return

    // * Before we log in, set the item to local storage, location.pathname => return URL pathname e.g. "/technique/osoto-gari"
    localStorage.setItem('post-login-redirect-path', location.pathname)

    /** Trigger a redirect of the current window to the authorization endpoint */
    return this.$auth.signinRedirect()
  },

  // Action corresponds to what we want to perform once user is initialized/loaded => executed only on client side
  _waitAuthenticated({state, getters}) {
    // Fuck off
    if (process.server) return

    // Return new promise for async purpose
    return new Promise((resolve, reject) => {
      // If we are Loading...
      if (state.loading){

        // Start watching
        console.log('Start watching!')

        const unwatch = this.watch(
          // Inside the store, what do we wanna watch => loading
          (store) => store.auth.loading,


          // New and old value corresponds to value of loading
          (newValue, oldValue) => {
            // Get rid of watcher -> we no longer need it since loading is finished
            unwatch()

            // If loading is false => loading is finished
            resolve(getters.authenticated)
          }
        )
      } else {
        // Else -> User is loaded.
        // Pass action to resolve, whatever action returns it's gonna be resolved value of the Promise
        console.log('User is already loaded => executing action!')
        resolve(getters.authenticated)
      }
    })
  }

}
