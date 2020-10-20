<template>
  <div>
    <div v-for="section in sections">
      <!-- Index page data -->
      <div class="d-flex flex-column align-center">
        <p class="text-h5">{{section.title}}</p>

        <div>
          <!-- Loop over collections -->
          <v-btn class="mx-1" v-for="item in section.collection"
                 :key="`${section.title}-${item.id}`"
                 :to="section.routeFactory(item)">{{ item.name }}
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
      // Importing lists from initial state of techniques store
      ...mapState("techniques", ["lists"]),

      // Function that returns array of objects of data from techniques store state
      sections() {
        return [
          // Collection is ARRAY of techniques || categories || subcategories and we can access their model props, like id
          // /technique corresponds to folder _technique, i => item, only technique uses slug
          {collection: this.lists.techniques, title: "Techniques", routeFactory: i => `/technique/${i.slug}`},
          {collection: this.lists.categories, title: "Categories", routeFactory: i => `/category/${i.id}`},
          {collection: this.lists.subcategories, title: "Subcategories", routeFactory: i => `/subcategory/${i.id}`},
        ]
      }
    },
  }
</script>
