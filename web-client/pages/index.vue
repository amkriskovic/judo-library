<template>
  <div>

    <!-- Login section -->
    <div>
      <v-btn @click="login">Login</v-btn>
      <v-btn @click="api('protected')">Api test auth</v-btn>
      <v-btn @click="api('mod')">Api Mod auth</v-btn>
    </div>

    <div v-for="section in sections">
      <!-- Index page data -->
      <div class="d-flex flex-column align-center">
        <p class="text-h5">{{section.title}}</p>

        <div>
          <!-- Loop over collections -->
          <v-btn class="mx-1" v-for="item in section.collection" :key="`${section.title}-${item.id}`"
                 :to="section.routeFactory(item.id)">{{ item.name }}
          </v-btn>
        </div>
      </div>

      <v-divider class="my-5"></v-divider>
    </div>

  </div>
</template>

<script>
  import {mapState} from "vuex"
  import {UserManager, WebStorageStateStore} from "oidc-client"

  export default {
    // Local data
    data: () => ({
      // Init user manager
      userManager: null
    }),

    // * Techniques, categories, subcategories are getting fetched from index.js
    computed: {
      ...mapState("techniques", ["techniques", "category", "subcategory"]),

      // Function that returns array of objects of data from techniques store state
      sections() {
        return [
          // Collection is ARRAY of techniques || categories || subcategories and we can access their model props, like id
          // /technique corresponds to folder _technique
          {collection: this.techniques, title: "Tricks", routeFactory: id => `/technique/${id}`},
          {collection: this.category, title: "Categories", routeFactory: id => `/category/${id}`},
          {collection: this.subcategory, title: "Subcategories", routeFactory: id => `/subcategory/${id}`},
        ]
      }
    },

    created() {
      // If we are not on the server -> we are on the Client
      if(!process.server) {
        // Instantiate user manager and provide required setting, as well as some optional settings
        this.userManager = new UserManager({
          // The URL of the OIDC/OAuth2 provider.
          authority: "http://localhost:5000",

          // Your client application's identifier as registered with the OIDC/OAuth2 provider.
          client_id: "web-client",

          // The redirect URI of your client application to receive a response from the OIDC/OAuth2 provider.
          redirect_uri: "http://localhost:3000",

          // The type of response desired from the OIDC/OAuth2 provider.
          response_type: "code",

          // The scope is being requested from the OIDC/OAuth2 provider.
          // * All the resources hidden behind our API, scope is gonna be specifically for the policy that's protecting it
          scope: "openid profile IdentityServerApi",

          // The OIDC/OAuth2 post-logout redirect URI.
          post_logout_redirect_uri: "http://localhost:3000",

          // The URL for the page containing the code handling the silent renew.
          // silent_redirect_uri:

          // Storage object used to persist User for currently authenticated user.
          // Saving user to the local storage rather than session storage, so it persists after we close the browser
          // * Session storage is temporary, after we close the browser it's deleted -> loosing authentication
          // * Solution => store the tokens in the local storage
          userStore: new WebStorageStateStore({ store: window.localStorage }),
        })

        // Load the User object for the currently authenticated user
        // * This is only get triggered if we are already authenticated and SIGNED IN
        this.userManager.getUser()
          .then(user => {
            // If we have a user
            if(user) {
              console.log('User from local storage [already signed in] ', user)
              // * Setting the token to user that's in local storage
              // this.$axios.setToken(`Bearer ${user.access_token}`)
            }
          })

        // * Extract query parameters from route -> this is the part where we got the code from IS4 and got redirected back to the client
        const {code, scope, session_state, state} = this.$route.query

        // If we have all these parameters
        if(code && scope && session_state && state){
          // * Returns promise to process response from the authorization endpoint (IS4)
          this.userManager.signinRedirectCallback()
            .then(user => {
              // * The result of the promise is the AUTHENTICATED User
              // We get the user from the promise which contains: access_token, id_token, profile obj(user info)., scope and many more...

              // Token has it's own endpoint => "token_endpoint": "http://localhost:5000/connect/token" which contains access_token, id_token...
              // access_token === authorization, id_token === authentication

              // UserInfo has it's own endpoint => "userinfo_endpoint": "http://localhost:5000/connect/userinfo", fetched separately
              // userinfo === profile
              console.log('User after we authenticate ',user)

              // * Setting the token after we are AUTHENTICATED
              this.$axios.setToken(`Bearer ${user.access_token}`)

              // Clear the URL which contains code after redirecting back to the client
              this.$router.push('/')
            })
        }
      }

    },

    methods: {
      // Login method
      login() {
        // Returns promise to trigger a redirect of the current window to the authorization endpoint.
        // * "authorization_endpoint": "http://localhost:5000/connect/authorize", get's attached to /Account/Login?ReturnUrl=...
        return this.userManager.signinRedirect()
      },

      // Making API call
      api(endpoint) {
        // Making get request to our API with provided endpoint, we can call this resource if we are authenticated and authorized with specifidc
        return this.$axios.$get(`/api/techniques/` + endpoint)
          .then(msg => {console.log('msg from API call', msg)})
      }
    },

  }
</script>
