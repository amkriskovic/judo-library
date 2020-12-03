<template>
  <div v-scroll="onScroll">
    <!-- Tabs -->
    <v-tabs v-model="tab" grow>
      <v-tab>Latest</v-tab>
      <v-tab>Top</v-tab>
    </v-tabs>

    <!-- Injecting submission component -> submissions gonna be loaded based on what we are looking at -->
    <submission class="my-3" :submission="c" v-for="c in content" :key="`submission-${c.id}`"/>
  </div>
</template>

<script>
import Submission from "@/components/submission";
import {feed} from "@/components/feed";

export default {
  name: "submission-feed",

  mixins: [feed('latest')],

  components: {Submission},

  data: () => ({
    tab: 0,
  }),

  async fetch() {
    await this.loadContent()

    if (this.$route.query.submission) {
      const submission = await this.$axios.$get(`/api/submissions/${this.$route.query.submission}`)

      const existingIndex = this.content.map(x => x.id).indexOf(submission.id)
      if (existingIndex > -1){
        this.content.splice(existingIndex, 1)
      }

      this.content.unshift(submission)
    }
  },

  watch: {
    // Watch tab -> data
    'tab': function (newValue) {
      this.order = newValue === 0 ? 'latest' :
        newValue === 1 ? 'top' :
          'latest'
    },
  },

}
</script>

<style scoped>

</style>
