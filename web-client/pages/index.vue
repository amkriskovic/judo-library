<template>
  <div>

    <div v-for="section in sections">
      <!-- Index page data -->
      <div class="d-flex flex-column align-center">
        <p class="text-h5">{{section.title}}</p>

        <div>
          <!-- Loop over collections -->
          <v-btn class="mx-1" v-for="modItem in section.collection"
                 :key="`${section.title}-${item.id}`"
                 :to="section.routeFactory(item.id)">{{ modItem.name }}
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
          {collection: this.techniques, title: "Tricks", routeFactory: id => `/technique/${id}`},
          {collection: this.category, title: "Categories", routeFactory: id => `/category/${id}`},
          {collection: this.subcategory, title: "Subcategories", routeFactory: id => `/subcategory/${id}`},
        ]
      }
    }
  }
</script>
