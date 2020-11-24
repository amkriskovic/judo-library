<template>
  <div>
    <v-row justify="space-around">
    <v-col lg="3" class="d-flex justify-center align-start" v-for="subcategory in content"
           :key="`subcategory-feed-${subcategory.id}`">
      <v-card width="320" @click="() => $router.push(`/subcategory/${subcategory.id}`)" :ripple="false">
        <v-card-title>{{ subcategory.name }}</v-card-title>
        <v-divider />
        <submission v-if="subcategory.submission" :submission="subcategory.submission" slim elevation="0"/>
        <v-card-text>{{ subcategory.description }}</v-card-text>
      </v-card>
    </v-col>
    </v-row>

    <div v-if="!finished" class="d-flex justify-center">
      <v-btn @click="loadContent">Load More</v-btn>
    </div>
  </div>
</template>

<script>
import {feed} from "@/components/feed";
import {mapState} from "vuex";
import Submission from "@/components/submission";

export default {
  name: "front-page-subcategory-feed",
  components: {Submission},
  mixins: [feed('')],
  data: () => ({
    limit: 8
  }),
  fetch() {
    return this.loadContent()
  },
  methods: {
    loadContent() {
      const maxRange = this.lists.subcategories.length
      let to = this.cursor + this.limit
      if (to >= maxRange) {
        to = maxRange
        this.finished = true
      }
      const subcategories = this.lists.subcategories.slice(this.cursor, to)
      this.cursor += this.limit

      const byTechniques = (x) => x.techniques.reduce((a, b) => `${a};${b}`, "")

      const submissionRequests = subcategories.map(subcategory => {
        if (subcategory.techniques.length > 0) {
          return this.$axios
            .$get(`/api/submissions/best-submission?byTechniques=${byTechniques(subcategory)}`)
            .then(submission => this.content.push({
              ...subcategory,
              submission
            }))
        } else {
          this.content.push(subcategory)
        }
      })

      return Promise.all(submissionRequests)
    }
  },
  computed: mapState('techniques', ['lists'])
}
</script>

<style scoped>
.v-card--link:focus:before {
  opacity: 0;
}
</style>
