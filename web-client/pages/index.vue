<template>
  <div>

    <!-- Place where techniques are displayed -->
    <div v-if="techniques">
      <div v-for="technique in techniques">
        <v-btn :to="`/technique/${technique.id}`">{{ technique.name }}</v-btn>
      </div>
    </div>

  </div>
</template>

<script>
  import {mapState} from "vuex"

  export default {
    // * Techniques are getting fetched from index.js
    computed: mapState("techniques", ["techniques"])
  }
</script>
