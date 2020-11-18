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
  moderator: (state, getters) => getters.authenticated && state.profile.isMod
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
      .catch(() => {
      })
  },

  login() {
    // Fuck off
    if (process.server) return

    const returnUrl = encodeURIComponent(location.href)

    window.location = `${this.$config.auth.loginPath}?returnUrl=${returnUrl}`
  },

  logout() {
    // Fuck off
    if (process.server) return

    window.location = this.$config.auth.logoutPath
  },

}
