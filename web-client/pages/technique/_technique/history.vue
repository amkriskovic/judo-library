<template>
  <v-list>
    <v-list-item v-for="technique in techniques" :key="`technique-history-${technique.id}`">
      <v-list-item-title>{{ technique.name }}</v-list-item-title>
    </v-list-item>
  </v-list>
</template>

<script>
export default {
  name: "history",
  data: () => ({
    techniques: []
  }),
  fetch() {
    return this.$axios.$get(`/api/techniques/${this.$route.params.technique}/history`)
      .then(techniques => this.techniques = techniques)
  }
}
</script>

<style scoped>
</style>
