<template>
  <div>
    <div>
      <div class="text-h4"> {{ category.name }}</div>
      <div class="text-body-1"> {{ category.description }}</div>
    </div>

    <v-divider class="my-3"></v-divider>

    <technique-list :techniques="techniques"/>

  </div>
</template>

<script>
import {mapState} from "vuex";
import TechniqueList from "../../components/technique-list";
import ItemContentLayout from "../../components/item-content-layout";

export default {
  // Injected components
  components: {ItemContentLayout, TechniqueList},

  computed: {
    ...mapState("techniques", ["lists", "dictionary"]),
    techniques() {
      const categoryId = this.$route.params.category;
      return this.lists.techniques.filter(x => x.category === categoryId)
    },
    category() {
      const categoryId = this.$route.params.category;
      return this.dictionary.categories[categoryId];
    },
  },

  // Pre-fetching data asynchronously for this particular page
  async fetch() {
    // const categoryId = this.$route.params.category;

    // Getting techniques for particular category(from our Category API controller) (async call)
    // this.techniques = await this.$axios.$get(`/api/categories/${categoryId}/techniques`);
  },

  // Setting via head method the HTML Head tags for the current page.
  head() {
    // If there is no category return empty obj
    if (!this.category) return {}

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
