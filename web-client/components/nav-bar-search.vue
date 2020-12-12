<template>
  <v-autocomplete rounded
                  dense
                  filled
                  hide-details
                  append-icon=""
                  prepend-inner-icon="mdi-magnify"
                  :items="items"
                  :filter="searchFilter"
  >
    <template v-slot:item="{item, on, attrs}">
      <v-list-item v-on="on" :attrs="attrs" :to="item.route">
        <v-list-item-content>{{ item.name }}</v-list-item-content>
        <v-spacer/>
        <v-list-item-content class="justify-end">{{ item.type }}</v-list-item-content>
      </v-list-item>
    </template>
  </v-autocomplete>
</template>

<script>
import {mapState} from "vuex";
import {hasOccurrences} from "@/data/functions";

const itemFactory = (name, type, slug) => ({
  name,
  type,
  route: `/${type}/${slug}`,
  searchIndex: (name + type).toLowerCase(),
  text: name,
})
export default {
  name: "nav-bar-search",
  methods: {
    searchFilter(item, queryText, itemText) {
      return hasOccurrences(item.searchIndex, queryText)
    }
  },
  computed: {
    ...mapState('library', ['lists']),
    items() {
      return []
        .concat(this.lists.techniques.map(x => itemFactory(x.name, 'technique', x.slug)))
        .concat(this.lists.categories.map(x => itemFactory(x.name, 'category', x.id)))
        .concat(this.lists.subcategories.map(x => itemFactory(x.name, 'subcategory', x.id)))
    }
  }
}
</script>

<style scoped>
</style>
