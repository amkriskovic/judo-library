<template>
  <div v-scroll="onScroll">
    <!-- Tabs -->
    <v-tabs v-model="tab" grow>
      <v-tab>Latest</v-tab>
      <v-tab>Top</v-tab>
    </v-tabs>

    <!-- Injecting submission component -> submissions gonna be loaded based on what we are looking at -->
    <submission :submission="submission" v-for="submission in submissions" :key="`submission-${submission.id}`"/>
  </div>
</template>

<script>
import Submission from "@/components/submission";

export default {
  name: "submission-feed",
  components: {Submission},
  props: {
    loadSubmissions: {
      type: Function,
      required: true
    }
  },
  data: () => ({
    submissions: [],
    // Pagination
    cursor: 0,
    tab: 0,
    finished: false,
    loading: false
  }),
  watch: {
    // Watch tab -> data
    'tab': function (newValue) {
      // Resetting
      this.submissions = []
      this.cursor = 0
      this.finished = false
      this.handleSubmissions()
    }
  },
  created() {
    this.handleSubmissions()
  },
  methods: {
    // Handles scrolling -> loading more submissions
    onScroll() {
      if(this.finished || this.loading) return;

      // Pagination, position of screen at the bottom => load more
      const loadMore = document.body.offsetHeight - (window.pageYOffset + window.innerHeight) < 500
      if (loadMore) {
        // Load another round of submissions
        this.handleSubmissions()
      }
    },

    handleSubmissions() {
      this.loading = true

      // When this component get's created, initiate -> loadSubmissions, pass the query() when loading submissions
      return this.loadSubmissions(this.query)
        .then(submissions => {
          console.log('submissions', submissions)
          // Finished with loading
          if (submissions.length === 0) {
            this.finished = true
          } else {
            submissions.forEach(submission => this.submissions.push(submission))

            // After we did 1st round, increment the cursor, we get next step of items
            this.cursor += 10
          }
        })
      .finally(() => this.loading = false)
    }
  },
  computed: {
    // Ordering based on tab value
    order() {
      return this.tab === 0 ? 'latest' :
        this.tab === 1 ? 'top' :
          'latest'
    },
    query() {
      return `?order=${this.order}&cursor=${this.cursor}`
    }
  },
}
</script>

<style scoped>

</style>
