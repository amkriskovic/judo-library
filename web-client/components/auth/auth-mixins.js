import {mapGetters} from "vuex";

export const GUARD_LEVEL = {
  AUTH: 1,
  MOD: 2,
}

export const guard = (level) => ({
  computed: mapGetters('auth', ['authenticated']),

  beforeRouteEnter(to, from, next) {
    if (process.server) {
      // Delegate, if we are on server just proceed
      next();
    } else {
      // * Browser
      next(nuxt => {
        nuxt.$store.dispatch('auth/_watchUserLoaded', () => {
          // Initial value is false
          let allowed = false

          // If u are authenticated, you are allowed
          if(level === GUARD_LEVEL.AUTH) {
            // Assign to allowed value from auth store getter -> authenticated -> basic User
            allowed = nuxt.$store.getters['auth/authenticated']
          }
          else if(level === GUARD_LEVEL.MOD) {
            // Assign to allowed value from auth store getter -> authenticated -> moderator
            allowed = nuxt.$store.getters['auth/moderator']
          }

          // Init the page => load
          if (allowed) {
            if (nuxt.$fetch) {
              nuxt.$fetch();
            }
          } else {
            // Not allowed -> redirect to login
            nuxt.$store.dispatch('auth/login')
          }

        })
      })
    }

  }
})
