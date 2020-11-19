<template>
  <v-row>
    <v-col class="d-flex justify-center align-start" v-for="technique in content" :key="`technique-feed-${technique.id}`">
      <v-card width="320" @click="() => $router.push(`/technique/${technique.slug}`)" :ripple="false">
        <v-card-title>{{ technique.name }}</v-card-title>
        <submission v-if="technique.submission" :submission="technique.submission" slim/>
        <v-card-text>{{ technique.description }}</v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import {feed} from "@/components/feed";
import {mapState} from "vuex";
import Submission from "@/components/submission";

export default {
  name: "front-page-technique-feed",
  components: {Submission},
  mixins: [feed('')],
  fetch() {
    return this.loadContent()
  },
  methods: {
    loadContent() {
      const maxRange = this.lists.techniques.length
      let to = this.cursor + this.limit
      if (to >= maxRange) {
        to = maxRange
      }
      const techniques = this.lists.techniques.slice(this.cursor, to)
      this.cursor += this.limit
      return Promise.all(techniques.map(technique => this.$axios
        .$get(`/api/techniques/${technique.slug}/best-submission`)
        .then(submission => this.content.push({
          ...technique,
          submission
        }))))
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
