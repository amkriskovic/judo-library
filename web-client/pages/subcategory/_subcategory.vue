<template>

  <div>
    <div>
      <div class="text-h4"> {{ subcategory.name }}</div>
      <div class="text-body-1"> {{ subcategory.description }}</div>
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
  components: {ItemContentLayout, TechniqueList},

  computed: {
    ...mapState("techniques", ["lists","dictionary"]),
    techniques() {
      // console.log(this.lists.techniques.filter(x => x.subCategory === subcategoryId).map(x => x.category), 'sadasdsad')
      // return this.lists.techniques.filter(x => x.subCategory === subcategoryId).map(x => x.category)

      const subcategoryId = this.$route.params.subcategory;
      return this.lists.techniques.filter(x => x.subCategory === subcategoryId)
    },
    subcategory() {
      const subcategoryId = this.$route.params.subcategory;
      return this.dictionary.subcategories[subcategoryId]
    },
  },

  // Setting via head method the HTML Head tags for the current page.
  head() {
    // If there is no subcategory return empty obj
    if (!this.subcategory) return {}

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
