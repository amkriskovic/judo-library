<template>
  <div v-scroll="onScroll">
    <!-- Tabs -->
    <v-tabs v-model="tab" grow>
      <v-tab>Latest</v-tab>
      <v-tab>Top</v-tab>
    </v-tabs>

    <!-- Injecting submission component -> submissions gonna be loaded based on what we are looking at -->
    <submission :submission="c" v-for="c in content" :key="`submission-${c.id}`"/>
  </div>
</template>

<script>
import Submission from "@/components/submission";
import {feed} from "@/components/feed";

export default {
  name: "submission-feed",

  mixins: [feed('')],

  components: {Submission},

  data: () => ({
    tab: 0,
  }),

  watch: {
    // Watch tab -> data
    'tab': {
      handler: function(newValue) {
        this.order = newValue === 0 ? 'latest' :
          newValue === 1 ? 'top' :
            'latest'
      },
      immediate: true
    }
  },

}
</script>

<style scoped>

</style>
