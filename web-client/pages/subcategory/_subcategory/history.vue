<template>
  <v-list>
    <v-list-item v-for="subcategory in subcategories" :key="`subcategory-history-${subcategory.id}`">
      <v-list-item-title>{{ subcategory.name }}</v-list-item-title>
    </v-list-item>
  </v-list>
</template>

<script>
export default {
  name: "history",
  data: () => ({
    subcategories: []
  }),
  fetch() {
    return this.$axios.$get(`/api/subcategories/${this.$route.params.subcategory}/history`)
      .then(subcategories => this.subcategories = subcategories)
  }
}
</script>

<style scoped>
</style>
