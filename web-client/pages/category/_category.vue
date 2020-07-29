<template>
  <div class="d-flex mt-3 justify-center align-start">

    <div class="mx-2">
      <!-- Input text field, mdi -> material design icons - {name of icon}, two way binding -> filter -->
      <v-text-field label="Search" placeholder="e.g. throw/reap/drop" v-model="filter"
                    prepend-inner-icon="mdi-magnify" outlined>

      </v-text-field>
      <!-- Iterating over filtered Techniques -->
      <div v-for="technique in filteredTechniques">
        {{ technique.name }} - {{ technique.description }}
      </div>
    </div>

    <!-- Component for http://localhost:3000/category/{everything that comes after that (slug)} -->
    <v-sheet class="pa-3 mx-2 sticky" v-if="category">
      <div class="text-h6"> {{ category.name }}</div>

      <v-divider class="my-1"></v-divider>

      <div class="text-body-2"> {{ category.description }}</div>
    </v-sheet>

  </div>
</template>

<script>
  import {mapGetters} from "vuex";

  export default {
    // Page data
    data: () => ({
      filter: "",
      techniques: [],
      category: null
    }),

    computed: {
      ...mapGetters("techniques", ["categoryById"]),

      // Function that returns filtered techniques based on search | Mutating page state | Interacting with page data
      filteredTechniques() {
        // If there is no filter(search) applied, just return original tricks
        if (!this.filter) return this.techniques;

        // Normalize/Sanitize filter(search) input/string
        const normalized = this.filter.trim().toLowerCase();

        // Returning filtered techniques array -> name and description, to lower both, passing normalized string that came from search input (v-model)
        return this.techniques.filter(t => t.name.toLowerCase().includes(normalized) || t.description.toLowerCase().includes(normalized));
      }
    },

    // Pre-fetching data asynchronously for this particular page
    async fetch() {
      // Getting categoryId from URL param
      const categoryId = this.$route.params.category;

      // Invoking categoryById, which get's us particular category based on categoryId we provide, storing result in component state
      this.category = this.categoryById(categoryId);

      // Getting techniques for particular category(from our Category API controller) (async call)
      this.techniques = await this.$axios.$get(`/api/categories/${categoryId}/techniques`);
    },

    // Setting via head method the HTML Head tags for the current page.
    head() {
      // If there is no category return empty obj
      if(!this.category) return {}

      return {
        title: this.category.name,
        meta: [
          {hid: 'description', name: 'description', content: this.category.description}
        ]
      }
    }

  }
</script>

<style scoped>

</style>
