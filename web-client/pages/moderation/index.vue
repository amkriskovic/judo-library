<template>
  <div>
    <!-- Button for displaying modItem for moderation -->
    <!-- id |> moderation modItem id -->
    <v-btn :to="`/moderation/${modItem.id}`" v-for="modItem in modItems" :key="modItem.id">
      {{modItem.target}} <- Target
    </v-btn>
  </div>
</template>

<script>
  export default {
    // Local state
    data: () => ({
      // Collection of items to moderate
      modItems: []
    }),

    // Every time you need to get asynchronous data. fetch is called on server-side when rendering the route, and on client-side when navigating.
    async fetch() {
      // Grabbing everything from API controller and storing in local state, arr of items
      this.modItems = await this.$axios.$get("/api/moderation-items")
    }
  }
</script>

<style scoped>

</style>
