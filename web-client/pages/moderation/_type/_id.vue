<template>
  <div>
    <!-- Check for item -->
    <div v-if="item">
      <!-- Display item's description -->
      {{item.description}}
    </div>
  </div>
</template>

<script>
  // Separate from component, easier to re-use it
  // Resolves endpoint based on type it's passed
  const endpointResolver = (type) => {
    // If type is technique, returns techniques string which we will use for resolving our API endpoint
    if (type === 'technique') return 'techniques'
  }

  export default {
    // Local state
    data: () => ({
      item: null
    }),

    // Every time you need to get asynchronous data. fetch is called on server-side when rendering the route, and on client-side when navigating.
    async fetch() {
      // We want extract data that we passed in from url params in /moderation/{}/{}
      const {type, id} = this.$route.params

      // Produce the endpoint based on url type parameter, e.g. techniques
      const endpoint = endpointResolver(type)

      // Assign endpoint to the item in local state
      // Get dynamic API controller => response - data, based on URL parameters that we extracted
      this.item = await this.$axios.$get(`/api/${endpoint}/${id}`)
    }
  }
</script>

<style scoped>

</style>
