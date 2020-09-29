<template>
  <div>

    <div>
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

  export default {
    // * Techniques, categories, subcategories are getting fetched from index.js
    computed: {
      ...mapState("techniques", ["techniques", "category", "subcategory"]),

      // Function that returns array of objects of data from techniques store state
      sections() {
        return [
          // Collection is ARRAY of techniques || categories || subcategories and we can access their model props, like id
          // /technique corresponds to folder _technique
          {collection: this.techniques, title: "Techniques", routeFactory: id => `/technique/${id}`},
          {collection: this.category, title: "Categories", routeFactory: id => `/category/${id}`},
          {collection: this.subcategory, title: "Subcategories", routeFactory: id => `/subcategory/${id}`},
        ]
      }
    },

    methods: {
      // Login method
      login() {
        // Returns promise to trigger a redirect of the current window to the authorization endpoint.
        // * "authorization_endpoint": "http://localhost:5000/connect/authorize", get's attached to /Account/Login?ReturnUrl=...
        // * $auth is referencing clint-init.js plugin which injects UserManager
      },

      // Logout method
      logout: function () {
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
