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
import TechniqueList from "@/components/technique-list";
import ItemContentLayout from "@/components/item-content-layout";

export default {
  // Injected components
  components: {ItemContentLayout, TechniqueList},

  computed: {
    ...mapState("library", ["lists", "dictionary"]),
    techniques() {
      const categorySlug = this.$route.params.category;
      return this.dictionary.categories[categorySlug]
        .techniques
        .map(x => this.dictionary.techniques[x])
    },

    category() {
      return this.dictionary.categories[this.$route.params.category]
    },
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
