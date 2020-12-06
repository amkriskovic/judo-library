<template>
  <div>
    <h6 class="text-h6 mb-2">Completed Techniques ({{completedTechniques.length}} / {{lists.techniques.length}})</h6>
    <v-chip class="mb-1 mr-1" small
            v-for="{submission, technique} in completedTechniques"
            @click="gotoSubmission(technique.slug, submission.id)"
            :key="`profile-technique-chip-${submission.id}`">
      {{ technique.name }}
    </v-chip>
  </div>
</template>

<script>
import {mapState} from "vuex";

export default {
  name: "profile-completed-techniques",
  props: {
    profileSubmissions: {
      required: true,
      type: Array
    }
  },
  methods: {
    gotoSubmission(techniqueSlug, submissionId) {
      this.$router.push(`/technique/${techniqueSlug}?submission=${submissionId}`)
    }
  },
  computed: {
    submissions() {
      return [...this.profileSubmissions].sort((a, b) => b.score - a.score)
    },
    ...mapState('techniques', ['lists', 'dictionary']),
    completedTechniques() {
      const submissions = this.submissions
        .filter((v, i, a) => a
          .map(x => x.techniqueId)
          .indexOf(v.techniqueId) === i);

      return submissions.map(submission => ({
        submission,
        technique: this.dictionary.techniques[submission.techniqueId]
      }))
    }
  }
}
</script>

<style scoped>

</style>
