<template>
  <div>
    <v-row justify="space-around">
      <v-col lg="3" class="d-flex justify-center align-start" v-for="technique in content" :key="`technique-feed-${technique.id}`">
        <v-card width="320" @click="() => $router.push(`/technique/${technique.slug}`)" :ripple="false">
          <v-card-title>{{ technique.name }}</v-card-title>
          <v-divider />
          <submission v-if="technique.submission"
                      :submission="technique.submission"
                      slim
                      elevation="0"/>
          <v-card-text>{{ technique.description }}</v-card-text>
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
  name: "front-page-technique-feed",
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
      const maxRange = this.lists.techniques.length
      let to = this.cursor + this.limit
      if (to >= maxRange) {
        to = maxRange
        this.finished = true
      }
      const techniques = this.lists.techniques.slice(this.cursor, to)
      this.cursor += this.limit
      const techniqueRequests = techniques.map(technique => this.$axios
        .$get(`/api/submissions/best-submission?byTechniques=${technique.slug}`)
        .then(submission => this.content.push({
          ...technique,
          submission
        })))
      return Promise.all(techniqueRequests)
    }
  },
  computed: mapState('library', ['lists'])
}
</script>

<style scoped>
.v-card--link:focus:before {
  opacity: 0;
}
</style>
