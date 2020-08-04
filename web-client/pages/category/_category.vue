<template>

  <!-- Item-Content component injection -->
  <item-content-layout>
    <!-- Template for Content-->
    <template v-slot:content>
      <!-- Injecting technique list component in order to replace unnecessary HTML | Handles filtering too -->
      <!-- :techniques is props defined in respective component, which marks details about how to bind tricks: [] -->
      <technique-list :techniques="techniques" class="mx-2"/>
    </template>

    <!-- Template for Item(card) -->
    <template v-slot:item>
      <div class="pa-3 mx-2 sticky" v-if="category">
        <div class="text-h6"> {{ category.name }}</div>

        <v-divider class="my-1"></v-divider>

        <div class="text-body-2"> {{ category.description }}</div>
      </div>

    </template>
  </item-content-layout>

</template>

<script>
  import {mapGetters} from "vuex";
  import TechniqueList from "../../components/technique-list";
  import ItemContentLayout from "../../components/item-content-layout";

  export default {
    // Injected components
    components: {ItemContentLayout, TechniqueList},

    // Page local state | data
    data: () => ({
      techniques: [],
      category: null
    }),

    computed: mapGetters("techniques", ["categoryById"]),

    // Pre-fetching data asynchronously for this particular page
    async fetch() {
      // Getting categoryId from URL param
      const categoryId = this.$route.params.category;

      // Invoking categoryById, which get's us particular category based on categoryId we provide,
      // storing result in page state
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
