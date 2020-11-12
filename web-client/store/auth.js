// Doing user init on client only -> user gonna get populated only when we return to the client

// Local state
const initState = () => ({
  profile: null,
})

export const state = initState

// Roles
const ROLES = {
  MODERATOR: "Mod"
}

export const getters = {
  // If it's not loading && is user is not null => we are authenticated
  authenticated: (state) => state.profile != null,

  // If User is authenticated first && he is moderator
  moderator: (state, getters) => getters.authenticated && state.profile === state.profile.isMod
}

export const mutations = {
  saveProfile(state, {profile}) {
    // Save profile, to initial state profile
    state.profile = profile
  },
}

export const actions = {
  initialize({commit}) {
    return this.$axios.$get('/api/users/me')
      .then(profile => commit('saveProfile', {profile}))
      .catch(e => {
        console.error('Loading profile error', e.response)
      })
  },

  login() {
    // Fuck off
    if (process.server) return

    // * Before we log in, set the item to local storage, location.pathname => return URL pathname e.g. "/technique/osoto-gari"
    localStorage.setItem('post-login-redirect-path', location.pathname)

    window.location = this.$config.auth.loginPath
  },

}
