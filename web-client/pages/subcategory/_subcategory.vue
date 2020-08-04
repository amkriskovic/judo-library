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
      <div v-if="subcategory">
        <div class="text-h6"> {{ subcategory.name }}</div>

        <v-divider class="my-1"></v-divider>

        <div class="text-body-2"> {{ subcategory.description }}</div>
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
      filter: "",
      techniques: [],
      subcategory: null
    }),

    computed: mapGetters("techniques", ["subcategoryById"]),

    // Pre-fetching data asynchronously for this particular page
    async fetch() {
      // Getting subcategoryId from URL param
      const subcategoryId = this.$route.params.subcategory;

      // Invoking subcategoryById, which get's us particular subcategory based on subcategoryId we provide,
      // storing result in page state
      this.subcategory = this.subcategoryById(subcategoryId);

      // Getting techniques for particular subcategory(from our subcategories API controller) (async call) | Fills page state
      this.techniques = await this.$axios.$get(`/api/subcategories/${subcategoryId}/techniques`);
    },

    // Setting via head method the HTML Head tags for the current page.
    head() {
      // If there is no subcategory return empty obj
      if(!this.subcategory) return {}

      return {
        title: this.subcategory.name,
        meta: [
          {hid: 'description', name: 'description', content: this.subcategory.description}
        ]
      }
    }

  }
</script>

<style scoped>

</style>
