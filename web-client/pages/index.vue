<template>
  <div>
    <div v-for="section in sections">
      <!-- Index page data -->
      <div class="d-flex flex-column align-center">
        <p class="text-h5">{{section.title}}</p>

        <div>
          <!-- Loop over collections -->
          <v-btn class="mx-1" v-for="item in section.collection" :key="`${section.title}-${item.slug}`"
                 :to="section.routeFactory(item.slug)">{{ item.name }}
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
          {collection: this.techniques, title: "Techniques", routeFactory: slug => `/technique/${slug}`},
          {collection: this.category, title: "Categories", routeFactory: slug => `/category/${slug}`},
          {collection: this.subcategory, title: "Subcategories", routeFactory: slug => `/subcategory/${slug}`},
        ]
      }
    },

    methods: {
      // Making API call
      api(endpoint) {
        // Making get request to our API with provided endpoint, we can call this resource if we are authenticated and authorized with specifidc
        return this.$axios.$get(`/api/techniques/` + endpoint)
          .then(msg => {console.log('msg from API call', msg)})
      }
    },

  }
</script>
