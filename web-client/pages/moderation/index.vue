<template>
  <div>
    <!-- Button for displaying item for moderation, target is technique -->
    <v-btn :to="`/moderation/${item.type}/${item.target}`" v-for="item in items" :key="item.id">
      {{item.target}}
    </v-btn>
  </div>
</template>

<script>
  export default {
    // Local state
    data: () => ({
      items: []
    }),

    // Every time you need to get asynchronous data. fetch is called on server-side when rendering the route, and on client-side when navigating.
    async fetch() {
      // Grabbing everything from API controller and storing in local state, arr of items
      this.items = await this.$axios.$get("/api/moderation-items")
    }
  }
</script>

<style scoped>

</style>
